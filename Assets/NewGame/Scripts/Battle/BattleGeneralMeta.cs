using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGeneralMeta : MonoBehaviour {

	public int tactics = 2;
	public string name = "none";
	public string description = "none";
	public List<GameObject> army;

	public BattleGeneralMeta(BattleGeneralMeta general){
		this.tactics = general.tactics;
		this.army = general.army;
	}
}
