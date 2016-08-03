using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGenerator {

	public static int[,] generateBoxedMap (Vector2 dimensions, int players) {
		//Find the biggest side of the map
		bool widthBiggest = dimensions.x > dimensions.y;

		int[,] map = new int[(int)dimensions.x,(int)dimensions.y];

		Debug.Log ("Width Bigger: " + widthBiggest);

		Debug.Log ("Half width: " + dimensions.y / 2);

		List<Rect> boxes = new List<Rect> ();

		if (widthBiggest) {
			boxes.Add (new Rect (-1, -1, (int) dimensions.x / 3, dimensions.y));
			boxes.Add (new Rect ((int) dimensions.x / 3, (int) dimensions.y / 2, dimensions.x, dimensions.y));
			boxes.Add (new Rect ((int) dimensions.x / 3, -1, dimensions.x, (int) dimensions.y / 2));
		} else {
			boxes.Add (new Rect (-1, -1, dimensions.x, (int) dimensions.y / 3));
			boxes.Add (new Rect (-1, (int) dimensions.y / 3, (int) dimensions.x / 2, dimensions.y));
			boxes.Add (new Rect ((int) dimensions.x / 2, (int) dimensions.y / 3, dimensions.x, dimensions.y));
		}

		for (int y = 0; y < dimensions.y; y++){
			for (int x = 0; x < dimensions.x; x++){
				Vector3 check = new Vector3 ((float)x, (float)y, 0);
				if (overlap(boxes,check)) {
					map [x,y] = 1;
				} else {
					map [x,y] = 0;
				}
			}
		}

		map = makeAccessable (map, boxes);

		return map;
	}

	public static int[,] generateTargetMap (Vector2 dimensions, int players) {
		int[,] map = new int[(int)dimensions.x,(int)dimensions.y];

		List<Rect> boxes = new List<Rect> ();

		//Top Left
		boxes.Add (new Rect (-1, -1, (int) dimensions.x / 2, (int) dimensions.y /2));

		//Top Right
		boxes.Add (new Rect ((int) dimensions.x / 2, -1, (int) dimensions.x, (int) dimensions.y /2));

		//Bottom Left
		boxes.Add (new Rect (-1, (int) dimensions.y /2, (int) dimensions.x / 2, (int) dimensions.y));

		//Bottom Right
		boxes.Add (new Rect ((int) dimensions.x / 2, (int) dimensions.y / 2, (int) dimensions.x, (int) dimensions.y));

		//Middle Box
		boxes.Add (new Rect ((int) dimensions.x / 3, (int) dimensions.y / 3, (int) (2 * dimensions.x / 3), (int) (2 * dimensions.y / 3)));

		Rect excludeBox = new Rect ((int) dimensions.x / 3 + 1, (int) dimensions.y / 3 + 1, (int) (2 * dimensions.x / 3) - 1, (int) (2 * dimensions.y / 3) - 1);

		for (int y = 0; y < dimensions.y; y++){
			for (int x = 0; x < dimensions.x; x++){
				Vector3 check = new Vector3 ((float)x, (float)y, 0);
				if (overlap(excludeBox, boxes,check)) {
					map [x,y] = 1;
				} else {
					map [x,y] = 0;
				}
			}
		}

		map = makeAccessable (map, boxes, excludeBox);

		return map;
	}

	public static bool overlap(List<Rect> boxes, Vector3 point){
		foreach (Rect box in boxes) {
			if ((box.x <= point.x && point.x <= box.width && (point.y == box.y || point.y == box.height))||
				(box.y <= point.y && point.y <= box.height && (point.x == box.x || point.x == box.width))) {
				return true;
			}
		}
		return false;
	}

	public static bool overlap(Rect excludeBox, List<Rect> boxes, Vector3 point){
		foreach (Rect box in boxes) {
			if (((box.x <= point.x && point.x <= box.width && (point.y == box.y || point.y == box.height))||
				(box.y <= point.y && point.y <= box.height && (point.x == box.x || point.x == box.width))) &&
				(!containsPoint(excludeBox, point))) {

				return true;
			}
		}
		return false;
	}

	public static int[,] makeAccessable(int[,] map, List<Rect> boxes){

		int width = map.GetLength (0);
		int height = map.GetLength (1);

		foreach (Rect box in boxes) {
			if (box.y > -1) {
				map [(int)UnityEngine.Random.Range (1, box.width - 2), (int)box.y] = 2;
			}
			if (box.x > -1) {
				map [(int)box.x, (int)UnityEngine.Random.Range (1, box.height - 2)] = 2;
			}
			if (box.height < height) {
				map [(int)UnityEngine.Random.Range (1, box.width - 2), (int)box.height] = 2;
			}
			if (box.width < width) {
				map [(int)box.width, (int)UnityEngine.Random.Range (1, box.height - 2)] = 2;
			}
		}
		return map;
	}

	public static int[,] makeAccessable(int[,] map, List<Rect> boxes, Rect excludeBox){

		int width = map.GetLength (0);
		int height = map.GetLength (1);

		foreach (Rect box in boxes) {

			Debug.Log ("Box: " + box.x + ":" + box.y + " - " + box.width + ":" + box.height);

			Vector2 point;
			if (box.y > -1) {
				point = new Vector2 ((int)UnityEngine.Random.Range (box.x + 1, box.width - 2), (int)box.y);
				while (containsPoint(excludeBox, point) && !containsSibling(map, point) && !(map [(int)point.x, (int)point.y] == 1)) {
					point = new Vector2 ((int)UnityEngine.Random.Range (box.x + 1, box.width - 2), (int)box.y);
				}
				Debug.Log ("Removing: " + point.ToString());
				map [(int)point.x, (int)point.y] = 2;
			}
			if (box.x > -1) {
				point = new Vector2 ((int)box.x, (int)UnityEngine.Random.Range (box.y + 1, box.height - 2));
				while (containsPoint(excludeBox, point) && !containsSibling(map, point) && !(map [(int)point.x, (int)point.y] == 1)) {
					point = new Vector2 ((int)box.x, (int)UnityEngine.Random.Range (box.y + 1, box.height - 2));
				}
				Debug.Log ("Removing: " + point.ToString());
				map [(int)point.x, (int)point.y] = 2;
			}
			if (box.height < height) {
				point = new Vector2 ((int)UnityEngine.Random.Range (box.x + 1, box.width - 2), (int)box.height);
				while (containsPoint(excludeBox, point) && !containsSibling(map, point) && !(map [(int)point.x, (int)point.y] == 1)) {
					point = new Vector2 ((int)UnityEngine.Random.Range (box.x + 1, box.width - 2), (int)box.height);
				}
				Debug.Log ("Removing: " + point.ToString());
				map [(int)point.x, (int)point.y] = 2;
			}
			if (box.width < width) {
				point = new Vector2 ((int)box.width, (int)UnityEngine.Random.Range (box.y + 1, box.height - 2));
				while (containsPoint(excludeBox, point) && !containsSibling(map, point) && !(map [(int)point.x, (int)point.y] == 1)) {
					point = new Vector2 ((int)box.width, (int)UnityEngine.Random.Range (box.y + 1, box.height - 2));
				}
				Debug.Log ("Removing: " + point.ToString());
				map [(int)point.x, (int)point.y] = 2;
			}
		}
		return map;
	}

	public static bool containsSibling(int[,] map, Vector3 point){
		//Has to have exactly two siblings
		//Walls taken out of intersections are worthless
		int count = 0;

		if (point.x - 1 > -1) {
			if (map[(int)point.x - 1, (int)point.y] == 1) {
				count++;
			}
			if (map[(int)point.x - 1, (int)point.y] == 2) {
				return false;
			}
		}

		if (point.y - 1 > -1) {
			if (map[(int)point.x + 1, (int)point.y] == 1) {
				count++;
			}
			if (map[(int)point.x + 1, (int)point.y] == 2) {
				return false;
			}
		}


		if (point.x + 1 < map.GetLength (0)) {
			if (map[(int)point.x, (int)point.y - 1] == 1) {
				count++;
			}
			if (map[(int)point.x, (int)point.y - 1] == 2) {
				return false;
			}
		}

		if (point.y + 1 < map.GetLength (1)) {
			if (map[(int)point.x, (int)point.y + 1] == 1) {
				count++;
			}
			if (map[(int)point.x, (int)point.y + 1] == 2) {
				return false;
			}
		}

		return count == 2;
	}

	public static bool containsPoint(Rect box, Vector3 point){
		if (box.x <= point.x && box.width >= point.x &&
			box.y <= point.y && box.height >= point.y) {
			return true;
		}
		return false;
	}
}
