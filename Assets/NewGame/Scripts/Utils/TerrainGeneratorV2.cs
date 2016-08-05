using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

		for (int y = 0; y < dimensions.y - 4; y += baseDimension) {
			for (int x = 0; x < dimensions.x - 4; x += baseDimension) {
				int[,] selectedTile = selectTile(x, y, dimensions);
				applyTile (map, selectedTile, new Point3(x,y,0));
			}
		}

		return map;
	}

	//Check to make sure that nothing is already in the spots on the map, then apply
	private static int[,] applyTile(int[,] map, int[,] tile, Point3 position){
		int width = tile.GetLength (0);
		int height = tile.GetLength (1);

		bool written = false;
		for (int ty = 0; ty < height; ty++) {
			for (int tx = 0; tx < width; tx++) {
				if (map [position.x + tx, position.y + ty] != 0) {
					written = true;
				}
			}
		}

		if (!written){
			for (int ty = 0; ty < height; ty++) {
				for (int tx = 0; tx < width; tx++) {
					map [position.x + tx, position.y + ty] = tile [tx, ty];
				}
			}
		}

		return map;
	}

	private static int[,] selectTile(int currX, int currY, Point3 dimensions){
		/*
		 * 
		 * int width = selectedTile.GetLength (0);
				int height = selectedTile.GetLength (1);
				*/

		int[,] selected = tiles [UnityEngine.Random.Range (0, tiles.Count)];
		while (currX + selected.GetLength (0) > dimensions.x || currY + selected.GetLength (1) > dimensions.y) {
			selected = tiles [UnityEngine.Random.Range (0, tiles.Count)];
		}
		return selected;
	}
}
