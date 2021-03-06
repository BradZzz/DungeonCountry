﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class BattleSetupManager : MonoBehaviour {


//	public GameObject[] outerWallTiles;
//	public GameObject[] innerWallTiles;
//	public GameObject[] floorTiles;

	private Transform boardHolder;
	//This holds all the panels
	protected Dictionary<Vector2, Transform> dict;
	//This holds all the panels where a character can be placed
	protected Dictionary<Vector2, Transform> placeable;

	private Transform lastClicked;
	private Panel panel;
	private BattleGeneralMeta general;
	private bool settingUp;
	private bool overlay;
	private GameObject lastClickedUnit;
	private BattleArmyManager armyManager;
	private BattleGameManager gameManager;
	private GeneralAttributes attribs;

	private List <Vector3> gridPositions;

	void Awake(){
		lastClicked = null;
		dict = new Dictionary<Vector2, Transform>();
		placeable = new Dictionary<Vector2, Transform>();
		gridPositions = new List <Vector3> ();
		settingUp = true;
		overlay = false;
		lastClickedUnit = null;
	}

	void BoardSetup (BattleLevels level)
	{
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;
		for(int x = -1; x <= gameManager.getColumns(); x++)
		{
			for(int y = -1; y <= gameManager.getRows(); y++)
			{
				GameObject toInstantiate = level.floorTiles[UnityEngine.Random.Range (0,level.floorTiles.Length)];

				if (y == -1 || x == -1 || y == gameManager.getRows() || x == gameManager.getColumns()) {
					//Debug.Log ("Got here!");
					toInstantiate = level.outerWallTiles[UnityEngine.Random.Range (0,level.outerWallTiles.Length)];
					//Debug.Log ("Instantiating: " + toInstantiate.name);
				}

				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
			}
		}
		foreach (Transform child in boardHolder) {
			Vector2 pos = new Vector2 (child.transform.position.x, child.transform.position.y);
			dict[pos] = child;
		}
	}

	void InitialiseList (int margin)
	{
		gridPositions.Clear ();
		for(int x = margin; x < gameManager.getColumns() - margin; x++)
		{
			for(int y = 0; y < gameManager.getRows(); y++)
			{
				gridPositions.Add (new Vector3(x, y, 0f));
			}
		}
	}

	Vector3 RandomPosition ()
	{
		int randomIndex = UnityEngine.Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		int objectCount = UnityEngine.Random.Range (minimum, maximum+1);

		for(int i = 0; i < objectCount; i++)
		{
			Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray[UnityEngine.Random.Range (0, tileArray.Length)];
			GameObject instance = Instantiate (tileChoice, randomPosition, Quaternion.identity) as GameObject;
//			Debug.Log ("Name: " + instance.name);
			instance.transform.SetParent (boardHolder);
		}

		/*int seed = 50;

		for(int i = 0; i < 8; i++)
		{
			//Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray[UnityEngine.Random.Range (0, tileArray.Length)];

			Vector3 randomPosition = gridPositions[seed + i];
			gridPositions.RemoveAt (seed + i);

			GameObject instance = Instantiate (tileChoice, randomPosition, Quaternion.identity) as GameObject;
			Debug.Log ("Name: " + instance.name);
			instance.transform.SetParent (boardHolder);
		}*/
	}

	public void SetupScene (BattleArmyManager armyManager, BattleGameManager gameManager, BattleLevels level)
	{
		this.armyManager = armyManager;
		this.gameManager = gameManager;
		BoardSetup (level);
		InitialiseList (4);
		LayoutObjectAtRandom (level.innerWallTiles, 12, 24);
		attribs = gameManager.getPlayerGeneral().GetComponent<BattleGeneralMeta> ().getResources ().getAttribs ();
	}

	public bool setUnit(Transform floor){
		Vector2 pos = new Vector2 (floor.position.x, floor.position.y);
		if (lastClickedUnit != null && placeable.ContainsKey(pos)) {
			Debug.Log ("Setting unit at position: " + floor.position.ToString());
			Debug.Log ("Unit Name: " + lastClickedUnit.transform.name);
			int position = Int32.Parse (lastClickedUnit.transform.name.Replace ("Unit", "")) - 1;
			GameObject instance = Instantiate (armyManager.getMyArmy()[position], floor.position, Quaternion.identity) as GameObject;
			instance.SetActive (true);

			//Here is where we set the unit bonuses
			BattleMeta unitMeta = armyManager.getMyArmy () [position].GetComponent<BattleMeta> ();
			BattleMeta meta = instance.GetComponent( typeof(BattleMeta) ) as BattleMeta;

			meta.setPlayer (true);
			meta.setLives (unitMeta.getLives());
			meta.setGUI (true);
			meta.setGeneralAttributes (attribs);
			instance.transform.SetParent (boardHolder);
			GameObject.Find ("Unit" + (position + 1)).SetActive(false);
		}

		lastClicked = null;
		overlay = false;
		resetPanels ();

		return true;
	}

	public void panelClicked(GameObject unit, BattleGeneralMeta general){
		overlay = true;
		lastClickedUnit = unit;
		//GeneralAttributes attribs = general.getResources ().getAttribs ();
		for(int x = 0; x < general.tactics; x++) {
			for (int y = 0; y < gameManager.getRows(); y++) {
				Vector2 pos = new Vector2 (x, y);
				Transform child = dict[pos];
				placeable [pos] = child;
				SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
				sprRend.material.shader = Shader.Find ("Custom/OverlayShaderOrange");
			}
		}
	}

	public void resetPanels(){
		for(int x = 0; x < gameManager.getColumns(); x++) {
			for (int y = 0; y < gameManager.getRows(); y++) {
				Vector2 pos = new Vector2 (x, y);
				Transform child = dict[pos];
				SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
				sprRend.material.shader = Shader.Find ("Sprites/Default");
			}
		}
	}

	public void populateUIPanel(List<GameObject> units){
		GameObject oCanvas = GameObject.Find ("oCanvas");
		Transform panel = oCanvas.transform.Find ("Panel");
		for (int i = 1; i < 7; i++) {
			Transform panelUnit = panel.transform.Find ("Unit"+i);
			if (i <= units.Count) {
				Image image = panelUnit.gameObject.GetComponent<Image> ();
				image.sprite = units [i - 1].GetComponent<SpriteRenderer> ().sprite;
				Text livesText = panelUnit.gameObject.transform.GetChild (0).GetComponent<Text> ();
				BattleMeta met = units [i - 1].GetComponent<BattleMeta> ();
				livesText.text = met.getLives().ToString();
			} else {
				panelUnit.gameObject.SetActive (false);
			}
		}
	}

	public void startGame(){
		Debug.Log ("Starting Game");
		settingUp = false;
		overlay = false;
		resetPanels ();
	}

	public Transform getBoard() {
		return boardHolder;
	}

	public bool isSettingUp() {
		//Debug.Log ("isSettingUp: " + settingUp);
		return settingUp;
	}

	public bool getOverlay() {
		return overlay;
	}

	public void setOverlay(bool overlay) {
		this.overlay = overlay;
	}

	public Dictionary<Vector2, Transform> getDict(){
		return dict;
	}
}
