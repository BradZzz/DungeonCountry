using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using AssemblyCSharp;

public class TerrainGeneratorV2 : MonoBehaviour {

	//Alright, this is the revised terrain generator. 
	//It makes much more sense to break up the bigger problem into smaller problems
	//This means that we need to require the board to be a multiple of 5 so that we can tile it appropriately
	static List<int[,]> tiles;
	static List<Point3> positions;

	public static int[,] generateMap (Point3 dimensions) {

		int[,] map = new int[dimensions.x, dimensions.y];

		tiles = TerrainTiles.returnTiles();
		positions = new List<Point3> ();

		//Initialize all the positions to
		for (int y = 0; y < dimensions.y - 4; y += TerrainTiles.terrainSizeSmall) {
			for (int x = 0; x < dimensions.x - 4; x += TerrainTiles.terrainSizeSmall) {
				map [x, y] = 0;
			}
		}

		for (int y = dimensions.y - TerrainTiles.terrainSizeSmall; y >= 0; y -= TerrainTiles.terrainSizeSmall) {
			for (int x = 0; x < dimensions.x; x += TerrainTiles.terrainSizeSmall) {
				Point3 pos = new Point3 (x, y, 0);
				if (!somethingThere(map, TerrainTiles.terrainSizeSmall, TerrainTiles.terrainSizeSmall, pos)) {
					int[,] selectedTile = selectTile(map, new Point3(x, y, 0));
					applyTile (map, selectedTile, pos);
				}
			}
		}
			
		/*List<Point3> rivers = buildRivers (map);
		foreach (Point3 river in rivers) {
			if (map [river.x, river.y] == 0) {
				map [river.x, river.y] = 3;
			}
		}*/

		return map;
	}

	private static List<Point3> buildRivers(int[,] map){
		//Pick random point on edge of map:
		//if that point isn't a 0, repick
		//else, pick second point on edge of map
		int width = map.GetLength (0);
		int height = map.GetLength (1);

		List<Point3> steps = new List<Point3> ();

		Point3 currentPos = new Point3(-1,0,0);

		for (int x = 1; x < width -1; x++) {
			if (map[x - 1,0] == 0 && map[x,0] == 0 && map[x + 1,0] == 0) {
				currentPos = new Point3(x,0,0);
				//map [x, 0] = 3;
				steps.Add (currentPos);
				break;
			}
		}

		if (currentPos.x != -1) {
			while (currentPos.y+1 < height) {
				List<Point3> rivers = checkClear (map, currentPos);
				Coroutines.ShuffleArray (rivers);
				if (rivers.Count > 0) {
					currentPos = rivers [0];
					//if (map [rivers [0].x, rivers [0].y] == 0) {
					//	map [rivers [0].x, rivers [0].y] = 3;
					//}
					steps.Add (currentPos);
				} else {
					break;
				}
			}
		}

		return steps;
	}

	private static List<Point3> checkClear(int[,] map, Point3 current){
		int width = map.GetLength (0);
		int height = map.GetLength (1);

		//check left, right, and up
		List<Point3> directions = new List<Point3>();

		//Left
		if (current.x-2 > -1 && current.y - 1 > -1 && current.y+1 < height) {
			Point3 leftishLeft = new Point3 (current.x - 1, current.y - 1, 0);
			Point3 leftishCenter = new Point3 (current.x - 1, current.y, 0);
			Point3 leftishRight = new Point3 (current.x - 1, current.y + 1, 0);

			bool leftRiver = map [leftishLeft.x, leftishLeft.y] == 3 || map [leftishCenter.x, leftishCenter.y] == 3 || map [leftishRight.x, leftishRight.y] == 3;

			int total = map [leftishLeft.x, leftishLeft.y] + map [leftishCenter.x, leftishCenter.y] + map [leftishRight.x, leftishRight.y];
			if (!leftRiver && (total == 6 || total == 0)) {
				directions.Add(leftishCenter);
			}
		}

		//Right
		if (current.x+2 < width && current.y - 1 > -1 && current.y+1 < height) {
			Point3 rightishLeft = new Point3 (current.x + 1, current.y - 1, 0);
			Point3 rightishCenter = new Point3 (current.x + 1, current.y, 0);
			Point3 rightishRight = new Point3 (current.x + 1, current.y + 1, 0);

			bool rightRiver = map [rightishLeft.x, rightishLeft.y] == 3 || map [rightishCenter.x, rightishCenter.y] == 3 || map [rightishRight.x, rightishRight.y] == 3;

			int total = map [rightishLeft.x, rightishLeft.y] + map [rightishCenter.x, rightishCenter.y] + map [rightishRight.x, rightishRight.y];
			if (!rightRiver && (total == 6 || total == 0)) {
				directions.Add(rightishCenter);
			}
		}

		//up
		if (current.y+1 < height) {
			Point3 upishLeft = new Point3 (current.x - 1, current.y + 1, 0);
			Point3 upishCenter = new Point3 (current.x, current.y + 1, 0);
			Point3 upishRight = new Point3 (current.x + 1, current.y + 1, 0);

			bool upRiver = map [upishLeft.x, upishLeft.y] == 3 || map [upishCenter.x, upishCenter.y] == 3 || map [upishRight.x, upishRight.y] == 3;

			int total = map [upishLeft.x, upishLeft.y] + map [upishCenter.x, upishCenter.y] + map [upishRight.x, upishRight.y];
			if (!upRiver && (total == 6 || total == 0)) {
				directions.Add(upishCenter);
			}
		}
		return directions;
	}

	private static bool somethingThere(int[,] map, int width, int height, Point3 position){
		for (int ty = 0; ty < height; ty++) {
			for (int tx = 0; tx < width; tx++) {
				if (map [position.x + tx, position.y + ty] != 0) {
					return true;
				}
			}
		}
		return false;
	}

	//Check to make sure that nothing is already in the spots on the map, then apply
	private static int[,] applyTile(int[,] map, int[,] tile, Point3 position){

		tile = Coroutines.RotateMatrixCounterClockwise(Coroutines.RotateMatrixCounterClockwise(Coroutines.RotateMatrixCounterClockwise(tile)));

		int width = tile.GetLength (0);
		int height = tile.GetLength (1);

		for (int ty = 0; ty < height; ty++) {
			for (int tx = 0; tx < width; tx++) {
				map [position.x + tx, position.y + ty] = tile [tx, ty];
			}
		}

		return map;
	}

	private static int[,] selectTile(int[,] map, Point3 position){

		List<int[,]> useableTiles = trimTiles (tiles, map, position);

		if (useableTiles.Count < 1) {
			Debug.Log ("ERROR!!!!!!!");
			useableTiles = tiles;
		}

		int width = map.GetLength (0);
		int height = map.GetLength (1);

		int[,] selected = useableTiles [UnityEngine.Random.Range (0, useableTiles.Count)];
		while (position.x + selected.GetLength (0) > width || position.y + selected.GetLength (1) > height) {
			selected = useableTiles [UnityEngine.Random.Range (0, useableTiles.Count)];
		}

		openConnect tileConnect = new openConnect (selected, false);
		tileConnect.printMap ();

		return selected;
	}

	private static List<int[,]> trimTiles(List<int[,]> allTiles, int[,] map, Point3 position){

		int width = map.GetLength (0);
		int height = map.GetLength (1);

		List<int[,]> returnTiles = new List<int[,]> ();
		openConnect mapConnectLeft;
		openConnect mapConnectRight;
		openConnect mapConnectTop;

		if (position.x != 0) {
			int[,] sliced = sliceArray (map, new Point3 (position.x - TerrainTiles.terrainSizeSmall, position.y, 0), new Point3 (TerrainTiles.terrainSizeSmall, TerrainTiles.terrainSizeSmall, 0));
			mapConnectLeft = new openConnect (sliced, true);
			mapConnectLeft.printMap ();
		} else {
			mapConnectLeft = new openConnect ();
		}

		if (position.y + TerrainTiles.terrainSizeSmall < height) {
			int[,] sliced = sliceArray (map, new Point3 (position.x, position.y + TerrainTiles.terrainSizeSmall, 0), new Point3 (TerrainTiles.terrainSizeSmall, TerrainTiles.terrainSizeSmall, 0));
			mapConnectTop = new openConnect (sliced, true);
			mapConnectTop.printMap ();
		} else {
			mapConnectTop = new openConnect ();
		}
			
		Point3 rightish = new Point3 (position.x + TerrainTiles.terrainSizeSmall, position.y, 0);

		if (position.x + TerrainTiles.terrainSizeSmall < width && somethingThere (map, TerrainTiles.terrainSizeSmall, TerrainTiles.terrainSizeSmall, rightish)) {
			int[,] sliced = sliceArray (map, rightish, new Point3 (TerrainTiles.terrainSizeSmall, TerrainTiles.terrainSizeSmall, 0));
			mapConnectRight = new openConnect (sliced, true);
			mapConnectRight.printMap ();
		} else {
			mapConnectRight = new openConnect ();
		}

		//For now, let's just look to the left
		foreach (int[,] tile in allTiles) {
			if (position.x == 0 && position.y + TerrainTiles.terrainSizeSmall >= height) {
				returnTiles.Add (tile);
			} else {
				//These are the connecting parts on the tile
				//openConnect tileConnect = new openConnect (tile, false);
				//tileConnect.printMap ();
				openConnect tileConnect = new openConnect (tile, false);
				tileConnect.printMap ();

				if (((mapConnectLeft.right && tileConnect.left) || (!mapConnectLeft.right && !tileConnect.left)) &&
					((mapConnectTop.down && tileConnect.up) || (!mapConnectTop.down && !tileConnect.up)) /*&& 
					((mapConnectRight.left && tileConnect.right) || (!mapConnectRight.left && !tileConnect.right))*/) {
					returnTiles.Add (tile);
				}
			}
		}

		//Debug.Log ("Something");

		return returnTiles;
	}

	private static int[,] sliceArray(int[,] map, Point3 position, Point3 dimension){

		int[,] slice = new int[dimension.x, dimension.y];

		for (int y = 0; y < dimension.y; y ++) {
			for (int x = 0; x < dimension.x; x ++) {
				try{
					slice [x, y] = map [x + position.x, y + position.y];
				}catch(Exception e){
					int width = map.GetLength (0);
					int height = map.GetLength (1);
					Debug.Log ("Error at: " + x + ":" + y + " Position: " + position.x + ":" + position.y + " Combined: " + (x + position.x) + ":" + (y + position.y));
				}
			}
		}

		return slice;
	}

	private class openConnect {
		public bool left, right, up, down;
		public int[,] map;

		public openConnect(){
			left = true;
			right = true;
			up = true;
			down = true;
		}

		public openConnect(int[,] map, bool rotate){
			//rotate = rotate -90 degrees

			int width = map.GetLength (0);
			int height = map.GetLength (1);

			//left
			for (int y = 0; y < height; y ++) {
				if (map[0,y] == 2) {
					if (rotate){
						up = true;
						//down = true;
					} else {
						left = true;
						//right = true;
					}
				}
			}

			//right
			for (int y = 0; y < height; y ++) {
				if (map[width - 1,y] == 2) {
					if (rotate){
						down = true;
						//up = true;
					} else {
						right = true;
						//left = true;
					}
				}
			}

			//bottom
			for (int x = 0; x < width; x ++) {
				if (map[x, height - 1] == 2) {
					if (rotate){
						left = true;
						//right  = true;
					} else {
						down = true;
						//up = true;
					}
				}
			}

			//top
			for (int x = 0; x < width; x ++) {
				if (map[x,0] == 2) {
					if (rotate){
						right = true;
						//left = true;
					} else {
						up = true;
						//down = true;
					}
				}
			}

			bool tLeft, tRight, tDown, tUp;

			tLeft = up;
			tUp = left;
			tRight = down;
			tDown = right;

			up = tUp;
			left = tLeft;
			down = tDown;
			right = tRight;

			this.map = map;
		}

		public void printMap(){
			int width = map.GetLength (0);
			int height = map.GetLength (1);
			for (int y = 0; y < height; y ++) {
				string row = "";
				for (int x = 0; x < width; x ++) {
					row += x;
				}
				//Debug.Log (row);
			}
			//Debug.Log ("Directions: l=> " + left + " r=> " + right + " up=> " + up + " down=> " + down);
		}
	}
}
