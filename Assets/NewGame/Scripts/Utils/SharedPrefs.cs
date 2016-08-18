using UnityEngine;
using System.Collections;

public class SharedPrefs : MonoBehaviour {

	public static SharedPrefs Instance;

	private static gOName playerArmyInstance = null;
	private static gOName enemyArmyInstance = null;

	void Awake ()   
	{
		if (Instance == null)
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy (gameObject);
		}
	}

	public static string getPlayerName(){
		if (playerArmyInstance != null) {
			return playerArmyInstance.name;
		}
		return "";
	}

	public static void setPlayerName(string name){
		if (playerArmyInstance == null) {
			playerArmyInstance = new gOName ();
		}
		playerArmyInstance.name = name;
	}

	public static string getEnemyName(){
		if (enemyArmyInstance != null) {
			return enemyArmyInstance.name;
		}
		return "";
	}

	public static void setEnemyName(string name){
		if (enemyArmyInstance == null) {
			enemyArmyInstance = new gOName ();
		}
		enemyArmyInstance.name = name;
	}

	private class gOName{
		public string name;
		public gOName() {
			name = "";
		}
	}

	/*void Start () 
	{   
		playerArmy = SharedPrefs.Instance.playerArmy;
		enemyArmy = SharedPrefs.Instance.enemyArmy;
	}*/
}
