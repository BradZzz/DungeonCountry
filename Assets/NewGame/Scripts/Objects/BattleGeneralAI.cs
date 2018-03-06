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
		List<Transform> castles = new List<Transform> ();
		List<Transform> resources = new List<Transform> ();
		List<Transform> rivals = new List<Transform> ();

		obstacles = new List<Point3> ();

		foreach (Transform child in board) {
			if (child.tag.Equals("Entrance")) {
				EntranceMeta eMeta = child.gameObject.GetComponent<EntranceMeta> ();
				GameObject info = eMeta.entranceInfo;
				CastleMeta castle = info.GetComponent<CastleMeta> ();
				if (castle != null) {
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
			if (child.tag.Equals("Unit")) {
				BattleGeneralMeta unit = child.GetComponent<BattleGeneralMeta> ();
				if (unit != null && unit.getPlayer()) {
					rivals.Add (child.transform);
					obstacles.Add (new Point3 (child.transform.position));
				}
			}
			if (child.tag.Equals("Obstacle")) {
				obstacles.Add (new Point3(child.transform.position));
			}
		}

		// First we need to figure out where we want our enemy hero to go
		decision = Random.Range(0, 3);
		List<Transform> potentialObjectives = new List<Transform> ();

		switch(decision){
			case 0:
				potentialObjectives = castles;
				break;
			case 1:
				potentialObjectives = resources;
				break;
			case 2:
				potentialObjectives = rivals;
				break;
		}

		// This is where we are heading to
		objective = GetClosest(ai.transform, potentialObjectives);
		// Now that we know where we are going, we need to figure out how to get there
		// StartCoroutine (steps.generateMapv2 (new Point3(lastClicked.position), click, gameManager.getRows (), gameManager.getColumns (), obstacles, setPath))
		// generateMapv2(Point3 startingPos, Point3 destination, int rows, int columns, List<Point3> obs, Action<List<Point3>, Point3> pathCallback)
		// steps.generateMapv2(new Point3(ai.transform.position), new Point3(objective.transform.position), rows, columns, obstacles, getPath);
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
