using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

/*
 * TODO:
 * Move board with drag
 * Zoom in and out with keys
 * Add a better check in each instance for enemy unit and player unit
 * 
 */
using AssemblyCSharp; 

public class BattleGameManager : MonoBehaviour {

	public float levelStartDelay = 2.2f;
	public static BattleGameManager instance = null;
	//public GameObject[] battleUnits;       

	public int columns = 10;
	public int rows = 10;

	//public GameObject playerGeneral;
	//public GameObject aiGeneral;

	private GameObject playerGeneral;
	private GameObject aiGeneral;

	private BattleSetupManager boardSetup;
	private BattleBoardManager boardScript;
	private BattleArmyManager armyScript;
	private Transform lastHitObj;
	private GameObject levelImage, playerPanel, enemyPanel;
	private Text levelText, gameOverText;

	//private bool playersTurn;
	private int level = 1;

	//New Shit
	Glossary glossary;

	void Awake()
	{
//		if (instance == null){
//			instance = this;
//		} else if (instance != this) { 
//			Destroy(gameObject);   
//		} 
//		DontDestroyOnLoad(gameObject);

		instance = this;
		lastHitObj = null;
		boardScript = GetComponent<BattleBoardManager>();
		boardSetup = GetComponent<BattleSetupManager>();
		//generalScript = GetComponent<BattleGeneralMeta>();
		InitGame();

		Debug.Log ("Printing Prefs");
	}

	//Initializes the game for each level.
	void InitGame()
	{
		levelImage = GameObject.Find("oImage");
		levelText = GameObject.Find("gText").GetComponent<Text>();
		gameOverText = GameObject.Find("oText").GetComponent<Text>();

		playerPanel = GameObject.Find("PlayerPanel");
		enemyPanel = GameObject.Find("EnemyPanel");

		glossary = GameObject.Find("Glossary").GetComponent<Glossary>();

		hidePanel (playerPanel);
		hidePanel (enemyPanel);

		levelImage.SetActive (false);
		levelText.gameObject.SetActive (false);
	}

	public Glossary getGlossary() {
		return glossary;
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

	public GameObject getPlayerGeneral() {
		return playerGeneral;
	}

	public GameObject getAIGeneral() {
		return aiGeneral;
	}

	void Start() {
		Debug.Log ("Start");
		Debug.Log("Player Name: " + SharedPrefs.getPlayerName());
		Debug.Log("Enemy Name: " + SharedPrefs.getEnemyName());

		//For testing...
		//BattleConverter.putSaveDemo ();

		GameObject[] save = BattleConverter.getSave(glossary);

		playerGeneral = save [0];
		aiGeneral = save [1];

		BattleGeneralMeta playerGenMeta = playerGeneral.GetComponent<BattleGeneralMeta>();
		BattleGeneralMeta aiGenMeta = aiGeneral.GetComponent<BattleGeneralMeta>();

		armyScript = new BattleArmyManager(playerGenMeta.getResources().getarmy().ToArray(), aiGenMeta.getResources().getarmy().ToArray());
		boardSetup.SetupScene (armyScript, instance, glossary.findLevels(BattleConverter.getSaveWorld()));

		//This puts the player's army into the ui panel
		List<GameObject> armies = new List<GameObject>();
		armies.AddRange (armyScript.getMyArmy ());
		boardSetup.populateUIPanel(armies);
		populateGeneralPanel (playerGeneral, GameObject.Find ("PlayerGeneral"), true);
		populateGeneralPanel (aiGeneral, GameObject.Find ("AIGeneral"), false);
	}

	public void populateGeneralPanel(GameObject general, GameObject panel, bool player){

		GameObject imgPanel;
		if (player) {
			imgPanel = panel.transform.Find ("PlayerGPanel").gameObject;
		} else {
			imgPanel = panel.transform.Find ("AIGPanel").gameObject;
		}

		GameObject img = imgPanel.transform.Find("Image").gameObject;

		Image image = img.GetComponent<Image> ();
		image.sprite = general.GetComponent<Image> ().sprite;

		BattleGeneralMeta gen = general.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;

		GameObject txt = panel.transform.Find("Text").gameObject;

		Text t = txt.GetComponent<Text> ();
		t.text = gen.name;
		t.resizeTextForBestFit = true;
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
		if (armyScript.getMyArmy () != null) {
			GameObject instance = armyScript.getMyArmy () [position];

			populatePanel (playerPanel, instance);

			BattleGeneralMeta general = playerGeneral.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
			boardSetup.panelClicked (unit, general);
		}
	}

	public void isSettingUp(RaycastHit2D [] hit){

		//Debug.Log ("isSettingUp");
		//Debug.Log ("Hit: " + hit.Length);

		Transform hitValid = null;

		foreach (RaycastHit2D shot in hit){

			BattleMeta enemy = shot.transform.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;

			if (shot.transform.name.Contains("Floor") && !Coroutines.hasParent(shot.transform)){
				hitValid = shot.transform;
				//Debug.Log ("Using: " + hitValid.gameObject.name);
				break;
			} 
		}

		if (hitValid != null) {
			//Debug.Log ("Hit");
			if (boardSetup.getOverlay()) {
				//Debug.Log ("Overlay");
				hidePanel (playerPanel);
				boardSetup.setUnit (hitValid);
			}
		}
	}

	public void isBattle(RaycastHit2D [] hit){

		//Debug.Log ("isBattle");

		Transform hitValid = null;
		foreach (RaycastHit2D shot in hit){
			if (!boardScript.charMoving() && !shot.transform.name.Contains("Floor")){
				hitValid = shot.transform;
				//Debug.Log ("Using: " + hitValid.gameObject.name);
				break;
			} else if (boardScript.charMoving()) {
				hitValid = shot.transform;
				//Debug.Log ("Using: " + hitValid.gameObject.name);
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
				//Debug.Log ("Hit2!");
				boardScript.moveClick (hitValid.transform);
				if (enemy == null) {
					hidePanel (playerPanel);
					hidePanel (enemyPanel);
				}
			} else {
				//Debug.Log ("Hit!");
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
				((child.gameObject.GetComponentsInChildren<Text> ()) [0]).text = "" + unit.getLives();
			}
			if (child.name == "TextAtk"){
				((child.gameObject.GetComponentsInChildren<Text> ()) [0]).text = "" + unit.getCharStrength();
			}
			if (child.name == "TextHealth"){
				((child.gameObject.GetComponentsInChildren<Text> ()) [0]).text = "" + unit.getCharHp();
			}
			if (child.name == "TextMovement"){
				((child.gameObject.GetComponentsInChildren<Text> ()) [0]).text = "" + unit.getMovement();
			}
			if (child.name == "TextRange"){
				((child.gameObject.GetComponentsInChildren<Text> ()) [0]).text = "" + unit.getRange();
			}
			if (child.name == "AbilityText"){
				((child.gameObject.GetComponentsInChildren<Text> ()) [0]).text = "" + unit.getAbilityString();
			}
		}
	}

	public void gameOver(bool won, string lvlText, List <GameObject>[] results){

		BattleGeneralMeta playGen = playerGeneral.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
		BattleGeneralMeta aiGen = aiGeneral.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;

		if (won) {
			aiGen.setDefeated (true);
			ScoreSerializeableStats score = new ScoreSerializeableStats ();
			score.medal = 0;
			score.name = ScoreConverter.getCurrentLvl();
			score.score = ScoreConverter.computeResults(results[0]);
			ScoreConverter.putSaveScoreObject(score);
		} else {
			playGen.setDefeated (true);
		}

		playGen.setArmy(results[0]);
		aiGen.setArmy(results[1]);

		BattleConverter.putSave (playGen, aiGen, null);

		GameObject button = GameObject.Find ("Button");
		if (button != null) {
			button.SetActive (false);
		}
		boardScript.UnloadScene ();
		gameOverText.text = lvlText;
		levelImage.SetActive (true);
		enabled = false;
		Invoke("returnToMenu", levelStartDelay);
	}

	public void startGame(){
		Debug.Log ("BattleGameManager startGame");
		boardSetup.startGame ();

		BattleGeneralMeta player = playerGeneral.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
		BattleGeneralMeta ai = aiGeneral.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;

		boardScript.setupScene (player, ai, armyScript, boardSetup.getBoard(), boardSetup.getDict(), true, glossary.findLevels(BattleConverter.getSaveWorld()));

		setLevelImageText ("Game Start");
	}

	public void setLevelImageText(string msg)
	{
		levelText.gameObject.SetActive (true);
		playEffect ("Stars");
		levelText.text = msg;
		//playEffect ("Blood");
		Invoke("setLevelTextInactive", levelStartDelay);
	}

	public void setLevelTextInactive()
	{
		levelText.text = "";
		levelText.gameObject.SetActive (false);
	}

	public void returnToMenu()
	{
		Destroy (gameObject);

		Debug.Log ("Fin");
		Application.LoadLevel (BattleConverter.prevScene());
	}

	public void playEffect(string effect) {
		try {
			EffectConfig exp = levelText.transform.Find(effect).gameObject.GetComponent<EffectConfig> ();
			exp.Play ();
		} catch (Exception e) {
			Debug.Log (e.Message);
		}
	}
}
