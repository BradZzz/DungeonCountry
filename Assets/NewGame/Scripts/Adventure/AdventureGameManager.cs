using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/*
 * TODO:
 * Move board with drag
 * Zoom in and out with keys
 * Add a better check in each instance for enemy unit and player unit
 * 
 */
using AssemblyCSharp; 

public class AdventureGameManager : MonoBehaviour {

	public static AdventureGameManager instance = null;

	public int columns = 10;
	public int rows = 10;
//	public GameObject[] generals;
	//public GameObject playerGeneral;
	//public GameObject enemyGeneral;

	private AdventureBoardManager boardSetup;
	private int level;

	void Awake()
	{
		Debug.Log ("AdventureGameManager Awake");
		if (instance == null){
			instance = this;
		} else if (instance != this) { 
			Destroy(gameObject);   
		} 
		DontDestroyOnLoad(gameObject);
	}
		
	void Start(){
		instance.gameObject.SetActive (true);
		boardSetup = GetComponent<AdventureBoardManager>();
		level = 1;
		boardSetup.setupScene (instance);
	}

	public void startAgain(){
		Debug.Log ("Start Again");
		Start();
	}

	public int getColumns() {
		return columns + level;
	}

	public int getRows() {
		return rows + level;
	}

	void Update()
	{
		if ( Input.GetMouseButtonDown (0)){ 
			Debug.Log ("Click");
			Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
			RaycastHit2D [] hit = Physics2D.RaycastAll(ray,Vector2.zero,Mathf.Infinity,Physics2D.DefaultRaycastLayers);
			if (hit.Length > 0) {
				boardSetup.clicked (new Point3(hit [0].transform.position));
			}
		}
	}

	public void returnToMenu()
	{
		Destroy (gameObject);
		Application.LoadLevel ("GameEndScene");
	}
}
