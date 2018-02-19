using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class AdventureBoardManager : MonoBehaviour {

	public static AdventureBoardManager instance = null;
	private static GameObject board;

	public GameObject[] outerWallTiles;
	public GameObject[] innerWallTiles;
	public GameObject[] outerFloorTiles;
	public GameObject[] floorTiles;
	public GameObject[] roadTiles;
	public GameObject[] bloodTiles;
	public GameObject[] castleTiles;
	public GameObject[] entranceTiles;
	public GameObject[] foundationTiles;
	public GameObject[] cliffTiles;
	public GameObject[] waterTiles;
	public GameObject[] bridgeTiles;
	public GameObject[] dwellingTiles;
	public GameObject[] resourceTiles;
	public GameObject footsteps;
	private int waters = 4;

	public GameObject worldCreator;
	public GameObject glossary;
	private AdventureWorldCreator creator;

	public GameObject cliffNinePatch;
	private Glossary glossy;
	private Transform lastClicked;
	private Transform boardHolder;
	private Point3 lastClick;
	private AdventureGameManager gameManager;
	private List<Point3> openPositions;
	private List<Point3> roadPositions;
	protected Dictionary<Point3, Transform> dict;
	private Camera cam;
	private Footsteps steps;
	private List<Point3> path;
	private TileNinePatch cliffPatch;

	private GameObject[] generals;

	void Awake(){

		if (instance == null){
			instance = this;
		} else if (instance != this) { 
			Destroy(gameObject);   
		} 
		DontDestroyOnLoad(gameObject);
	}

	void Start(){
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		glossy = glossary.GetComponent<Glossary> ();
		glossy.init ();
		steps = footsteps.GetComponent<Footsteps>();
		creator = worldCreator.GetComponent<AdventureWorldCreator>();
		openPositions = new List <Point3> ();
		roadPositions = new List <Point3> ();
		cliffPatch = cliffNinePatch.GetComponent<TileNinePatch> ();
		if (board == null) {
			board = new GameObject ("Board");
		}
//		} else if (instance != this) { 
//			Destroy(gameObject);   
//		} 
		DontDestroyOnLoad(board);
	}

	private void formatObjects(bool init){

		GameObject[] generals = BattleConverter.getSave (glossy);
		GameObject cGeneral = CastleConverter.getSave (glossy);

		//if (sharedPrefs || init) {
		foreach(GameObject unity in GameObject.FindGameObjectsWithTag("Unit")){
			//Transform unit = unity.transform;
			//Point3 pos = new Point3(unit.position);
			BattleGeneralMeta meta = unity.GetComponent<BattleGeneralMeta> ();
			if (!init) {
				if (cGeneral != null) {
					BattleGeneralMeta player = cGeneral.GetComponent<BattleGeneralMeta> ();
					if (meta.name.Equals(player.name)) {
						meta.setResources (player.getResources());
						meta.setArmy(player.getArmy());
						if (meta.getArmy().Count < 1) {
							meta.setDefeated (true);
						}
					}
				}
				if (generals != null) {
					BattleGeneralMeta player = generals[0].GetComponent<BattleGeneralMeta> ();
					BattleGeneralMeta ai = generals[1].GetComponent<BattleGeneralMeta> ();
					if (meta.name.Equals(player.name)) {
						meta.setArmy(player.getArmy());
						if (meta.getArmy().Count < 1) {
							meta.setDefeated (true);
						}
						//BattleGeneralResources srcs = meta.getResources ();
						//srcs.setarmy (player.getResources().getarmy());
					} else if (meta.name.Equals(ai.name)) {
						meta.setArmy(ai.getArmy());
						if (meta.getArmy().Count < 1) {
							meta.setDefeated (true);
						}
						//BattleGeneralResources srcs = meta.getResources ();
						//srcs.setarmy (ai.getResources().getarmy());
					}
				}
			}

			if (generals != null) {
				BattleConverter.reset ();
			}

			if (meta != null) {
				if (meta.getDefeated ()) {
					unity.SetActive (false);

					//Add blood splatter where hero was killed
					GameObject tileChoice = bloodTiles[UnityEngine.Random.Range (0, bloodTiles.Length)];
					GameObject instance = Instantiate (tileChoice, unity.transform.position, Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);
				} 
				if (init) {
					//BattleGeneralResources res = meta.getResources ();
					//if (res != null){
					List<GameObject> newUnits = new List<GameObject> ();
					AffiliationMeta affmet = glossy.findFaction(meta.faction);
					foreach (GameObject unit in affmet.units) {
						BattleMeta gmeta = unit.GetComponent<BattleMeta> ();
						if (gmeta.lvl < 3) {
							if (gmeta.lvl == 1) {
								gmeta.setLives (Random.Range (15, 25));
							} else {
							if (meta.getPlayer()) {
									gmeta.setLives (Random.Range (15, 20));
								} else {
									gmeta.setLives (Random.Range (5, 10));
								}
							}
							newUnits.Add (unit);
						}
					}
					meta.setArmy(newUnits);
					//}
				}
			}
			if (meta == null) {
				Debug.Log ("Meta is null");
			} else {
				Debug.Log ("Name: " + meta.name + " Defeated: " + meta.getDefeated());
			}

		}
		BattleConverter.reset ();
		CastleConverter.reset ();
	}

	public void setupScene (AdventureGameManager gameManager, GameObject[] generals)
	{
		Start ();
		this.gameManager = gameManager;
		this.generals = generals;
		boardHolder = board.transform;
		if (boardHolder.childCount > 0) {
			Debug.Log ("Refreshing board");
			boardHolder = board.transform;
			boardHolder.gameObject.SetActive (true);
			Coroutines.toggleVisibilityTransform(boardHolder,true);
			formatObjects (false);
		} else {
			Debug.Log ("Creating board");
			creator.createWorld(boardHolder, gameManager.getColumns (), gameManager.getRows (), boardCreated);
		}
	}

	public void boardCreated(List<Point3> roadPositions, List<Point3> openPositions) {
		GameObject board = GameObject.Find("Board");
		this.boardHolder = board.transform;
		this.roadPositions = roadPositions;
		this.openPositions = openPositions;

		foreach (GameObject general in generals){
			BattleGeneralMeta gMeta = general.GetComponent<BattleGeneralMeta> ();
			if (gMeta != null) {
				if (checkFaction (SharedPrefs.getPlayerFaction ()) == gMeta.faction) {
					LayoutObjectAtRandom (new GameObject[]{ general }, 1, 1, true, true);
				} else {
					LayoutObjectAtRandom (new GameObject[]{ general }, 1, 1, true, false);
				}
			}
		}

		formatObjects (true);
	}

	public string checkFaction(int faction){
		return GameStatics.factionDict[faction];
	}

	Point3 RandomPosition (bool onRoad)
	{
		Point3 randomPosition;
		if (onRoad) {
			int randomIndex = UnityEngine.Random.Range (0, roadPositions.Count);
			randomPosition = roadPositions [randomIndex];
			roadPositions.RemoveAt (randomIndex);
		} else {
			int randomIndex = UnityEngine.Random.Range (0, openPositions.Count);
			randomPosition = openPositions [randomIndex];
			openPositions.RemoveAt (randomIndex);
		}
		return randomPosition;
	}

	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum, bool onRoad, bool isPlayer)
	{
		int objectCount = UnityEngine.Random.Range (minimum, maximum+1);
		for(int i = 0; i < objectCount; i++)
		{
			Point3 randomPosition = RandomPosition(onRoad);
			GameObject tileChoice = tileArray[UnityEngine.Random.Range (0, tileArray.Length)];
			GameObject instance = Instantiate (tileChoice, randomPosition.asVector3(), Quaternion.identity) as GameObject;
			BattleGeneralMeta gMeta = instance.GetComponent<BattleGeneralMeta> ();
			if (gMeta != null) {
				gMeta.setPlayer (isPlayer);
			}
			Debug.Log ("Laying down, name: " + instance.name + " position: " + instance.transform.position);
			instance.transform.SetParent (boardHolder);
		}
	}

	public void clicked(Point3 click){
		Debug.Log ("Clicked: " + click.ToString());
		Debug.Log("LastClicked: " + lastClicked);
		if (lastClicked == null) {
			Debug.Log ("New Click");
			foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
				Debug.Log ("Searching");
				BattleGeneralMeta gMeta = unit.GetComponent<BattleGeneralMeta> ();
				if (gMeta != null) {
					if  (click.Equals(unit.transform.position) && gMeta.getPlayer()) {
						Debug.Log ("Clicked: " + click.ToString());
						lastClicked = unit.transform;
					}
				}
			}
		} else if (lastClicked != null) { 
			Debug.Log("LastClicked: " + lastClicked.name + " pos: " + lastClicked.position);
			if (!click.Equals(lastClicked.position) && (!steps.walking () || !click.Equals(lastClick))) {
				steps.destroySteps ();
				Debug.Log ("Moving: " + lastClicked.name);
				List<Point3> obstacles = new List<Point3> ();
				foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
					Debug.Log ("Placing Unit: " + unit.name + " Position: " + unit.transform.position);
					obstacles.Add (new Point3(unit.transform.position));
				}

				foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Obstacle")) {
					obstacles.Add (new Point3(obs.transform.position));
				}

				foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Entrance")) {
					obstacles.Add (new Point3(obs.transform.position));
				}

				StartCoroutine (steps.generateMapv2 (new Point3(lastClicked.position), click, gameManager.getRows (), gameManager.getColumns (), obstacles, setPath));
			} else if (steps.walking () && click.Equals(lastClick)) {
//				BattleConverter.reset ();
				moveAdventurer (lastClicked, path);
				lastClicked = null;
				steps.destroySteps ();
			}
		}
	}

	public bool inScene(Point3 targetPosition){
		//Point3 screenPoint = cam.WorldToViewportPoint(targetPosition);
		//return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

		return true;
	}

	public void moveAdventurer(Transform lastClicked, List<Point3> path) {
		//Check to make sure the last step isn't an enemy here
		int prevTotal = path.Count;

		Point3 edge  = path [path.Count - 1];

		Debug.Log ("Searching for: " + edge.ToString());

		bool attacking = false;
		GameObject enemy = Coroutines.findUnitParent (edge);

		if (enemy != null) {
			Debug.Log ("Enemy is not null!");

			path.Remove (edge);

			if (enemy.tag.Equals ("Unit")) {
				//SharedPrefs.playerArmy = Instantiate (lastClicked.gameObject, lastClicked.position, Quaternion.identity) as GameObject;
				//SharedPrefs.playerArmy.SetActive (false);

//				SharedPrefs.setPlayerName (lastClicked.gameObject.name);
//				SharedPrefs.setEnemyName (enemy.name);
//
//				//SharedPrefs.enemyArmy = Instantiate (enemy, enemy.transform.position, Quaternion.identity) as GameObject;
//				//SharedPrefs.enemyArmy.SetActive (false);
//				Debug.Log ("Player: " + SharedPrefs.getPlayerName());
//				Debug.Log ("Enemy: " + SharedPrefs.getEnemyName());

				BattleGeneralMeta player = lastClicked.gameObject.GetComponent<BattleGeneralMeta> ();
				BattleGeneralMeta ai = enemy.GetComponent<BattleGeneralMeta> ();
				BattleConverter.putSave (player, ai);
				BattleConverter.putPrevScene ("AdventureScene");

				Debug.Log (PlayerPrefs.GetString ("battle"));

				attacking = true;
			}
		} else {
			Debug.Log ("Enemy is null!");
		}

		StartCoroutine (step_path (lastClicked, path, .5f, attacking));
		//If the last step is an enemy, we need to fight it here
	}

	public void setPath(List<Point3> path, Point3 destination){
		this.path = path;
		if (path != null) {
			steps.createSteps (new Point3(lastClicked.position), boardHolder, path);
			lastClick = destination;
		} else {
			lastClicked = null;
		}
	}

	//This takes each individual path and smooths them out
	public List<Point3> compilePath(List<Point3> path){
		List<Point3> smoothed = new List<Point3> ();
		Point3 lastStep = null;
		bool changeX = false;
		foreach (Point3 step in path) {
			bool lastX = changeX;
			if (lastStep == null) { lastStep = step; }
			if (lastStep.x != step.x) { changeX = true; }
			if (lastStep.y != step.y) { changeX = false; }
			if (lastX != changeX) { smoothed.Add (lastStep); } 
			lastStep = step;
		}
		smoothed.Add (path[path.Count - 1]);
		return smoothed;
	}

	IEnumerator step_path(Transform origin, List<Point3> path, float speed, bool battle)
	{
		foreach(Point3 step in /*compilePath(*/path/*)*/){
			yield return StartCoroutine( Coroutines.smooth_move(origin, step.asVector3(), speed));
		}
		if (battle) {
			//Hide the board. We'll remove from the heirarchy once the meta has been loaded in the next scene
			Coroutines.toggleVisibilityTransform(boardHolder,false);
			//Remove the gameManager click listeners
			gameManager.gameObject.SetActive (false);
			Application.LoadLevel ("BattleScene");
		}
	}
}
