using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuStart : MonoBehaviour {

	void Start(){
		Text t = (GameObject.Find ("Button").GetComponentsInChildren<Text> ())[0];
		t.text = "Confirm";
	}

	public void onClick(){
		Application.LoadLevel ("BattleScene");
	}

	//This is the panel click for the foreground panel
	//It behaves differently depending on if it was clicked n=1 and n=m times
	public void onClickPanel(){
		BattleLoader game = GameObject.Find ("BattleCamera").GetComponent<BattleLoader>();
		if (game.getGameManager ().getBoardSetup ().isSettingUp ()) {
			Debug.Log ("BattleSetup");

			//This is for when the units ares
			for (int i = 1; i <= 6; i++) {
				GameObject unit = GameObject.Find ("Unit" + i);
				if (unit != null) {
					unit.SetActive (false);
				}
			}

			Text t = (GameObject.Find ("Button").GetComponentsInChildren<Text> ()) [0];
			t.text = "End Turn";

			game.getGameManager ().startGame ();
		} else {
			Debug.Log ("BattleBoard");
			game.getGameManager ().endTurn ();
		}
	}
}
