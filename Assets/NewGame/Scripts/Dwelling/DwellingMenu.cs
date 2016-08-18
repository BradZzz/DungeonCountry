using UnityEngine;
using System.Collections;

public class DwellingMenu : MonoBehaviour {

	public void onClickAccept(){
		Debug.Log("Accept Clicked");
		foreach (GameObject unity in GameObject.FindGameObjectsWithTag("Unit")) {
			Debug.Log("Unit Name: " + unity.name);
			BattleGeneralMeta meta = unity.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
			if (meta != null) {
				Debug.Log("Unit Gold: " + meta.getResource("gold").getQuantity());
			}
		}
		Debug.Log("Player Name: " + SharedPrefs.getPlayerName());
		Debug.Log("Enemy Name: " + SharedPrefs.getEnemyName());
		GameObject player = GameObject.Find (SharedPrefs.getPlayerName());
		if (player != null) {
			BattleGeneralMeta meta = player.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
			if (meta.getResource("gold").spendResource(2000)) {
				Debug.Log ("Player Gold Now: " + meta.getResource("gold").getQuantity());
			} else {
				Debug.Log ("Cant afford, player Gold Now: " + meta.getResource("gold").getQuantity());
			}
		}
		Application.LoadLevel ("AdventureScene");
	}

	public void onClickDecline(){
		Application.LoadLevel ("AdventureScene");
	}
}
