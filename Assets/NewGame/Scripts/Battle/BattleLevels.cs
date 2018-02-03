using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLevels : MonoBehaviour {
	public string name;
	public enum lvltype 
	{
		Cave, Dungeon, Magical, Palace, World
	};
	public lvltype lvl;
	public GameObject[] outerWallTiles;
	public GameObject[] innerWallTiles;
	public GameObject[] floorTiles;
	public GameObject[] sludgeTiles;
	public GameObject[] lavaTiles;
}
