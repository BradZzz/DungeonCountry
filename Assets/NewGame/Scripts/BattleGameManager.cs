using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleGameManager : MonoBehaviour {

	/*public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
	public float turnDelay = 0.1f;                          //Delay between each Player turn.
	public int playerFoodPoints = 100;*/                      //Starting value for Player food points.
	public static BattleGameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

	/*[HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.
	[HideInInspector] public bool reloadLevel = false;      //Boolean to check if the level has been completed.


	private Text levelText;                                 //Text to display current level number.
	private GameObject levelImage;  */                        //Image to block out level as levels are being set up, background for levelText.
	private BattleBoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
	private BattleArmyManager armyScript;                       //Store a reference to our BoardManager which will set up the level.
	private Transform lastHitObj;
	/*private int level = 1;                                  //Current level number, expressed in game as "Day 1".
	private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
	private bool enemiesMoving;                             //Boolean to check if enemies are moving.
	private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.
	*/
	private int level = 1;

	//Awake is always called before any Start functions
	void Awake()
	{
		Debug.Log ("Awake");
		//Check if instance already exists
		if (instance == null)

			//if not, set instance to this
			instance = this;

		//If instance already exists and it's not this:
		else if (instance != this)

			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);    

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);

		//Assign enemies to a new List of Enemy objects.
		//enemies = new List<Enemy>();

		lastHitObj = null;

		//Get a component reference to the attached BoardManager script
		boardScript = GetComponent<BattleBoardManager>();

		armyScript = GetComponent<BattleArmyManager>();
		boardScript.setArmy (armyScript.army);

		Debug.Log ("Game manager!");

		//Call the InitGame function to initialize the first level 
		InitGame();
	}

	//This is called each time a scene is loaded.
	/*void OnLevelWasLoaded(int index)
	{
		if (reloadLevel) {
			reloadLevel = false;
			Debug.Log ("Match!");
			//Add one to our level number.
			level++;
			Debug.Log ("Level: " + level + " loaded");
			Debug.Log ("Food: " + playerFoodPoints);
			//Call InitGame to initialize our level.
			InitGame();
		}
	}*/

	//Initializes the game for each level.
	void InitGame()
	{
		//While doingSetup is true the player can't move, prevent player from moving while title card is up.
		/*doingSetup = true;

		//Get a reference to our image LevelImage by finding it by name.
		levelImage = GameObject.Find("LevelImage");

		//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
		levelText = GameObject.Find("LevelText").GetComponent<Text>();

		//Set the text of levelText to the string "Day" and append the current level number.
		levelText.text = "Day " + level;

		//Set levelImage to active blocking player's view of the game board during setup.
		levelImage.SetActive(true);

		//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
		Invoke("HideLevelImage", levelStartDelay);

		//Clear any Enemy objects in our List to prepare for next level.
		enemies.Clear();*/

		//Call the SetupScene function of the BoardManager script, pass it current level number.
		boardScript.SetupScene(level);
	}


	/*//Hides black image used between levels
	void HideLevelImage()
	{
		//Disable the levelImage gameObject.
		levelImage.SetActive(false);

		//Set doingSetup to false allowing player to move again.
		doingSetup = false;
	}*/

	//Update is called every frame.
	void Update()
	{
		if ( Input.GetMouseButtonDown (0)){ 
			Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
			RaycastHit2D [] hit = Physics2D.RaycastAll(ray,Vector2.zero,Mathf.Infinity,Physics2D.DefaultRaycastLayers);
			Transform hitValid = null;
			foreach (RaycastHit2D shot in hit){

				BattleMeta enemy = shot.transform.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
				if (enemy != null) {
					Debug.Log ("Valid Iteration!");
					Debug.Log ("It: " + shot.transform.name);
					Debug.Log ("It: " + enemy.gameObject.name);
				}

				if (!boardScript.charMoving() && !shot.transform.name.Contains("Floor")){
					hitValid = shot.transform;
					Debug.Log ("Using: " + hitValid.gameObject.name);
					break;
				} else if (boardScript.charMoving()) {
					hitValid = shot.transform;
					Debug.Log ("Using: " + hitValid.gameObject.name);
					break;
				}
			}

			if (hitValid != null) {

				BattleMeta enemy = hitValid.transform.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
				if (enemy != null) {
					Debug.Log ("Valid Hit!");
				}

				if (boardScript.charMoving()) {
					Debug.Log ("Hit2!");
					boardScript.moveClick (hitValid.transform);
				} else {
					Debug.Log ("Hit!");
					boardScript.boardClicked (hitValid.transform);
				}
			}
		}
	}

	/*public void boardClicked (Transform hitObj, int movementDistance){
		if (!boardScript.charMoving ()) {
			Debug.Log ("Moving");
			boardScript.boardClicked (hitObj, movementDistance);
		} else {
			boardScript.resetMoving ();
		}
	}*/

	//Call this to add the passed in Enemy to the List of Enemy objects.
	/*public void AddEnemyToList(Enemy script)
	{
		//Add Enemy to List enemies.
		enemies.Add(script);
	}

	public void returnToMenu()
	{
		Destroy (gameObject);
		Application.LoadLevel ("MenuScene");
	}

	//GameOver is called when the player reaches 0 food points
	public void GameOver()
	{
		string gameOverStr = "";

		Debug.Log ("Score: " + PlayerPrefs.GetInt ("High Score"));

		if (PlayerPrefs.GetInt ("High Score") < level) {
			gameOverStr += "New Highscore!\n";
			PlayerPrefs.SetInt("High Score", level);
		}
		gameOverStr += "After " + level + " days, you starved.";

		//Set levelText to display number of levels passed and game over message
		levelText.text = gameOverStr;

		//Enable black background image gameObject.
		levelImage.SetActive(true);

		//Disable this GameManager.
		enabled = false;

		Invoke("returnToMenu", levelStartDelay);
	}

	public void DestroyPotions(){
		boardScript.replacePotionsFood ();
	}

	//Coroutine to move enemies in sequence.
	IEnumerator MoveEnemies()
	{
		//While enemiesMoving is true player is unable to move.
		enemiesMoving = true;

		//Wait for turnDelay seconds, defaults to .1 (100 ms).
		yield return new WaitForSeconds(turnDelay);

		//If there are no enemies spawned (IE in first level):
		if (enemies.Count == 0) 
		{
			//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
			yield return new WaitForSeconds(turnDelay);
		}

		//Loop through List of Enemy objects.
		for (int i = 0; i < enemies.Count; i++)
		{
			//Call the MoveEnemy function of Enemy at index i in the enemies List.
			enemies[i].MoveEnemy ();

			//Wait for Enemy's moveTime before moving next Enemy, 
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		//Once Enemies are done moving, set playersTurn to true so player can move.
		playersTurn = true;

		//Enemies are done moving, set enemiesMoving to false.
		enemiesMoving = false;
	}*/
}
