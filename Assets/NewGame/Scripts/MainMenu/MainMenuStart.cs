using UnityEngine;
using System.Collections;

public class MainMenuStart : MonoBehaviour {

	public void onClick(){
		Application.LoadLevel ("BattleScene");
	}

	public void onClickPanel(){
		Debug.Log ("BattleScene");

		BattleLoader game = GameObject.Find ("BattleCamera").GetComponent<BattleLoader>();
		game.getGameManager ().startGame();
	}
}
