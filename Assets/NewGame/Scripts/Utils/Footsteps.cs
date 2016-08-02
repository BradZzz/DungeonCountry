using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class Footsteps : MonoBehaviour {

	public GameObject footprint;
	public GameObject dfootprint;

	private Queue<List<Vector3>> paths;
	private List<Vector3> foundVal;

	private Vector3 destination;
	private int columns, rows;
	private int[,] map;
	private List<GameObject> thisPath;

	void Awake(){
		thisPath = new List<GameObject> ();
	}

	public List<Vector3> generateMap(Vector3 startingPos, Vector3 destination, int rows, int columns, List<Vector3> obs){
		this.destination = destination;
		this.columns = columns;
		this.rows = rows;

		thisPath.Clear ();

		map = new int[columns,rows];
		for (int y = 0; y < rows; y++){
			for (int x = 0; x < columns; x++){
				Vector3 check = new Vector3 (x, y, 0);
				if ((startingPos == check || obs.Contains (check)) && !destination.Equals(check)) {
					map [x,y] = 1;
				} else {
					map [x,y] = 0;
				}
			}
		}
		foundVal = null;
		paths = new Queue<List<Vector3>>();
		paths.Enqueue (new List<Vector3> (){ startingPos });

		//Debug.Log ("Looking: " + destination.ToString());

		while(paths.Count > 0 && paths.Count < 1000){
			step (paths.Dequeue());
			if (foundVal != null) {
				break;
			}
		}

		if (paths.Count > 990) {
			Debug.Log ("Warning!!!");
			Debug.Log ("start: " + startingPos.ToString());
			Debug.Log ("end: " + destination.ToString());
		}
		if (foundVal != null) {
			foundVal.Remove (startingPos);
		}
		return foundVal;
	}

	public bool walking (){
		return thisPath.Count > 0;
	}

	public void createSteps(Vector3 start, Transform board, List<Vector3> steps){
		Vector3 rotation;
		Vector3 last = start;

		foreach (Vector3 tStep in steps){
			GameObject stepObj = footprint;
			if (steps[steps.Count - 1] == tStep) {
				stepObj = dfootprint;
			}

			GameObject instance = Instantiate (stepObj, tStep, Quaternion.identity) as GameObject;
			rotation = compareRotate (last, tStep);
			last = tStep;

			SpriteRenderer sprite = instance.GetComponent<SpriteRenderer> ();
			Debug.Log ("Rotating: " + rotation.ToString());
			sprite.transform.Rotate (rotation);
			//sprite.transform.Rotate (new Vector3(0, 0, 90));
			thisPath.Add (instance);
			instance.transform.SetParent (board);
		}

	}

	private Vector3 compareRotate(Vector3 start, Vector3 end){
		//Now we have to return the rotation;
		Vector3 difference = new Vector3(start.x - end.x, start.y - end.y, 0);
		Vector3 rotation = new Vector3(0, 0, 0);
		if (difference.x > 0) {
			rotation.z = 90;
		} else if (difference.x < 0) {
			rotation.z = 270;
		} else if (difference.y > 0) {
			rotation.z = 180;
		}
		return rotation;
	}

	public void destroySteps(){
		foreach (GameObject tStep in thisPath){
			Destroy (tStep);
		}
		thisPath.Clear ();
		foundVal = null;
	}

	public void printList(List<Vector3> path){
		thisPath.Clear ();
		foreach (Vector3 step in path) {
			Debug.Log ("Step: " + step.ToString());
		}
	}

	private void step(List<Vector3> step){
		Vector3 edge = step[step.Count - 1];
		//Debug.Log ("Working on: " +  edge.ToString() + " : " + step.Count);

		int[] directions = new int[]{ 1, 2, 3, 4 };
		Coroutines.ShuffleArray (directions);

		foreach (int way in directions) {
			switch(way){
				case 1:
					if (edge.x - 1 >= 0) {
						deepCopyPush (edge, step, new Vector3(-1,0,0));
					}
					break;
				case 2:
					if (edge.y - 1 >= 0) {
						deepCopyPush (edge, step, new Vector3(0,-1,0));
					}
					break;
				case 3:
					if (edge.x + 1 < columns) {
						deepCopyPush (edge, step, new Vector3(1,0,0));
					}
					break;
				case 4:
					if (edge.y + 1 < rows) {
						deepCopyPush (edge, step, new Vector3(0,1,0));
					}
					break;
			}
		}
	}

	private void deepCopyPush(Vector3 step, List<Vector3> path, Vector3 translation){
		Vector3 nextStep = new Vector3 (step.x + translation.x, step.y + translation.y, 0);
		if (map [(int)nextStep.x,(int)nextStep.y] == 0 && !path.Contains(nextStep)) {
			List<Vector3> newPath = new List<Vector3> (path);
			newPath.Add (nextStep);
			if (nextStep.Equals (destination)) {
				foundVal = newPath;
				paths.Clear ();
			} else {
				paths.Enqueue (newPath);
			}
		}
	}

}
