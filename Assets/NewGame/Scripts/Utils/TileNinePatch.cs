using UnityEngine;
using System.Collections;

public class TileNinePatch : MonoBehaviour {
	public GameObject topLeft;
	public GameObject topCenter;
	public GameObject topRight;

	public GameObject centerLeft;
	public GameObject center;
	public GameObject centerRight;

	public GameObject bottomLeft;
	public GameObject bottomCenter;
	public GameObject bottomRight;

	public GameObject sideWalls;

	/*void Start(){
		foreach(){

		}
	}*/

	/* -------------
	 * | 1 | 2 | 3 |
	 * | 4 | 5 | 6 |
	 * | 7 | 8 | 9 |
	 * |10 |11 |12 |
	 * -------------
	 */ 

	//Returns sprite from place number above. 10,11,12 are reserved for cliffs and building walls
	public GameObject returnPatch(int place){
		GameObject tile;
		SpriteRenderer sprite;

		switch(place){
			case 1:
				return topLeft;
			case 2:
				return topCenter;
			case 3:
				return topRight;
			case 4:
				return centerLeft;
			case 5:
				return center;
			case 6:
				return centerRight;
			case 7:
				return bottomLeft;
			case 8:
				return bottomCenter;
			case 9:
				return bottomRight;
			case 10:
				return sideWalls;
			case 11:
				return sideWalls;
			case 12:
				return sideWalls;
			default:
				return center;
		}
	}

}
