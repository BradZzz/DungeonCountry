using UnityEngine;
using System.Collections;

public class SharedPrefs : MonoBehaviour {

	public static SharedPrefs Instance;
	public static GameObject playerArmy;
	public static GameObject enemyArmy;

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

	/*void Start () 
	{   
		playerArmy = SharedPrefs.Instance.playerArmy;
		enemyArmy = SharedPrefs.Instance.enemyArmy;
	}*/
}
