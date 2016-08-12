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
	private AdventureWorldCreator creator;

	public GameObject cliffNinePatch;

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
		steps = footsteps.GetComponent<Footsteps>();
		creator = worldCreator.GetComponent<AdventureWorldCreator>();
		openPositions = new List <Point3> ();
		roadPositions = new List <Point3> ();
		cliffPatch = cliffNinePatch.GetComponent<TileNinePatch> ();
		if (board == null){
			board = new GameObject ("Board");
		} else if (instance != this) { 
			Destroy(gameObject);   
		} 
		DontDestroyOnLoad(board);
	}

	private void formatObjects(){

		Point3 playerPos = new Point3(), enemyPos = new Point3();
		bool sharedPrefs = SharedPrefs.playerArmy != null && SharedPrefs.enemyArmy != null;

		if (sharedPrefs) {
			playerPos = new Point3(SharedPrefs.playerArmy.transform.position);
			Debug.Log ("Player: " + SharedPrefs.playerArmy.name + "Position: " + enemyPos.ToString ());
			enemyPos = new Point3(SharedPrefs.enemyArmy.transform.position);
			Debug.Log ("Enemy: " + SharedPrefs.enemyArmy.name + "Position: " + enemyPos.ToString ());
		}

		if (sharedPrefs) {
			foreach(GameObject unity in GameObject.FindGameObjectsWithTag("Unit")){
				Transform unit = unity.transform;
				Point3 pos = new Point3(unit.position);
				BattleGeneralMeta meta = null;
				if (pos.Equals(playerPos)) {
					//(item.transform.position.Equals(playerPos) || item.transform.position.Equals(enemyPos))
					meta = SharedPrefs.playerArmy.gameObject.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
				} else if (pos.Equals(enemyPos)) {
					meta = SharedPrefs.enemyArmy.gameObject.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
				}
				if (meta == null) {
					Debug.Log ("Meta is null");
					Debug.Log ("playerPosition: " + playerPos.ToString());
					Debug.Log ("enemyPosition: " + enemyPos.ToString());
					Debug.Log ("thisPosition: " + pos.ToString());
				} else {
					Debug.Log ("Name: " + meta.name + " Defeated: " + meta.getDefeated());
				}

				if (meta != null && meta.getDefeated ()) {
					unit.gameObject.SetActive (false);

					//Add blood splatter where hero was killed
					GameObject tileChoice = bloodTiles[UnityEngine.Random.Range (0, bloodTiles.Length)];
					GameObject instance = Instantiate (tileChoice, unit.position, Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);
				} 
			}
		}
	}

	public void setupScene (AdventureGameManager gameManager, GameObject[] generals)
	{

		Start ();

		this.gameManager = gameManager;
		this.generals = generals;

		//worldCreator.SetActive(true);
		//GameObject board = GameObject.Find("Board");

		boardHolder = board.transform;
		//Debug.Log ("Children: " + boardHolder.childCount);

		if (boardHolder.childCount > 0) {
			Debug.Log ("Refreshing board");
			boardHolder = board.transform;
			boardHolder.gameObject.SetActive (true);
			formatObjects ();
		} else {
			Debug.Log ("Creating board");
			creator.createWorld(boardHolder, gameManager.getColumns (), gameManager.getRows (), boardCreated);
		}
	}

	public void boardCreated(List<Point3> roadPositions, List<Point3> openPositions) {
		GameObject board = GameObject.Find("Board");
		//DontDestroyOnLoad (board);
		this.boardHolder = board.transform;
		this.roadPositions = roadPositions;
		this.openPositions = openPositions;

		foreach (GameObject general in generals) {
			placeGeneral (general);
		}

		formatObjects ();
	}

	private void placeGeneral (GameObject general)
	{
		LayoutObjectAtRandom (new GameObject[]{general}, 1, 1, true);
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

	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum, bool onRoad)
	{
		int objectCount = UnityEngine.Random.Range (minimum, maximum+1);
		for(int i = 0; i < objectCount; i++)
		{
			Point3 randomPosition = RandomPosition(onRoad);
			GameObject tileChoice = tileArray[UnityEngine.Random.Range (0, tileArray.Length)];
			GameObject instance = Instantiate (tileChoice, randomPosition.asVector3(), Quaternion.identity) as GameObject;
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
				if  (click.Equals(unit.transform.position)) {
					Debug.Log ("Clicked: " + click.ToString());
					lastClicked = unit.transform;
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
				StartCoroutine (steps.generateMapv2 (new Point3(lastClicked.position), click, gameManager.getRows (), gameManager.getColumns (), obstacles, setPath));
			} else if (steps.walking () && click.Equals(lastClick)) {
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
				SharedPrefs.playerArmy = Instantiate (lastClicked.gameObject, lastClicked.position, Quaternion.identity) as GameObject;
				SharedPrefs.playerArmy.SetActive (false);
				SharedPrefs.enemyArmy = Instantiate (enemy, enemy.transform.position, Quaternion.identity) as GameObject;
				SharedPrefs.enemyArmy.SetActive (false);
				Debug.Log ("Player: " + SharedPrefs.playerArmy.name);
				Debug.Log ("Enemy: " + SharedPrefs.enemyArmy.name);
				attacking = true;
			}
		} else {
			Debug.Log ("Enemy is null!");
		}

		StartCoroutine (step_path (lastClicked, path, 1f, attacking));
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

	IEnumerator step_path(Transform origin, List<Point3> path, float speed, bool battle)
	{
		foreach(Point3 step in path){
			yield return StartCoroutine( smooth_move(origin, step.asVector3(), speed));
		}
		if (battle) {
			gameManager.gameObject.SetActive (false);
			boardHolder.gameObject.SetActive (false);
			Application.LoadLevel ("BattleScene");
		}
	}

	IEnumerator smooth_move(Transform origin, Vector3 direction,float speed){
		float startime = Time.time;
		Vector3 start_pos = new Vector3(origin.position.x, origin.position.y, origin.position.z);
		Vector3 end_pos = direction;
		while (!origin.position.Equals(end_pos)) { 
			float move = Mathf.Lerp (0,1, (Time.time - startime) * speed);

			Vector3 position = origin.position;

			position.x += ((end_pos.x - start_pos.x) * move);
			position.y += ((end_pos.y - start_pos.y) * move);

			if (start_pos.x > end_pos.x && origin.position.x < end_pos.x) {
				position.x = end_pos.x;
			}

			if (start_pos.x < end_pos.x && origin.position.x > end_pos.x) {
				position.x = end_pos.x;
			}

			if (start_pos.y > end_pos.y && origin.position.y < end_pos.y) {
				position.y = end_pos.y;
			}

			if (start_pos.y < end_pos.y && origin.position.y > end_pos.y) {
				position.y = end_pos.y;
			}

			origin.position = position;

			if (((Time.time - startime)*speed) >= .75f) {
				origin.position = end_pos;
			}

			yield return null;
		}
			
		//yield return null;
	}
}
