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

		int baseDimension = 5;

		int[,] map = new int[dimensions.x, dimensions.y];

		tiles = TerrainTiles.returnTiles();
		positions = new List<Point3> ();

		//Initialize all the positions to
		for (int y = 0; y < dimensions.y - 4; y += baseDimension) {
			for (int x = 0; x < dimensions.x - 4; x += baseDimension) {
				map [x, y] = 0;
			}
		}

		for (int y = dimensions.y - baseDimension; y >= 0; y -= baseDimension) {
			for (int x = 0; x < dimensions.x; x += baseDimension) {
				Point3 pos = new Point3 (x, y, 0);
				if (!somethingThere(map, baseDimension, baseDimension, pos)) {
					int[,] selectedTile = selectTile(map, new Point3(x, y, 0));
					applyTile (map, selectedTile, pos);
				}
			}
		}

		return map;
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
		openConnect mapConnectTop;
		if (position.x != 0) {
			int[,] sliced = sliceArray (map, new Point3 (position.x - 5, position.y, 0), new Point3 (5, 5, 0));
			mapConnectLeft = new openConnect (sliced, true);
			mapConnectLeft.printMap ();
		} else {
			mapConnectLeft = new openConnect ();
		}

		if (position.y + 5 <= height) {
			int[,] sliced = sliceArray (map, new Point3 (position.x, position.y + 5, 0), new Point3 (5, 5, 0));
			mapConnectTop = new openConnect (sliced, true);
			mapConnectTop.printMap ();
		} else {
			mapConnectTop = new openConnect ();
		}

		//For now, let's just look to the left
		foreach (int[,] tile in allTiles) {
			if (position.x == 0) {
				returnTiles.Add (tile);
			} else {
				//These are the connecting parts on the tile
				//openConnect tileConnect = new openConnect (tile, false);
				//tileConnect.printMap ();
				openConnect tileConnect = new openConnect (tile, false);
				tileConnect.printMap ();

				if (((mapConnectLeft.right && tileConnect.left) || (!mapConnectLeft.right && !tileConnect.left)) &&
					((mapConnectTop.down && tileConnect.up) || (!mapConnectTop.down && !tileConnect.up))) {
					returnTiles.Add (tile);
				}
			}
		}

		Debug.Log ("Something");

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
				Debug.Log (row);
			}
			Debug.Log ("Directions: l=> " + left + " r=> " + right + " up=> " + up + " down=> " + down);
		}
	}
}
