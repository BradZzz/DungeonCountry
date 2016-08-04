using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

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
		map = addCastles (map, 4);

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

		Point3 newPath;

		List<Point3> paths = new List<Point3> ();
		foreach (Rect box in boxes) {
			if (box.y > -1) {
				newPath = new Point3 (UnityEngine.Random.Range (box.x + 1, box.width - 2), box.y, 0);
				map [newPath.x, newPath.y] = 2;
				paths.Add (newPath);
			}
			if (box.x > -1) {
				newPath = new Point3 (box.x, UnityEngine.Random.Range (box.y + 1, box.height - 2), 0);
				map [newPath.x, newPath.y] = 2;
				paths.Add (newPath);
			}
			if (box.height < height) {
				newPath = new Point3 (UnityEngine.Random.Range (box.x + 1, box.width - 2), box.height, 0);
				map [newPath.x, newPath.y] = 2;
				paths.Add (newPath);
			}
			if (box.width < width) {
				newPath = new Point3 (box.width, UnityEngine.Random.Range (box.y + 1, box.height - 2), 0);
				map [newPath.x, newPath.y] = 2;
				paths.Add (newPath);
			}
		}
		return createRoads (paths, map);
	}

	public static int[,] makeAccessable(int[,] map, List<Rect> boxes, Rect excludeBox){

		int width = map.GetLength (0);
		int height = map.GetLength (1);

		Point3 newPath;

		List<Point3> paths = new List<Point3> ();

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
				paths.Add (new Point3(point.x, point.y, 0));
			}
			if (box.x > -1) {
				point = new Vector2 ((int)box.x, (int)UnityEngine.Random.Range (box.y + 1, box.height - 2));
				while (containsPoint(excludeBox, point) && !containsSibling(map, point) && !(map [(int)point.x, (int)point.y] == 1)) {
					point = new Vector2 ((int)box.x, (int)UnityEngine.Random.Range (box.y + 1, box.height - 2));
				}
				Debug.Log ("Removing: " + point.ToString());
				map [(int)point.x, (int)point.y] = 2;
				paths.Add (new Point3(point.x, point.y, 0));
			}
			if (box.height < height) {
				point = new Vector2 ((int)UnityEngine.Random.Range (box.x + 1, box.width - 2), (int)box.height);
				while (containsPoint(excludeBox, point) && !containsSibling(map, point) && !(map [(int)point.x, (int)point.y] == 1)) {
					point = new Vector2 ((int)UnityEngine.Random.Range (box.x + 1, box.width - 2), (int)box.height);
				}
				Debug.Log ("Removing: " + point.ToString());
				map [(int)point.x, (int)point.y] = 2;
				paths.Add (new Point3(point.x, point.y, 0));
			}
			if (box.width < width) {
				point = new Vector2 ((int)box.width, (int)UnityEngine.Random.Range (box.y + 1, box.height - 2));
				while (containsPoint(excludeBox, point) && !containsSibling(map, point) && !(map [(int)point.x, (int)point.y] == 1)) {
					point = new Vector2 ((int)box.width, (int)UnityEngine.Random.Range (box.y + 1, box.height - 2));
				}
				Debug.Log ("Removing: " + point.ToString());
				map [(int)point.x, (int)point.y] = 2;
				paths.Add (new Point3(point.x, point.y, 0));
			}
		}

		return createRoads (paths, map);
		//return map;
	}

	//generateMapv2(Point3 startingPos, Point3 destination, int rows, int columns, List<Point3> obs)
	private static int[,] createRoads(List<Point3> nodes, int[,] map){

		int width = map.GetLength (0);
		int height = map.GetLength (1);

		List<Point3> obs = new List<Point3> ();
		for (int y = 0; y < height; y++){
			for (int x = 0; x < width; x++){
				if (map [x,y] == 1) {
					obs.Add (new Point3(x,y,0));
				}
			}
		}
		map = shuffleMap(nodes, map, obs);
		map = shuffleMap(nodes, map, obs);
		return map = shuffleMap(nodes, map, obs);
	}

	private static int[,] shuffleMap(List<Point3> nodes, int[,] map, List<Point3> obs){

		int width = map.GetLength (0);
		int height = map.GetLength (1);

		Coroutines.ShuffleArray(nodes);

		ShortestPath sPath = new ShortestPath ();
		List<Point3> thisMap;

		Point3 start, end;
		start = nodes [0];
		for (int i = 1; i < nodes.Count; i++) {
			end = nodes [i];
			thisMap = sPath.generateMapv2 (start, end, height, width, obs);
			if (thisMap != null) {
				foreach (Point3 point in thisMap) {
					map [point.x, point.y] = 2;
					//obs.Add (new Point3(point.x,point.y,0));
				}
			}
			start = end;
		}

		start = nodes[nodes.Count - 1];
		end = nodes [0];

		thisMap = sPath.generateMapv2 (start, end, height, width, obs);
		if (thisMap != null) {
			foreach (Point3 point in thisMap) {
				map [point.x, point.y] = 2;
			}
		}

		return map;
	}

	private static bool checkPlot(int[,] map, Point3 plot){
		return map [plot.x, plot.y] == 2 && map [plot.x + 1, plot.y] == 2 && map [plot.x, plot.y + 1] == 0 && map [plot.x, plot.y + 2] == 0 && map [plot.x + 1, plot.y + 1] == 0 && map [plot.x + 1, plot.y + 2] == 0;
	}

	private static int[,] addCastles(int[,] map, int castles){

		int width = map.GetLength (0);
		int height = map.GetLength (1);

		List<Point3> roads = new List<Point3> ();
		//Castles cant be on the top square
		for (int y = 2; y < height && castles > 0; y++){
			for (int x = 0; x < width - 2 && castles > 0; x++){
				if (map [x, y] == 2){
					roads.Add (new Point3 (x, y, 0));
				}
			}
		}

		Coroutines.ShuffleArray(roads);

		foreach (Point3 road in roads) {
			if (checkPlot (map, road)){
				//This is the castle base
				map [road.x, road.y + 1] = 12;
				//These are the tiles that the castle is also covering
				map [road.x, road.y + 2] = 12;
				map [road.x + 1, road.y + 1] = 12;
				map [road.x + 1, road.y + 2] = 11;
				castles--;
				if (castles <= 0) {
					break;
				}
			}
		}

		return map;
	}

	private static bool containsSibling(int[,] map, Vector3 point){
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
