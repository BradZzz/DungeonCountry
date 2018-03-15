using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LvlUpManager : MonoBehaviour {

	public GameObject[] upgrades;
	private LevelUpMeta meta;

	void Awake(){
		meta = new LevelUpMeta (new GeneralAttributes(), upgrades);
	}

	public void clickUpgrade(int panel){
		meta.selectStarters (panel);
	}
}
