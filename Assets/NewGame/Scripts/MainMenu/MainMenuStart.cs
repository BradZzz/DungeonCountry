using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuStart : MonoBehaviour {

	GameObject button;

	void Start(){
		button = GameObject.Find ("Button");
		Text t = (button.GetComponentsInChildren<Text> ())[0];
		t.text = "Start";
	}

	public void onClickStart(){
		Debug.Log ("Clicked!");
		Application.LoadLevel ("CharacterSelect");
	}

	public void onClickFaction(int faction){
		Debug.Log ("Clicked!");
		SharedPrefs.setPlayerFaction (faction);
		Application.LoadLevel ("AdventureScene");
	}

	public void onClickPuzzle(){
		Application.LoadLevel ("PuzzleScene");
	}

	public void onClickBack(){
		Application.LoadLevel ("MainMenu");
	}

	//This is the panel click for the foreground panel
	//It behaves differently depending on if it was clicked n=1 and n=m times
	public void onClickPanel(){
		Debug.Log ("Clicked!");
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

			GameObject button = GameObject.Find ("Button");
			Text t = (button.GetComponentsInChildren<Text> ()) [0];
			t.text = "End Turn";

			game.getGameManager ().startGame ();

			GameObject.Find ("Panel").gameObject.SetActive (false);
		} else {
			Debug.Log ("BattleBoard");
			game.getGameManager ().endTurn ();
		}
	}
}
