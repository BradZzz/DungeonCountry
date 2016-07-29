using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Panel : MonoBehaviour {

	private Transform selectedChild;
	private List<Transform> children;
	private BattleLoader game;
	private BattleSetupManager board;

	void Start(){
		game = GameObject.Find ("BattleCamera").GetComponent<BattleLoader>();
		board = game.getGameManager ().getBoardSetup ();
		children = new List<Transform> ();
		foreach (Transform child in gameObject.transform) {
			children.Add (child);
		}
		selectedChild = children[0];
	}

	Transform myRayHit(Vector2 click){
		Debug.Log ("Looking: " + click.ToString());
		foreach (Transform child in gameObject.transform) {
			Rect rt = ((RectTransform) child).rect;

			Rect rect = new Rect (child.position.x - (rt.width/2), child.position.y - (rt.height/2), rt.width, rt.height);

			Debug.Log ("Found: " + child.transform + " pos: " + rect.position.ToString() + " dims: " + rect.width + ":" + rect.height);

			if (rect.Contains (Input.mousePosition) && !child.transform.name.Contains("Button")) {
				return child;
			}
		} 
		return null;
	}

	void Update(){
		if (Input.GetMouseButtonDown (0)) {
			Transform child = myRayHit (Input.mousePosition);
			if (child != null) {
				selectedChild = child;

				Debug.Log ("Clicked: " + child.name);

				Debug.Log ("Name: " + GameObject.Find ("BattleCamera").name);
				//Debug.Log ("Name: " + GameObject.Find ("Main Camera").name);

				game = GameObject.Find ("BattleCamera").GetComponent<BattleLoader>();

				game.getGameManager ().panelClicked (child.gameObject);
				//GetComponent<BattleGeneralMeta>()
				//game.getGameManager ().panelClicked (child.gameObject);
			}
		}
	}  

	public Transform getSelected(){
		return selectedChild;
	}

	public void removeSelected(){
		children.Remove(selectedChild);
		selectedChild = children[0];
	}
}
