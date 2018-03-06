using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class EntranceMeta : MonoBehaviour {

	public GameObject entranceInfo;
	public Sprite image;

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	public void addComponent(Component component){
		if (entranceInfo == null) {
			entranceInfo = new GameObject ("Entrance");
			DontDestroyOnLoad(entranceInfo);
			entranceInfo.tag = "Entrance";
		}
		Coroutines.CopyComponent (component,entranceInfo);
	}
}
