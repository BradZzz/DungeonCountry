using UnityEngine;
using System.Collections;

public class BattleLoader : MonoBehaviour {

	public GameObject battleGameManager;          //GameManager prefab to instantiate.

	void Awake ()
	{
		Camera cam = GetComponent<Camera>();
		cam.transparencySortMode = TransparencySortMode.Orthographic;
		if (BattleGameManager.instance == null) {
			Debug.Log ("Loading Game Manager");
			battleGameManager = Instantiate (battleGameManager);
		} else {
			Debug.Log ("Game Manager already instatiated");
		}
	}

	public BattleGameManager getGameManager(){
		return battleGameManager.GetComponent<BattleGameManager>();
	}
}
