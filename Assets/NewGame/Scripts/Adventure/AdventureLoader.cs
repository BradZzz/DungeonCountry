using UnityEngine;
using System.Collections;

public class AdventureLoader : MonoBehaviour {

	public GameObject adventureGameManager;          //GameManager prefab to instantiate.

	void Awake ()
	{
		Camera cam = GetComponent<Camera>();
		cam.transparencySortMode = TransparencySortMode.Orthographic;
		if (AdventureGameManager.instance == null) {
			Debug.Log ("Loading Game Manager");
			adventureGameManager = Instantiate (adventureGameManager);
		} else {
			Debug.Log ("Game Manager already instatiated");

			AdventureGameManager manager = adventureGameManager.GetComponent( typeof(AdventureGameManager) ) as AdventureGameManager;
			manager.startAgain();
		}
	}

	public AdventureGameManager getGameManager(){
		return adventureGameManager.GetComponent<AdventureGameManager>();
	}
}
