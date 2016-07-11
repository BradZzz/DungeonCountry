using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class MazeGenerator
	{
		private bool [,] map;
		private List<Vector2> toExamine;
		private Vector2 dimensions;

		public MazeGenerator (Vector2 start, Vector2 end, Vector2 dimensions)
		{
			toExamine = new List<Vector2> ();
			this.dimensions = dimensions;

			map = new bool[(int)dimensions.x, (int)dimensions.y];
			for (int y = 0; y < dimensions.y; y++) {
				for (int x = 0; x < dimensions.x; x++) {
					//Make all the positions unexamined to start out
					map [x, y] = true;
				}
			}
			//Remove the start and the exit from the unexamined map
			map [(int)start.x, (int)start.y] = false;
			ClearSquare (end);

			examinePoint (start);

			while (toExamine.Count > 0) {
				examinePoint (toExamine[0]);
				toExamine.RemoveAt (0);
				toExamine.Sort((a, b)=> 1 - 2 * UnityEngine.Random.Range(0, 1));
			}
		}

		//Clear a square around a point. Only used for the exit now. Maybe something else later on
		private void ClearSquare(Vector2 point){
			if (point.x >= 0 && point.x < dimensions.x && point.y >= 0 && point.y < dimensions.y) {
				map [(int)point.x, (int)point.y] = false;
			}

			if (point.x - 1 >= 0) {
				map [(int)point.x - 1, (int)point.y] = false;
			}
			if (point.x + 1 < dimensions.x) {
				map [(int)point.x + 1, (int)point.y] = false;
			}

			if (point.y - 1 >= 0) {
				map [(int)point.x, (int)point.y - 1] = false;
			}
			if (point.y + 1 < dimensions.y) {
				map [(int)point.x, (int)point.y + 1] = false;
			}

			if (point.x + 1 < dimensions.x && point.y - 1 >= 0) {
				map [(int)point.x + 1, (int)point.y - 1] = false;
			}
			if (point.x - 1 >= 0 && point.y + 1 < dimensions.y) {
				map [(int)point.x - 1, (int)point.y + 1] = false;
			}

			if (point.x - 1 >= 0 && point.y - 1 >= 0) {
				map [(int)point.x - 1, (int)point.y - 1] = false;
			}
			if (point.x + 1 < dimensions.x && point.y + 1 < dimensions.y) {
				map [(int)point.x + 1, (int)point.y + 1] = false;
			}
		}

		public bool [,] getMap(){
			return map;
		}

		private void examinePoint(Vector2 point){
			List<int> paths = new List<int>();
			paths.Add (0);
			paths.Add (1);
			paths.Add (2);
			paths.Add (3);
			paths.Sort((a, b)=> 1 - 2 * UnityEngine.Random.Range(0, 1));
			while(paths.Count > 0){
				switch(paths[0]){
				case 0:
					Right (point);
					break;
				case 1:
					Left (point);
					break;
				case 2:
					Up (point);
					break;
				case 3:
					Down (point);
					break;
				default:
					break;
				}
				paths.RemoveAt (0);
			}
		}

		private void Right(Vector2 point){
			if (point.x + 2 < dimensions.x && map[(int)point.x + 1, (int)point.y] && map[(int)point.x + 2, (int)point.y]) {
				map [(int)point.x + 1, (int)point.y] = false;
				map [(int)point.x + 2, (int)point.y] = false;
				toExamine.Add (new Vector2 (point.x + 2, point.y));
			}
		}

		private void Left(Vector2 point){
			if (point.x - 2 > -1 && map[(int)point.x - 1, (int)point.y] && map[(int)point.x - 2, (int)point.y]) {
				map [(int)point.x - 1, (int)point.y] = false;
				map [(int)point.x - 2, (int)point.y] = false;
				toExamine.Add (new Vector2 (point.x - 2, point.y));
			}
		}

		private void Down(Vector2 point){
			if (point.y - 2 > -1 && map[(int)point.x, (int)point.y - 1] && map[(int)point.x, (int)point.y - 2]) {
				map [(int)point.x, (int)point.y - 1] = false;
				map [(int)point.x, (int)point.y - 2] = false;
				toExamine.Add (new Vector2 (point.x, point.y - 2));
			}
		}

		private void Up(Vector2 point){
			if (point.y + 2 < dimensions.y && map[(int)point.x, (int)point.y + 1] && map[(int)point.x, (int)point.y + 2]) {
				map [(int)point.x, (int)point.y + 1] = false;
				map [(int)point.x, (int)point.y + 2] = false;
				toExamine.Add (new Vector2 (point.x, point.y + 2));
			}
		}
	}
}

