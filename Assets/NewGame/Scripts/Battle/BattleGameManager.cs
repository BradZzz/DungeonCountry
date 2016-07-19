using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleGameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
	public static BattleGameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
	public GameObject[] battleUnits;       

	public int columns = 10;                                         //Number of columns in our game board.
	public int rows = 10;                                            //Number of rows in our game board.

	private BattleSetupManager boardSetup;
	private BattleBoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
	private BattleArmyManager armyScript;                         //Store a reference to our ArmyManager which will set up the level.
	//private BattleGeneralMeta generalScript;                         //Store a reference to our ArmyManager which will set up the level.
	private Transform lastHitObj;
	private GameObject levelImage;
	private Text levelText;

	private int level = 1;

	//Awake is always called before any Start functions
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

	public void isSettingUp(RaycastHit2D [] hit){

		Debug.Log ("isSettingUp");
		Debug.Log ("Hit: " + hit.Length);

		Transform hitValid = null;

		foreach (RaycastHit2D shot in hit){

			BattleMeta enemy = shot.transform.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;

			if (shot.transform.name.Contains("Floor") && !boardScript.hasParent(shot.transform)){
				hitValid = shot.transform;
				Debug.Log ("Using: " + hitValid.gameObject.name);
				break;
			} 
		}

		if (hitValid != null) {
			Debug.Log ("Hit");
			if (boardSetup.getOverlay()) {
				Debug.Log ("Overlay");
				boardSetup.setUnit (hitValid);
			}
		}
	}

	public void isBattle(RaycastHit2D [] hit){
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

			if (boardScript.charMoving()) {
				Debug.Log ("Hit2!");
				boardScript.moveClick (hitValid.transform);
			} else {
				Debug.Log ("Hit!");
				boardScript.boardClicked (hitValid.transform);
			}
		}
	}

	public void gameOver(string lvlText){
		boardScript.UnloadScene ();
		levelText.text = lvlText;
		levelImage.SetActive (true);
		enabled = false;
		Invoke("returnToMenu", levelStartDelay);
	}

	public void startGame(){
		boardSetup.startGame ();
		boardScript.setupScene (armyScript, boardSetup.getBoard(), boardSetup.getDict());
	}

	public void returnToMenu()
	{
		Destroy (gameObject);
		Application.LoadLevel ("MainMenu");
	}
}
