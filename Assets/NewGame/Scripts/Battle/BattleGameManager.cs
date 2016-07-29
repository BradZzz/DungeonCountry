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

public class BattleGameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
	public static BattleGameManager instance = null;
	public GameObject[] battleUnits;       

	public int columns = 10;
	public int rows = 10;

	public GameObject playerGeneral;
	public GameObject aiGeneral;

	private BattleSetupManager boardSetup;
	private BattleBoardManager boardScript;
	private BattleArmyManager armyScript;
	private Transform lastHitObj;
	private GameObject levelImage, playerPanel, enemyPanel;
	private Text levelText;

	//private bool playersTurn;
	private int level = 1;

	void Awake()
	{
		if (instance == null){
			instance = this;
		} else if (instance != this) { 
			Destroy(gameObject);   
		} 
		DontDestroyOnLoad(gameObject);
		lastHitObj = null;
		boardScript = GetComponent<BattleBoardManager>();
		boardSetup = GetComponent<BattleSetupManager>();
		//generalScript = GetComponent<BattleGeneralMeta>();
		InitGame();
	}

	//Initializes the game for each level.
	void InitGame()
	{
		levelImage = GameObject.Find("oImage");
		levelText = GameObject.Find("oText").GetComponent<Text>();

		playerPanel = GameObject.Find("PlayerPanel");
		enemyPanel = GameObject.Find("EnemyPanel");

		hidePanel (playerPanel);
		hidePanel (enemyPanel);

		levelImage.SetActive (false);
		//Give the scene and the battle units to the board script
		armyScript = new BattleArmyManager(battleUnits);

		boardSetup.SetupScene (armyScript, instance);
		//boardScript.SetupScene(level, armyScript);
		//Debug.Log ("Dict: " + boardScript.getDict().Count);
	}


	public BattleSetupManager getBoardSetup() {
		return boardSetup;
	}

	public BattleBoardManager getBoard() {
		return boardScript;
	}

	public int getColumns() {
		return columns + level;
	}

	public int getRows() {
		return rows + level;
	}

	void Start() {
		List<GameObject> armies = new List<GameObject>();
		armies.AddRange (armyScript.getMyArmy ());
		boardSetup.populateUIPanel(armies);

		populateGeneralPanel (playerGeneral, GameObject.Find ("PlayerGeneral"));
		populateGeneralPanel (aiGeneral, GameObject.Find ("AIGeneral"));
	}

	public void populateGeneralPanel(GameObject general, GameObject panel){
		GameObject img = panel.transform.Find("Image").gameObject;

		Image image = img.GetComponent<Image> ();
		image.sprite = general.GetComponent<SpriteRenderer> ().sprite;

		BattleGeneralMeta gen = general.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;

		GameObject txt = panel.transform.Find("Text").gameObject;

		Text t = txt.GetComponent<Text> ();
		t.text = gen.name;
	}

	//Update is called every frame.
	void Update()
	{
		if ( Input.GetMouseButtonDown (0)){ 
			Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
			RaycastHit2D [] hit = Physics2D.RaycastAll(ray,Vector2.zero,Mathf.Infinity,Physics2D.DefaultRaycastLayers);
			if (boardSetup.isSettingUp ()) {
				isSettingUp (hit);
			} else {
				isBattle (hit);
			}
		}
	}

	public void endTurn(){
		//playersTurn = !playersTurn;
		hidePanel (playerPanel);
		hidePanel (enemyPanel);
		boardScript.activateUnits ();
	}

	public void panelClicked(GameObject unit){
		int position = Int32.Parse (unit.transform.name.Replace ("Unit", "")) - 1;
		GameObject instance = armyScript.getMyArmy () [position];

		populatePanel (playerPanel, instance);

		BattleGeneralMeta general = playerGeneral.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
		boardSetup.panelClicked (unit, general);
	}

	public void isSettingUp(RaycastHit2D [] hit){

		Debug.Log ("isSettingUp");
		Debug.Log ("Hit: " + hit.Length);

		Transform hitValid = null;

		foreach (RaycastHit2D shot in hit){

			BattleMeta enemy = shot.transform.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;

			if (shot.transform.name.Contains("Floor") && !Coroutines.hasParent(shot.transform)){
				hitValid = shot.transform;
				Debug.Log ("Using: " + hitValid.gameObject.name);
				break;
			} 
		}

		if (hitValid != null) {
			Debug.Log ("Hit");
			if (boardSetup.getOverlay()) {
				Debug.Log ("Overlay");
				hidePanel (playerPanel);
				boardSetup.setUnit (hitValid);
			}
		}
	}

	public void isBattle(RaycastHit2D [] hit){

		Debug.Log ("isBattle");

		Transform hitValid = null;
		foreach (RaycastHit2D shot in hit){

			BattleMeta enemy = shot.transform.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;

			if (!boardScript.charMoving() && !shot.transform.name.Contains("Floor")){
				hitValid = shot.transform;
				Debug.Log ("Using: " + hitValid.gameObject.name);
				break;
			} else if (boardScript.charMoving()) {
				hitValid = shot.transform;
				Debug.Log ("Using: " + hitValid.gameObject.name);
				if (hitValid.tag == "Unit") {
					break;
				} 
			}
		}

		if (hitValid != null) {

			BattleMeta enemy = hitValid.transform.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			if (enemy != null) {
				if (enemy.getPlayer()) {
					populatePanel (playerPanel, hitValid.gameObject);
					hidePanel (enemyPanel);
				} else {
					populatePanel (enemyPanel, hitValid.gameObject);
				}
			}
				
			if (boardScript.charMoving()) {
				Debug.Log ("Hit2!");
				boardScript.moveClick (hitValid.transform);
				if (enemy == null) {
					hidePanel (playerPanel);
					hidePanel (enemyPanel);
				}
			} else {
				Debug.Log ("Hit!");
				boardScript.boardClicked (hitValid.transform);
			}
		}
	}

	public void hidePanel (GameObject panel) {
		panel.SetActive (false);
	}


	public void populatePanel(GameObject panel, GameObject rawUnit){
		panel.SetActive (true);

		BattleMeta unit = rawUnit.GetComponent( typeof(BattleMeta) ) as BattleMeta;

		foreach (Transform child in panel.transform){
			if (child.name == "TextLife"){
				((child.gameObject.GetComponentsInChildren<Text> ()) [0]).text = "" + 1;
			}
			if (child.name == "TextAtk"){
				Debug.Log (unit.attack);
				((child.gameObject.GetComponentsInChildren<Text> ()) [0]).text = "" + unit.attack;
			}
			if (child.name == "TextHealth"){
				((child.gameObject.GetComponentsInChildren<Text> ()) [0]).text = "" + unit.hp;
			}
			if (child.name == "TextMovement"){
				((child.gameObject.GetComponentsInChildren<Text> ()) [0]).text = "" + unit.movement;
			}
			if (child.name == "TextRange"){
				((child.gameObject.GetComponentsInChildren<Text> ()) [0]).text = "" + unit.range;
			}
		}
	}

	public void gameOver(string lvlText){
		GameObject button = GameObject.Find ("Button");
		if (button != null) {
			button.SetActive (false);
		}
		boardScript.UnloadScene ();
		levelText.text = lvlText;
		levelImage.SetActive (true);
		enabled = false;
		Invoke("returnToMenu", levelStartDelay);
	}

	public void startGame(){
		Debug.Log ("BattleGameManager startGame");
		boardSetup.startGame ();

		BattleGeneralMeta general = aiGeneral.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;

		boardScript.setupScene (general, armyScript, boardSetup.getBoard(), boardSetup.getDict(), true);
	}

	public void returnToMenu()
	{
		Destroy (gameObject);
		Application.LoadLevel ("MainMenu");
	}
}
