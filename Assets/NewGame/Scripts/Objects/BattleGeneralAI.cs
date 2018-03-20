using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class BattleGeneralAI {
	private GameObject ai;
	private Transform objective;
	private int decision;
	private List<Point3> obstacles;

	public BattleGeneralAI (GameObject ai) {
		this.ai = ai;
	}

	public Transform getObjective(){
		return objective;
	}

	public List<Point3> getObstacles(){
		return obstacles;
	}

	/*
	 * TODO:
	 * Take in general. Figure out what he's going to do:
	 * 1) Gather resources
	 * 2) Visit castle
	 * 3) Attack rival
	 */

	public void moveGeneral(Transform board){
		BattleGeneralMeta aiMeta = ai.GetComponent<BattleGeneralMeta> ();

		List<Transform> castles = new List<Transform> ();
		List<Transform> resources = new List<Transform> ();
		List<Transform> rivals = new List<Transform> ();

		obstacles = new List<Point3> ();

		foreach (Transform child in board) {
			if (child.tag.Equals("Unit")) {
				BattleGeneralMeta unit = child.GetComponent<BattleGeneralMeta> ();
				if (unit != null && unit.getPlayer()) {
					if (unit.getPlayer () || !unit.faction.Equals("Neutral")) {
						rivals.Add (child.transform);
						obstacles.Add (new Point3 (child.transform.position));
					} else {
						obstacles.Add (new Point3 (child.transform.position));
					}
				}
			}
		}

		foreach (Transform child in board) {
			//Check to make sure that another unit isn't over the entrance here
			if (child.tag.Equals("Entrance")) {
				EntranceMeta eMeta = child.gameObject.GetComponent<EntranceMeta> ();
				GameObject info = eMeta.entranceInfo;
				CastleMeta castle = info.GetComponent<CastleMeta> ();
				if (castle != null && !checkUnitOn(rivals, new Point3(child.position))) {
					Debug.Log("Castle at: " + child.transform.position.ToString());
					castles.Add (child.transform);
					obstacles.Add (new Point3 (child.transform.position));
				}
			}
			if (child.tag.Equals("Resource")) {
				ResourceMeta resource = child.GetComponent<ResourceMeta> ();
				if (resource != null) {
					resources.Add (child.transform);
				}
			}
			if (child.tag.Equals("Obstacle")) {
				obstacles.Add (new Point3(child.transform.position));
			}
		}

		// First we need to figure out where we want our enemy hero to go
		List<Transform> potentialObjectives = new List<Transform> ();

		/*
		 * If the ai isn't at least twice as strong as the player, pick 0 or 1
		 * If the ai doesn't have enough resources, pick 1
		 * If the ai has enough resources but isn't strong enough, pick 0
		 */ 

		int aiArmyScore = getArmyScore(ai.GetComponent<BattleGeneralMeta>());

//		int playerArmyScore = getArmyScore(rivals [0].GetComponent<BattleGeneralMeta>());
//		bool armyStronger = (aiArmyScore / 1.5) > playerArmyScore;

		List<Transform> weakRivals = new List<Transform> ();
		foreach (Transform rival in rivals) {
//			int armyScore = getArmyScore(rival.GetComponent<BattleGeneralMeta>());
//			if ((aiArmyScore / 1.25) >= armyScore) {
//				weakRivals.Add (rival);
//			}
			if (!rival.GetComponent<BattleGeneralMeta>().getPlayer()) {
				weakRivals.Add (rival);
			}
		}
		bool needResources = checkResources (ai.GetComponent<BattleGeneralMeta> ());

		if (weakRivals.Count > 0 || aiMeta.faction.Equals("Neutral")) {
			Debug.Log ("Decision: Attack Player");
			if (aiMeta.faction.Equals("Neutral")) {
				potentialObjectives = rivals;
			} else {
				potentialObjectives = weakRivals;
			}
		} else {
			if (needResources) {
				Debug.Log ("Decision: Collect Resources");
				potentialObjectives = resources;
			} else {
				Debug.Log ("Decision: Visit Castle");
				potentialObjectives = castles;
			}
		}

		// This is where we are heading to
		objective = GetClosest(ai.transform, potentialObjectives);
	}

	private int getArmyScore(BattleGeneralMeta unit){
		return ScoreConverter.computeResults (unit.getArmy());
	}

	private bool checkResources(BattleGeneralMeta unit){
		BattleGeneralResources res = unit.getResources ();
		foreach(KeyValuePair<string, int> item in res.getResources())
		{
			// do something with entry.Value or entry.Key
			if (item.Key.Equals("gold")) {
				if (item.Value < 2500) {
					return true;
				}
			}
		}
		return false;
	}

	private bool checkUnitOn(List<Transform> rivals, Point3 point){
		foreach(Transform rival in rivals){
			if (point.Equals (new Point3(rival.position))) {
				return true;
			}
		}
		return false;
	}

	private Transform GetClosest(Transform ai,List<Transform> objectives)
	{
		Transform tMin = null;
		float minDist = Mathf.Infinity;
		Vector3 currentPos = ai.position;
		foreach (Transform t in objectives)
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
