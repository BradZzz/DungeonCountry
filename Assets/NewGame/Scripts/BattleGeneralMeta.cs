using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGeneralMeta : MonoBehaviour {

	public int tactics = 2;
	public List<GameObject> army;

	public BattleGeneralMeta(BattleGeneralMeta general){
		this.tactics = general.tactics;
		this.army = general.army;
	}

	void OnDisable(){
		Debug.Log ("OnDisable");
	}

	void OnDestroy(){
		Debug.Log ("OnDestroy");
	}
}
