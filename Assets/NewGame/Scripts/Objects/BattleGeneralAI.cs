using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class BattleGeneralAI {
	private GameObject ai;
	private Transform objective;
	private Stack<Transform> altObjectives;
	private int decision;
	private List<Point3> obstacles;
	private int turn;

	public BattleGeneralAI (GameObject ai, int turn) {
		this.ai = ai;
		this.turn = turn;
		Debug.Log ("Turn: " + turn.ToString());
	}

	public Transform getObjective(){
		if (objective != null) {
			return objective;
		} else {
			return getAltObjective ();
		}
	}

	public List<Point3> getObstacles(){
		return obstacles;
	}

	public Transform getAltObjective(){
		if (altObjectives.Count > 0) {
			return altObjectives.Pop();
		} else {
			return null;
		}
	}

//	public void popAltObjective(){
//		altObjectives.RemoveAt (0);
//	}

	/*
	 * TODO:
	 * Take in general. Figure out what he's going to do:
	 * 1) Gather resources
	 * 2) Visit castle
	 * 3) Attack rival
	 */

	public void moveGeneral(Transform board){
		altObjectives = new Stack<Transform> ();
		BattleGeneralMeta aiMeta = ai.GetComponent<BattleGeneralMeta> ();

		List<Transform> castles = new List<Transform> ();
		List<Transform> resources = new List<Transform> ();
		List<Transform> rivals = new List<Transform> ();

		obstacles = new List<Point3> ();

		foreach (Transform child in board) {
			if (child.tag.Equals("Unit") && child.gameObject.activeInHierarchy) {
				BattleGeneralMeta unit = child.GetComponent<BattleGeneralMeta> ();
				bool notSamePos = child.position.x != ai.transform.position.x && child.position.y != ai.transform.position.y;
				if (unit != null && notSamePos) {
//					rivals.Add (child.transform);
//					obstacles.Add (new Point3 (child.transform.position));
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
			if (child.gameObject.activeInHierarchy && child.position.x != ai.transform.position.x && child.position.y != ai.transform.position.y) {
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
		Debug.Log ("Chosen General: " + aiMeta.name + " score: " + aiArmyScore.ToString());

		List<Transform> weakRivals = new List<Transform> ();
		foreach (Transform rival in rivals) {
			int armyScore = getArmyScore(rival.GetComponent<BattleGeneralMeta>());
			Color thisBanner = ai.GetComponent<BattleGeneralMeta> ().getBanner ();
			Color rivalBanner = rival.GetComponent<BattleGeneralMeta> ().getBanner ();
			if ((aiArmyScore / 2.75) >= armyScore && turn > 5 && !rival.GetComponent<BattleGeneralMeta>().faction.Equals("Neutral") && thisBanner != rivalBanner) {
				Debug.Log ("Weak Rival General: " + rival.GetComponent<BattleGeneralMeta>().name + " score: " + armyScore.ToString());
				weakRivals.Add (rival);
			}
		}
		bool needResources = checkResources (ai.GetComponent<BattleGeneralMeta> ());
		int choice = 1;

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
				choice = 2;
			} else {
				Debug.Log ("Decision: Visit Castle");
				potentialObjectives = castles;
				choice = 3;
			}
		}

		//Were we are going
		objective = GetClosest(ai.transform, potentialObjectives);

		switch(choice) {
			case 1:
				foreach (Transform obs in castles) {
					altObjectives.Push (obs);
				}
				foreach (Transform obs in resources) {
					altObjectives.Push (obs);
				}
				foreach (Transform obs in potentialObjectives) {
					altObjectives.Push (obs);
				}
				break;
			default:
				foreach (Transform obs in rivals) {
					altObjectives.Push (obs);
				}
				foreach (Transform obs in castles) {
					altObjectives.Push (obs);
				}
				foreach (Transform obs in resources) {
					altObjectives.Push (obs);
				}
				break;
		}
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
