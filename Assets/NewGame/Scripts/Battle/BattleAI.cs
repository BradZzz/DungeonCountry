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

	private static List<Transform> destinations; 
	private int movingPosition;

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
	}

	public delegate void EndTurnCallback();
	static EndTurnCallback endTurnCallback;

	public void moveUnits(EndTurnCallback callback){
		endTurnCallback = callback;
		destinations = new List<Transform> ();

		movingPosition = 0;
		if (aiUnits.Count > 0) {
			StartCoroutine (aiMove (aiUnits [movingPosition]));
		} else {
			endTurnCallback ();
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

	public void iterateAIUnit(){
		movingPosition++;
		if (movingPosition < aiUnits.Count) {
			Debug.Log ("Moving Position Before: " + movingPosition);
			while (movingPosition < aiUnits.Count && !aiUnits [movingPosition].gameObject.activeInHierarchy) {
				movingPosition++;
			}
			Debug.Log ("Moving Position After: " + movingPosition);
			if (movingPosition < aiUnits.Count) {
				StartCoroutine (aiMove (aiUnits [movingPosition]));
			} else {
				endTurnCallback ();
			}
		} else {
			endTurnCallback ();
		}
	}

	IEnumerator aiMove(Transform ai){
		yield return new WaitForSeconds (0.5f);

		aiMoveSubroutine(ai);
		iterateAIUnit ();

		yield return null;
	}

	private void aiMoveSubroutine(Transform ai){

		//The places where we can move
		List <Transform> moveables = new List<Transform> ();
		//The places where we can move
		List <Transform> attackables = new List<Transform> ();
		//The ai unit properties
		BattleMeta meta = ai.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;

		//Check to make sure that an enemy isn't already in the ai's range
		foreach (Transform unit in playersUnits) {
			//Find the list of tiles the robot can move to
			if (unit.gameObject.activeInHierarchy && Coroutines.checkRange(unit.position, ai.position, meta.range)) {
				attackables.Add (unit);
			}
		}

		if (attackables.Count > 0) {
			if (meta.getAttacks() > 0) {
				BattleMeta weakest = null;
				//Have the ai attack the weakest unit
				foreach (Transform unit in attackables) {
					BattleMeta unitProp = unit.gameObject.GetComponent (typeof(BattleMeta)) as BattleMeta;
					if (weakest == null || weakest.getCurrentHP () > unitProp.getCurrentHP ()) {
						weakest = unitProp;
					}
				}
				//if (meta.getTurn()) {
				meta.isAttacking (weakest);
				weakest.isAttacked (meta.attack);
				//}

				if (meta.getAttacks() > 0) {
					aiMoveSubroutine (ai);
				}
				meta.setTurn(false);
			} else {
				//End the turn here, since the enemy could attack the player, just ran out of attacks

				/*
				 * TODO: Back up enemies here with attack range > 1
				 */ 

				meta.setTurn(false);
			}
		} else {
			if (meta.getActions () > 0) {
				Debug.Log ("Moving");
				//move ai and repeat function
				//Get the closest enemy
				Transform enemy = GetClosest (ai, playersUnits);
				//There are not more player units left to fight. end game...
				if (enemy == null) {
					endTurnCallback ();
				} else {
					Vector2 ePos = enemy.position;
					if (enemy != null) {
						foreach (Transform tile in floor) {
							//Find the list of tiles the robot can move to
							if (Coroutines.checkRange (tile.position, ai.position, meta.movement) && !hasParent (tile) && !destinations.Contains (tile)) {
								//Check to make sure that enemy isn't on top of the current tile
								moveables.Add (tile);
							}
						}
						Transform closest = GetClosest (enemy, moveables);
						if (meta.range > 1) {
							List<Transform> enemyRadius = new List<Transform> ();
							foreach (Transform tile in floor) {
								//Find the list of tiles the robot can move to
								if (Coroutines.checkRange (tile.position, enemy.position, meta.range) && !hasParent (tile) && !destinations.Contains (tile)) {
									//Check to make sure that enemy isn't on top of the current tile
									enemyRadius.Add (tile);
								}
							}
							List<Transform> overlap = new List<Transform> ();
							//So now we have the enemy radius. We need to find if the moveables and the enemyRadius overlap
							foreach (Transform movePosition in enemyRadius) {
								if (moveables.Contains (movePosition)) {
									overlap.Add (movePosition);
								}
							}
							//If they do, we want to take the closest overlap to the ai unit
							if (overlap.Count > 0) {
								closest = GetClosest (ai, overlap);
							}
						}
						destinations.Add (closest);
						meta.isMoving ();
						StartCoroutine (smooth_move (ai, closest.position, 1f));
					} else {
						meta.setTurn (false);
					} 
				}
			} else {
				//The ai has no actions and no attacks. end turn
				meta.setTurn(false);
			}
		}
		checkEndTurn ();
	}

	private void checkEndTurn(){
		bool activeUnits = false;
		int atks, actions;
		foreach(Transform unit in aiUnits){
			BattleMeta unitProp = unit.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			//Check to make sure the enemy is active and the enemy is available in the heirarchy
			atks = unitProp.getAttacks ();
			actions = unitProp.getActions ();
			activeUnits = activeUnits || unitProp.getTurn();
		}
	}

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
		aiMoveSubroutine (origin);
	}

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
