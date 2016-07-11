using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

	public GameObject gameManager;          //GameManager prefab to instantiate.
	public GameObject soundManager;         //SoundManager prefab to instantiate.


	void Awake ()
	{
		//Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
		if (GameManager.instance == null) {
			Debug.Log ("Loading Game Manager");
			Instantiate (gameManager);
		} else {
			Debug.Log ("Game Manager already instatiated");
		}
		//Check if a SoundManager has already been assigned to static variable GameManager.instance or if it's still null
		if (SoundManager.instance == null) {
			Debug.Log ("Loading Sound Manager");
			Instantiate (soundManager);
		} else {
			Debug.Log ("Sound Manager already instatiated");
		}
	}
}
