using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.


public class BattleArmyManager {

	/*
	 * What do I need here? 
	 * 1) GameObjects associated with the army
	 * 2) Controls that recognize when 
	 */

	private List<GameObject> myArmy;   
	private List<GameObject> theirArmy;   

	public BattleArmyManager(GameObject[] armyTiles){
		myArmy = new List<GameObject> ();
		theirArmy = new List<GameObject> ();

		foreach (GameObject unit in armyTiles) {
			BattleMeta meta = unit.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			if (meta != null) {
				if (meta.getPlayer()) {
					this.myArmy.Add (unit);
				} else {
					this.theirArmy.Add (unit);
				}
			}
		}
	}

	public BattleArmyManager(GameObject[] myArmy, GameObject[] theirArmy){
		this.myArmy = new List<GameObject> ();
		this.theirArmy = new List<GameObject> ();

		this.myArmy.AddRange (myArmy);
		this.theirArmy.AddRange(theirArmy);
	}

	public List<GameObject> getMyArmy(){
		return myArmy;
	}

	public List<GameObject> getTheirArmy(){
		return theirArmy;
	}

	public bool iLost(Transform board){
		foreach (GameObject unit in myArmy) {
			foreach (Transform child in board) {
				if (child.name.Contains(unit.name) && child.gameObject.activeInHierarchy) {
					return false;
				}
			}
		}
		return true;
	}

	public bool theyLost(Transform board){
		foreach (GameObject unit in theirArmy) {
			foreach (Transform child in board) {
				if (child.name.Contains(unit.name) && child.gameObject.activeInHierarchy) {
					return false;
				}
			}
		}
		return true;
	}
}
