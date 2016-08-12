using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System;

public class AdventureWorldCreator : MonoBehaviour {
	public GameObject[] outerWallTiles;
	public GameObject[] innerWallTiles;
	public GameObject[] outerFloorTiles;
	public GameObject[] floorTiles;
	public GameObject[] roadTiles;
	public GameObject[] bloodTiles;
	public GameObject[] castleTiles;
	public GameObject[] entranceTiles;
	public GameObject[] foundationTiles;
	public GameObject[] cliffTiles;
	public GameObject[] waterTiles;
	public GameObject[] bridgeTiles;
	public GameObject[] dwellingTiles;
	public GameObject[] resourceTiles;
	public GameObject footsteps;

	private int waters = 4;
	private static int odds = 50;

	public GameObject cliffNinePatch;
	private Footsteps steps;
	private TileNinePatch cliffPatch;

	private Transform board;
	private List<Point3> openPositions;
	private List<Point3> roadPositions;

	private Action<List<Point3>, List<Point3>> callback;

	public void createWorld(Transform board, int columns, int rows, Action<List<Point3>, List<Point3>> callback){
		this.board = board;
		this.callback = callback;

		steps = footsteps.GetComponent<Footsteps>();
		cliffPatch = cliffNinePatch.GetComponent<TileNinePatch> ();

		openPositions = new List<Point3> ();
		roadPositions = new List<Point3> ();


		constructBoard (columns, rows);
		constructRiverMap(TerrainGeneratorV2.generateMap (new Point3 (columns, rows, 0)));
	}

	private void constructBoard (int columns, int rows)
	{
		for(int x = -1; x <= columns; x++)
		{
			for(int y = -1; y <= rows; y++)
			{
				bool outer = y == -1 || x == -1 || y == rows || x == columns;
				GameObject toInstantiate;
				if (outer) {
					toInstantiate = outerFloorTiles [UnityEngine.Random.Range (0, outerFloorTiles.Length)];
				} else {
					toInstantiate = floorTiles [UnityEngine.Random.Range (0, floorTiles.Length)];
				}

				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (board);

				if (outer) {
					toInstantiate = outerWallTiles [UnityEngine.Random.Range (0, outerWallTiles.Length)];
					instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent (board);
				}
			}
		}
	}

	private void constructRiverMap(int[,] map){
		//Find empty at top
		//Find empty at bottom
		//Remove every other road

		int width = map.GetLength (0);
		int height = map.GetLength (1);

		List<Point3> bottom = new List<Point3> ();
		List<Point3> tops = new List<Point3> ();
		List<Point3> obstacles = new List<Point3> ();

		Point3 currentPos = new Point3(-1,0,0);

		for (int x = 1; x < width -1; x++) {
			if (map[x - 1,0] == 0 && map[x,0] == 0 && map[x + 1,0] == 0) {
				currentPos = new Point3(x,0,0);
				bottom.Add (currentPos);
			}
			if (map[x - 1,height - 1] == 0 && map[x,height - 1] == 0 && map[x + 1,height - 1] == 0) {
				currentPos = new Point3(x,height - 1,0);
				tops.Add (currentPos);
			}
		}

		//Debug.Log ("Bottom: " + bottom.Count);
		//Debug.Log ("Tops: " + tops.Count);

		Coroutines.ShuffleArray (bottom);
		Coroutines.ShuffleArray (tops);

		bool switchy = false;

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if ((map[x,y] != 0 && map[x,y] != 2) || (x == 0 || y == 0 || x == width - 1 || y == height - 1)) {
					obstacles.Add (new Point3 (x, y, 0));
				} else if (map[x,y] == 2) {
					if (!solid (map, new Point3 (x, y, 0))) {
						switchy = !switchy;
						if (switchy) {
							obstacles.Add (new Point3 (x, y, 0));
						}
					} else {
						obstacles.Add (new Point3 (x, y, 0));
					}
				} else if (map[x,y] == 0) {
					Point3 pos = new Point3 (x, y, 0);
					//generateMapv2Serial
					int grassPatch = pos.returnPatchLocation(map, 0);

					int materialPatch = pos.returnPatchLocation(map, 20);
					if ((materialPatch == 3 || materialPatch == 1)) {
						//Resource
						if (UnityEngine.Random.Range (0, 150) > odds) {
							map [pos.x, pos.y] = 40;
						}
					}
					int dwellingPatch = pos.returnPatchLocation(map, 2);
					if ((dwellingPatch == 7 || dwellingPatch == 9) /*&& (grassPatch > 3)*/) {
						//Dwelling
						if (UnityEngine.Random.Range (0, 80) > odds) {
							map [pos.x, pos.y] = 30;
							map [pos.x, pos.y-1] = 31;
						}
					}
				}
			}
		}

		if (bottom.Count == 0 || tops.Count == 0) {
			deployTerrainSprites (new List<Point3> (), map);
		} else {
			waterMap(steps.generateMapv2Serial (bottom[0], tops[0], height, width, obstacles, map), map);

		}
	}

	private bool solid (int[,] map, Point3 position){
		if (position.awayFromEdge(map) && position.crowded(map, 0, 2)){
			return true;
		}
		return false;
	}

	public bool siblings(int[,] map, Point3 pos, int search, bool vertical){
		int width = map.GetLength (0);
		int height = map.GetLength (1);

		//vertical
		if (vertical) {
			if (pos.y > 0 && pos.y < height - 1 && map[pos.x, pos.y -1] == search && map[pos.x, pos.y + 1] == search) {
				return true;
			}
			return false;
			//else horizontal
		} else {
			if (pos.x > 0 && pos.x < width - 1 && map[pos.x - 1, pos.y] == search && map[pos.x + 1, pos.y] == search) {
				return true;
			}
			return false;
		}
	}

	private void waterMap(List<Point3> path, int[,] map){
		if (path != null) {
			//The more water there is, the more resources there are...
			foreach (Point3 water in path) {
				if (map [water.x, water.y] == 0) {
					map [water.x, water.y] = 3;

				}
			}
		}
		if (waters > 0) {
			waters--;
			constructRiverMap (map);
		} else {
			deployTerrainSprites(path, map);
		}
	}

	private void deployTerrainSprites (List<Point3> path, int[,] map){

		for (int y = 0; y < map.GetLength(1); y++) {
			for (int x = 0; x < map.GetLength(0); x++) {
				Point3 pos = new Point3 (x,y,0);

				if (map[x,y] == 0) {
					openPositions.Add (pos);
				}

				//Wall = 1
				//Could be hedges or rocks
				if (map[x,y] == 1) {
					GameObject tileChoice = innerWallTiles[UnityEngine.Random.Range (0, innerWallTiles.Length)];
					GameObject instance = Instantiate (tileChoice, pos.asVector3(), Quaternion.identity) as GameObject;
					instance.transform.SetParent (board);
				}
				//Road = 2
				if (map[x,y] == 2) {
					GameObject tileChoice;
					bool sibVer = siblings (map, new Point3 (pos), 3, true);
					bool sibHor = siblings (map, new Point3 (pos), 3, false);

					if (sibVer || sibHor) {
						tileChoice = bridgeTiles [UnityEngine.Random.Range (0, bridgeTiles.Length)];
					} else {
						tileChoice = roadTiles [UnityEngine.Random.Range (0, roadTiles.Length)];
					}
					GameObject instance = Instantiate (tileChoice, pos.asVector3 (), Quaternion.identity) as GameObject;
					if (sibHor) {
						SpriteRenderer sprite = instance.GetComponent<SpriteRenderer> ();
						sprite.transform.Rotate (new Vector3(0,0,90));
					}
					instance.transform.SetParent (board);
					roadPositions.Add (pos);
				}

				//water = 3
				if (map[x,y] == 3) {
					GameObject tileChoice = waterTiles[UnityEngine.Random.Range (0, waterTiles.Length)];
					GameObject instance = Instantiate (tileChoice, pos.asVector3(), Quaternion.identity) as GameObject;
					instance.transform.SetParent (board);
				}

				//Castle = 1x
				//10 = entrance
				//11 = sprite placement/foundation
				//12 = foundation
				if (map[x,y] == 10 || map[x,y] == 11 || map[x,y] == 12) {
					if (map [x, y] == 10) {
						GameObject tileChoice = entranceTiles [UnityEngine.Random.Range (0, entranceTiles.Length)];
						GameObject instance = Instantiate (tileChoice, pos.asVector3 (), Quaternion.identity) as GameObject;
						instance.transform.SetParent (board);
					} else {
						GameObject tileChoice = foundationTiles [UnityEngine.Random.Range (0, foundationTiles.Length)];
						GameObject instance = Instantiate (tileChoice, pos.asVector3 (), Quaternion.identity) as GameObject;
						instance.transform.SetParent (board);

						if (map [x, y] == 11) {
							Vector3 vect = pos.asVector3 ();
							vect.x -= .5f;
							vect.y -= .5f;
							tileChoice = castleTiles [UnityEngine.Random.Range (0, castleTiles.Length)];
							instance = Instantiate (tileChoice, vect, Quaternion.identity) as GameObject;
							instance.transform.SetParent (board);
						}
					}
				}

				//cliffs = 2x
				//More coherent walls
				if (map[x,y] == 20) {
					//GameObject tileChoice = cliffTiles[UnityEngine.Random.Range (0, cliffTiles.Length)];
					int pathPos = pos.returnPatchLocation(map,20);
					//Debug.Log ("PathPos: " + pathPos);
					//Debug.Log ("Patch: " + cliffPatch.returnPatch(pathPos).name);

					GameObject instance = Instantiate (cliffPatch.returnPatch(pathPos), pos.asVector3(), Quaternion.identity) as GameObject;
					instance.transform.SetParent (board);
				}

				//dwellings = 3x
				if (map[x,y] == 30 || map[x,y] == 31) {

					if (map [x, y] == 30) {
						GameObject tileChoice = foundationTiles [UnityEngine.Random.Range (0, foundationTiles.Length)];
						GameObject instance = Instantiate (tileChoice, pos.asVector3 (), Quaternion.identity) as GameObject;
						instance.transform.SetParent (board);
						tileChoice = dwellingTiles [UnityEngine.Random.Range (0, dwellingTiles.Length)];
						instance = Instantiate (tileChoice, pos.asVector3 (), Quaternion.identity) as GameObject;
						instance.transform.SetParent (board);
					} else if (map [x, y] == 31) {
						GameObject tileChoice = entranceTiles [UnityEngine.Random.Range (0, entranceTiles.Length)];
						GameObject instance = Instantiate (tileChoice, pos.asVector3 (), Quaternion.identity) as GameObject;
						instance.transform.SetParent (board);
					}
				}

				//resources = 4x
				if (map[x,y] == 40) {
					GameObject tileChoice = resourceTiles [UnityEngine.Random.Range (0, resourceTiles.Length)];
					GameObject instance = Instantiate (tileChoice, pos.asVector3 (), Quaternion.identity) as GameObject;
					instance.transform.SetParent (board);
				}
			}
		}
		callback (openPositions, roadPositions);
	}

}
