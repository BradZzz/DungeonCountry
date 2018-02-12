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
		foreach (Transform child in board) {
			BattleMeta bMeta = child.gameObject.GetComponent<BattleMeta> ();
			if (bMeta != null && bMeta.getPlayer()) {
				if (bMeta.getLives () > 0) {
					Debug.Log ("Player alive because of: " + bMeta.name);
					return false;
				}
			}
		}
		return true;
	}

	public bool theyLost(Transform board){
		foreach (Transform child in board) {
			BattleMeta bMeta = child.gameObject.GetComponent<BattleMeta> ();
			if (bMeta != null && !bMeta.getPlayer()) {
				if (bMeta.getLives () > 0) {
					Debug.Log ("AI alive because of: " + bMeta.name);
					return false;
				}
			}
		}
		return true;
	}

	public List <GameObject>[] getResults(Transform board){
		List <GameObject> playerArmy = new List <GameObject> (); 
		List <GameObject> aiArmy = new List <GameObject> (); 
		foreach (Transform child in board) {
			BattleMeta bMeta = child.gameObject.GetComponent<BattleMeta> ();
			if (bMeta != null && bMeta.getLives () > 0) {
				if (!bMeta.getPlayer ()) {
					aiArmy.Add (child.gameObject);
				} else {
					playerArmy.Add (child.gameObject);
				}
			}
		}
		return new List <GameObject>[]{ playerArmy, aiArmy };
	}
}
