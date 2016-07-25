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
			Transform enemy = GetClosest (ai, playersUnits);
			List <Transform> moveables = new List<Transform> ();
			BattleMeta meta = ai.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			foreach (Transform tile in floor) {
				//Find the list of tiles the robot can move to
				if (Coroutines.checkRange(tile.position, ai.position, meta.range) && !hasParent(tile) && !destinations.Contains(tile)) {
					moveables.Add (tile);
				}
			}

			//Now we take the tiles and return the tile closest to the player's units
			Transform closest = GetClosest (enemy, moveables);
			destinations.Add (closest);

			if (closest == null) {
				Debug.Log ("closest = null");
			}

			meta.isMoving ();
			//moveAI (ai, closest.position);
			Debug.Log ("AI: " + ai.name + " position: " + closest.position);
			StartCoroutine (smooth_move (ai, closest.position, 1f));
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

	public static void pAttack(Transform ai){
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
		/*if (aiMoving.done()) {
			StartCoroutine (delay (2, boardHolder));
		}*/
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
		pAttack (origin);
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
