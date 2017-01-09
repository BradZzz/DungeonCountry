using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LvlUpManager : MonoBehaviour {

	private LevelUpMeta meta;

	void Awake(){
		meta = new LevelUpMeta (new GeneralAttributes());
	}

	public void clickUpgrade(int panel){
		meta.selectStarters (panel);
	}
}
