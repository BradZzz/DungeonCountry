using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurePanel : MonoBehaviour {

	private GameObject GeneralPanel, ResourcePanel, ArmyPanel;

	// Use this for initialization
	void Start () {
		foreach (Transform child in transform)
		{
			if (child.name.Equals ("GeneralPanel")) {
				GeneralPanel = child.gameObject;
			}
			if (child.name.Equals ("ResourcePanel")) {
				ResourcePanel = child.gameObject;
			}
			if (child.name.Equals ("ArmyPanel")) {
				ArmyPanel = child.gameObject;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject unit in units) {
			BattleGeneralMeta bgm = unit.GetComponent<BattleGeneralMeta> ();
			if (bgm.getPlayer()) {

			}
		}
	}
}
