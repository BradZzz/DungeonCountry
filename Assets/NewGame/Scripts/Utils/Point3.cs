using UnityEngine;
using System.Collections;
using System;

public class Point3 {

	public int x, y, z;

	public Point3 () {
		this.x = 0;
		this.y = 0;
		this.z = 0;
	}

	public Point3 (int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Point3 (Point3 point) {
		this.x = point.x;
		this.y = point.y;
		this.z = point.z;
	}

	//So fucking stupid I have to write this class... Do your job C#
	public Point3 (Vector3 point) {
		this.x = (int) Math.Round(point.x);
		this.y = (int) Math.Round(point.y);
		this.z = (int) Math.Round(point.z);
	}

	public Point3 (float x, float y, float z) {
		this.x = (int) Math.Round(x);
		this.y = (int) Math.Round(y);
		this.z = (int) Math.Round(z);
	}

	public bool Equals (Point3 point) {
		if (point == null) {
			return false;
		}
		return this.x == point.x && this.y == point.y && this.z == point.z;
	}

	public bool Equals (Vector3 point) {
		return this.x == (int) Math.Round(point.x) && this.y == (int) Math.Round(point.y) && this.z == (int) Math.Round(point.z);
	}

	public Vector3 asVector3(){
		return new Vector3 (x,y,z);
	}

	public string ToString(){
		return "(" + x + "," + y + "," + z + ")";
	}

	public bool awayFromEdge(int[,] map){
		return x > 0 && y > 0 && x < map.GetLength (0) - 1 && y < map.GetLength (1) - 1;
	}

	public bool crowded(int[,] map, int dismiss, int error){
		for (int y = this.y - 1; y < this.y + 2; y++) {
			for (int x = this.x - 1; x < this.x + 2; x++) {
				if (map[x,y] != dismiss) {
					error--;
				}
			}
		}
		return error > 0;
	}

	public int returnPatchLocation(int[,] map, int looking) {

		int[,] directions = new int[3, 3];

		for (int y = 0; y < 3; y++){
			for (int x = 0; x < 3; x++){
				directions [x, y] = 0;
			}
		}

		for (int y = this.y - 1, yy = 0; y < this.y + 2; y++, yy++) {
			for (int x = this.x - 1, xx = 0; x < this.x + 2; x++, xx++) {
				try {
					if (map[x,y] == looking) {
						directions[xx,yy] = 1;
					}
				} catch (Exception e) {
					directions[xx,yy] = -1;
				}
			}
		}

		int leftI = directions[0,1];
		int rightI = directions[2,1];
		int upI = directions [1,0];
		int downI = directions[1,2];

		bool left = leftI == -1 || leftI == 1;
		bool right = rightI == -1 || rightI == 1;
		bool up = upI == -1 || upI == 1;
		bool down = downI == -1 || downI == 1;

		//topleft
		if (right && down) {
			return 1;
		}

		//topcenter
		if (left && right && down) {
			return 2;
		}

		//topright
		if (right && down) {
			return 3;
		}

		//midleft
		if (right && up && down) {
			return 4;
		}

		//midcenter
		if (left && right && up && down) {
			return 5;
		}

		//midright
		if (left && up && down) {
			return 6;
		}

		//bottomleft
		if (right && up) {
			return 7;
		}

		//bottomcenter
		if (left && right && up) {
			return 8;
		}

		//bottomright
		if (left && up) {
			return 9;
		}
		return 5;
	}
}