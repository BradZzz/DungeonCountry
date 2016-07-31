using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	public float perspectiveMoveSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
	public float orthoMoveSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.


	void Update()
	{
		if (Input.GetKey (KeyCode.UpArrow)) {
			Debug.Log ("UpArrow key was pressed.");
			move (new Vector3(0,1,0));
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			Debug.Log ("DownArrow key was pressed.");
			move (new Vector3(0,-1,0));
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			Debug.Log ("LeftArrow key was pressed.");
			move (new Vector3(-1,0,0));
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			Debug.Log ("RightArrow key was pressed.");
			move (new Vector3(1,0,0));
		}
	}

	private void move(Vector3 direction){
		Camera cam = GetComponent<Camera>();
		Vector3 pos = cam.transform.position;
		cam.transform.position = new Vector3(direction.x + pos.x, direction.y + pos.y, pos.z);
	}
}
