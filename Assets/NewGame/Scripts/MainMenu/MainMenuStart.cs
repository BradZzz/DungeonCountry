using UnityEngine;
using System.Collections;

public class MainMenuStart : MonoBehaviour {

	public void onClick(){
		Debug.Log ("BattleScene");
		Application.LoadLevel ("BattleScene");
	}
}
