using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using AssemblyCSharp;

public class BattleAI : MonoBehaviour {

	private Transform boardHolder;
	private Transform board; 
	private List<Transform> allUnits;
	private List<Transform> aiUnits;
	//unit. lol.
	private static List<Transform> playersUnits; 
	private List<Transform> floor; 
	private static aiUnit aiMoving;

	private static List<Transform> destinations; 
	/*
	 * Enemy actions:
	 * 
	 * 	if player is in attack range of enemy:
	 * 		attack enemy, then move away from enemy
	 *  if player is not in range of enemy:
	 * 		move towards nearest enemy and try to attack
	*/

	private class aiUnit {
		
		int movingAI;

		public aiUnit(int ai){
			movingAI = ai;
		}

		public bool done(){
			movingAI--;
			return movingAI <= 0;
		}
	}

	public void init(Transform boardHolder, List<Transform> aiUnits){

		this.boardHolder = boardHolder;
		//These are all the objects the ai can interact with
		allUnits = new List<Transform>();
		//These are the units that we need to move
		this.aiUnits = aiUnits;
		//These are the player units the ai will be attacking
		playersUnits = new List<Transform>();
		//These are the player units the ai will be attacking
		floor = new List<Transform>();

		foreach (Transform tile in boardHolder){
			//Organize the units into buckets
			if (tile.tag.Contains ("Unit")) {
				if (!this.aiUnits.Contains (tile)) {
					playersUnits.Add (tile);
				}
				allUnits.Add (tile);
			} else {
				floor.Add (tile);
			}
		}

		aiMoving = new aiUnit (aiUnits.Count);
	}

	public void moveUnits(){
		destinations = new List<Transform> ();
		foreach(Transform ai in aiUnits){
			StartCoroutine (aiMove (ai));
		}
	}

	/*
	 * Check to see if a unit is within range of the user
	 * 	Yes:
	 * 		Attack User
	 *  No:
	 * 		Move towards user
	 *
	 */

	IEnumerator aiMove(Transform ai){
		//The places where we can move
		List <Transform> moveables = new List<Transform> ();
		//The places where we can move
		List <Transform> attackables = new List<Transform> ();
		//The ai unit properties
		BattleMeta meta = ai.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;

		int atks = meta.getAttacks ();
		int actions = meta.getActions ();

		//Check to make sure that an enemy isn't already in the ai's range
		foreach (Transform unit in playersUnits) {
			//Find the list of tiles the robot can move to
			if (unit.gameObject.activeInHierarchy && Coroutines.checkRange(unit.position, ai.position, meta.range)) {
				attackables.Add (unit);
			}
		}

		Debug.Log ("Moving: " + ai.name + " atk: " + atks + " actns: " + actions + " attackables: " + attackables.Count);

		if (attackables.Count > 0) {
			if (atks > 0) {
				Debug.Log ("Attacking...");
				BattleMeta weakest = null;
				//Have the ai attack the weakest unit
				foreach (Transform unit in attackables) {
					BattleMeta unitProp = unit.gameObject.GetComponent (typeof(BattleMeta)) as BattleMeta;
					if (weakest == null || weakest.getCurrentHP () > unitProp.getCurrentHP ()) {
						weakest = unitProp;
					}
				}
				meta.isAttacking (weakest);
				weakest.isAttacked (meta.attack);
				yield return null;
				aiMove(ai);
			} else {
				Debug.Log ("No Attacks. Ending Turn");
				//End the turn here, since the enemy could attack the player, just ran out of attacks

				/*
				 * TODO: Back up enemies here with attack range > 1
				 */ 

				meta.setTurn(false);
			}
		} else {
			if (actions > 0) {
				Debug.Log ("Moving");
				//move ai and repeat function
				//Get the closest enemy
				Transform enemy = GetClosest (ai, playersUnits);
				foreach (Transform tile in floor) {
					//Find the list of tiles the robot can move to
					if (Coroutines.checkRange(tile.position, ai.position, meta.movement) && !hasParent(tile) && !destinations.Contains(tile)) {
						moveables.Add (tile);
					}
				}
				Transform closest = GetClosest (enemy, moveables);
				destinations.Add (closest);
				meta.isMoving ();
				StartCoroutine (smooth_move (ai, closest.position, 1f));
			} else {
				Debug.Log ("No Actions ending");
				//The ai has no actions and no attacks. end turn
				meta.setTurn(false);
			}
		}
		yield return null;
	}

	/*public static void pAttack(Transform ai){
		Debug.Log ("pAttack");
		BattleMeta meta = ai.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;

		//Race conditions :(
		int atks = meta.getAttacks ();
		foreach (Transform tile in playersUnits) {
			if (Coroutines.checkRange(tile.position, ai.position, meta.range)) {
				while (atks > 0){
					Debug.Log (meta.name + " Attacking... " + meta.getAttacks());
					BattleMeta player = tile.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
					meta.isAttacking (player);
					if (!player.isAttacked (meta.attack)) {
						break;
					}
					atks--;
				}
			}
		}
	}*/

	public bool hasParent(Transform child){
		foreach (GameObject children in GameObject.FindGameObjectsWithTag("Unit")) {
			if (children.transform.position.x == child.position.x && children.transform.position.y == child.position.y) {
				return true;
			}
		}
		return false;
	}

	IEnumerator smooth_move(Transform origin, Vector3 direction, float speed){
		float startime = Time.time;
		Vector3 start_pos = new Vector3(origin.position.x, origin.position.y, origin.position.z);
		Vector3 end_pos = direction;
		while (origin.position != end_pos) { 
			float move = Mathf.Lerp (0,1, (Time.time - startime) * speed);

			Vector3 position = origin.position;

			position.x += (end_pos.x - start_pos.x) * move;
			position.y += (end_pos.y - start_pos.y) * move;

			if (start_pos.x > end_pos.x && origin.position.x < end_pos.x) {
				position.x = end_pos.x;
			}

			if (start_pos.x < end_pos.x && origin.position.x > end_pos.x) {
				position.x = end_pos.x;
			}

			if (start_pos.y > end_pos.y && origin.position.y < end_pos.y) {
				position.y = end_pos.y;
			}

			if (start_pos.y < end_pos.y && origin.position.y > end_pos.y) {
				position.y = end_pos.y;
			}

			origin.position = position;
			yield return null;
		}
		Debug.Log ("Done Moving!");
		aiMove (origin);
	}

	/*public static IEnumerator delay(float delay, BattleBoardManager board)
	{
		yield return new WaitForSeconds(delay);
		endTurn (board);
	}

	public static void endTurn(BattleBoardManager board){
		board.activateUnits (true);
	}*/

	/*public static void moveAI(Transform ai, Vector3 finish){		
		StartCoroutine (smooth_move (ai, finish, 1f));
	}*/

	//This function returns the closest unit to the player
	Transform GetClosest(Transform ai,List<Transform> enemies)
	{
		Transform tMin = null;
		float minDist = Mathf.Infinity;
		Vector3 currentPos = ai.position;
		foreach (Transform t in enemies)
		{
			if (t.gameObject.activeInHierarchy) {
				float dist = Vector3.Distance(t.position, currentPos);
				if (dist < minDist)
				{
					tMin = t;
					minDist = dist;
				}
			}
		}
		return tMin;
	}
}
