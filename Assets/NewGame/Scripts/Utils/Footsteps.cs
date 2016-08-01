using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Footsteps : MonoBehaviour {

	public GameObject footprint;

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

		map = new int[columns,rows];
		for (int y = 0; y < rows; y++){
			for (int x = 0; x < columns; x++){
				Vector3 check = new Vector3 (x, y, 0);
				if (startingPos == check || obs.Contains (check)) {
					map [x,y] = 1;
				} else {
					map [x,y] = 0;
				}
			}
		}
		foundVal = null;
		paths = new Queue<List<Vector3>>();
		paths.Enqueue (new List<Vector3> (){ startingPos });

		Debug.Log ("Looking: " + destination.ToString());

		while(paths.Count > 0 && paths.Count < 200){
			step (paths.Dequeue());
			if (foundVal != null) {
				break;
			}
		}
		foundVal.Remove (startingPos);
		return foundVal;
	}

	public bool walking (){
		return thisPath.Count > 0;
	}

	public void createSteps(Transform board, List<Vector3> steps){
		foreach (Vector3 tStep in steps){
			GameObject instance = Instantiate (footprint, tStep, Quaternion.identity) as GameObject;
			thisPath.Add (instance);
			instance.transform.SetParent (board);
		}

	}

	public void destroySteps(){
		foreach (GameObject tStep in thisPath){
			Destroy (tStep);
		}
		thisPath.Clear ();
	}

	public void printList(List<Vector3> path){
		thisPath.Clear ();
		foreach (Vector3 step in path) {
			Debug.Log ("Step: " + step.ToString());
		}
	}

	private void step(List<Vector3> step){
		Vector3 edge = step[step.Count - 1];
		Debug.Log ("Working on: " +  edge.ToString() + " : " + step.Count);

		if (edge.x - 1 >= 0) {
			deepCopyPush (edge, step, new Vector3(-1,0,0));
		}
		if (edge.y - 1 >= 0) {
			deepCopyPush (edge, step, new Vector3(0,-1,0));
		}
		if (edge.x + 1 < columns) {
			deepCopyPush (edge, step, new Vector3(1,0,0));
		}
		if (edge.y + 1 < rows) {
			deepCopyPush (edge, step, new Vector3(0,1,0));
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
