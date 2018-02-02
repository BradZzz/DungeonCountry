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

	private static List<Point3> destinations; 
	private int movingPosition;
	private int width, height;
	private Astar astar;
	private Point3 retreatPos;
	private BattleBoardManager boardManager;
	private BattleGameManager gameManager;
	private GeneralAttributes aiAttribs;
	private GeneralAttributes playerAttribs;

	public void init(Transform boardHolder, List<Transform> aiUnits, int width, int height, 
		BattleBoardManager boardManager, BattleGameManager gameManager, GeneralAttributes aiAttribs, GeneralAttributes playerAttribs){
		this.boardHolder = boardHolder;
		//These are all the objects the ai can interact with
		allUnits = new List<Transform>();
		//These are the units that we need to move
		this.aiUnits = aiUnits;
		this.height = height;
		this.width = width;
		this.boardManager = boardManager;
		this.gameManager = gameManager;
		this.aiAttribs = aiAttribs;
		this.playerAttribs = playerAttribs;

		astar = new Astar ();
		//These are the player units the ai will be attacking
		playersUnits = new List<Transform>();
		//These are the player units the ai will be attacking
		floor = new List<Transform>();

		retreatPos = new Point3 (this.width - 1, this.height / 2, 0);
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
		destinations = new List<Point3> ();

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
		//List <Transform> moveables = new List<Transform> ();
		//The places where we can move
		List <Transform> attackables = new List<Transform> ();
		//Obstacles players can interact with
		List <Transform> obstacles = new List<Transform> ();

		//The ai unit properties
		BattleMeta meta = ai.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			
		if (meta.getActions () > 0) {
			takeActions(ai, meta);
		}

		//Check to make sure that an enemy isn't already in the ai's range
		foreach (Transform unit in playersUnits) {
			//Find the list of tiles the robot can move to
			if (unit.gameObject.activeInHierarchy && Coroutines.checkRange(unit.position, ai.position, meta.range)) {
				attackables.Add (unit);
			}
		}

		//Check to make sure that an enemy isn't already in the ai's range
		foreach (Transform obs in boardManager.getObsPos()) {
			//Find the list of tiles the robot can move to
			if (obs.gameObject.activeInHierarchy && Coroutines.checkRange(obs.position, ai.position, meta.range)) {
				obstacles.Add (obs);
			}
		}

		//Sap
		if (meta.sap() && !meta.sapSpawn().Equals("") && meta.getAttacks() > 0 && obstacles.Count > 0) {
			
			meta.takeAttacks (1);
			obstacles[0].gameObject.SetActive (false);
			foreach (GameObject obj in gameManager.getGlossary().factions) {
				AffiliationMeta affiliation = obj.gameObject.GetComponent( typeof(AffiliationMeta) ) as AffiliationMeta;
				foreach (GameObject unit in affiliation.units) {
					if (unit.name.Equals(meta.sapSpawn())) {
						GameObject instance = Instantiate (unit, obstacles[0].position, Quaternion.identity) as GameObject;
						instance.SetActive (true);
						BattleMeta summon = instance.GetComponent( typeof(BattleMeta) ) as BattleMeta;
						summon.init ();
						summon.setPlayer (false);
						summon.setTurn (true);
						summon.setLives (meta.getLives());
						summon.setGeneralAttributes (aiAttribs);
						instance.transform.SetParent (boardHolder);
						boardManager.addUnit (instance.transform);
						aiUnits.Add (instance.transform);
					}
				}
			}
		//Regular attacks
		} else if (meta.getAttacks() > 0 && attackables.Count > 0) {
			if (meta.atkAll()) {
				foreach (Transform unit in attackables) {
					BattleMeta unitProp = unit.gameObject.GetComponent (typeof(BattleMeta)) as BattleMeta;
					meta.isAttacking (unitProp, true);
					unitProp.isAttacked (meta, playerAttribs, aiAttribs);
				}
				meta.takeAttacks (1);
			} else {
				BattleMeta weakest = null;
				//Have the ai attack the weakest unit
				foreach (Transform unit in attackables) {
					BattleMeta unitProp = unit.gameObject.GetComponent (typeof(BattleMeta)) as BattleMeta;
					if (weakest == null || weakest.getCurrentHP () > unitProp.getCurrentHP ()) {
						weakest = unitProp;
					}
				}
				meta.isAttacking (weakest, false);
				weakest.isAttacked (meta, playerAttribs, aiAttribs);
			}

			if (meta.getAttacks () > 0) {
				aiMoveSubroutine (ai);
			} else {
				meta.setTurn(false);
			}
		} 

		if (meta.getActions () == 0 && !(meta.getAttacks() > 0 && attackables.Count > 0)) {
			//The ai has no actions and no valid attacks. end turn
			meta.setTurn(false);
		}

		checkEndTurn ();
	}

	public void takeActions(Transform ai, BattleMeta meta){
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
				List <Point3> moveables = new List<Point3> ();

				//here we need to feed the possible tiles into the ai's choices
				foreach (Transform tile in floor) {
					//Find the list of tiles the ai can move to
					if (Coroutines.hasParent (tile) || destinations.Contains (new Point3(tile.position)) || new Point3(enemy.position).Equals(new Point3(tile.position))) {
						//Check to make sure that enemy isn't on top of the current tile
						//This is where the movables need to be set
						moveables.Add (new Point3(tile.position));
					}
				}
					
				//This tells the computer where the most direct path to the nearest player
				List<Point3> pathMap = astar.baseAlgorithm (new Point3 (ai.position), new Point3 (enemy.position), height, width, moveables, true);
				Point3 closest = new Point3 ();

				if (pathMap != null) {
					int distance = pathMap.Count - 1;
					if (distance < 0){
						closest = new Point3 (ai.position);
					//Now let's check the ai's range. If the range is bigger than the distance, we need to back up the ai
					} else if (meta.range > 1 && meta.range > distance + 1) {
						//Retreat to back lines
						pathMap = astar.baseAlgorithm (new Point3 (ai.position), retreatPos, height, width, moveables, true);

						try {
							closest = pathMap [0];
							int distance2 = pathMap.Count - 1;
							int extraRange = meta.range - (distance + 1);
							closest = extraRange > distance2 ? pathMap [distance2] : pathMap [extraRange];
						} catch (Exception e) {
							closest = new Point3 (ai.position);
						}

						/*if (distance - meta.range >= distance2 ) {
							closest = pathMap [distance2];
						} else {
							//else take the movement or distance - range. whichever is smaller
							int smaller = 1;
							closest = pathMap [smaller];
						}*/

					} else if (meta.movement > distance - meta.range) {
						if (distance - meta.range < 0) {
							closest = new Point3 (ai.position);
						} else {
							closest = pathMap [distance - meta.range];
						}
					} else {
						closest = pathMap [meta.movement];
					}
				} else {
					closest = new Point3 (ai.position);
				}
				moveCallback(closest, meta, enemy, ai);
			} else {
				meta.setTurn (false);
			} 
		}
	}

	public void  moveCallback(Point3 closest, BattleMeta meta, Transform enemy, Transform ai){
		destinations.Add (closest);
		meta.isMoving ();
		if (ai != null) {
			StartCoroutine (smooth_move (ai, closest.asVector3 (), 1f));
		}
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

	IEnumerator smooth_move(Transform origin, Vector3 direction, float speed){
		float startime = Time.time;
		Vector3 start_pos = new Vector3(origin.position.x, origin.position.y, origin.position.z);
		Vector3 end_pos = direction;
		while (origin.position != end_pos) { 
			//float move = Mathf.Lerp (0,1, (Time.time - startime) * speed);
			float move = .25f;

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

			if (((Time.time - startime) * speed) >= .75f) {
				origin.position = end_pos;
			}

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

	//This function returns the closest unit to the player
	Point3 GetClosest(Transform ai,List<Point3> enemies)
	{
		Point3 tMin = null;
		float minDist = Mathf.Infinity;
		Vector3 currentPos = ai.position;
		foreach (Point3 t in enemies)
		{
			//if (t.gameObject.activeInHierarchy) {
			float dist = Vector3.Distance(t.asVector3(), currentPos);
			if (dist < minDist)
			{
				tMin = t;
				minDist = dist;
			}
			//}
		}
		return tMin;
	}
}
