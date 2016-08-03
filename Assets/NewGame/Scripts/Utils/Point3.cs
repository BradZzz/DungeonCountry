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
		if (point == null) {
			return false;
		}
		return this.x == (int) Math.Round(point.x) && this.y == (int) Math.Round(point.y) && this.z == (int) Math.Round(point.z);
	}

	public Vector3 asVector3(){
		return new Vector3 (x,y,z);
	}

	public string ToString(){
		return "(" + x + "," + y + "," + z + ")";
	}
}
