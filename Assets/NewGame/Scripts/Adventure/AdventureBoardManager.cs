using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine.SceneManagement;

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
		DontDestroyOnLoad(board);
	}

	private void turnOnCastleGUI(){
		foreach (GameObject ents in GameObject.FindGameObjectsWithTag("Entrance"))
		{
			EntranceMeta eMet = ents.GetComponent<EntranceMeta> ();
			if (eMet != null) {
				eMet.showFlag ();
			}
		}
	}

	class DescComparer<T> : IComparer<T>
	{
		public int Compare(T x, T y)
		{
			return Comparer<T>.Default.Compare(y, x);
		}
	}

	private Point3[] getEmptySpaces(Point3 startPt, int spcNeeded){
		string strtPt = (new Point3 (startPt.x, startPt.y, 0)).ToString ();
		Point3[] spcs = new Point3[spcNeeded];
		int cnt = 0;
		int spread = 1;
		//gameManager.getColumns (), gameManager.getRows ()
		List<string> obs = new List<string>();
		foreach(Point3 ob in getObstacles ()){
			obs.Add ((new Point3 (ob.x, ob.y, 0)).ToString());
		}

		SortedList<string,int> pts = new SortedList<string,int>(new DescComparer<string>());
		for (int x = 0; x < gameManager.getColumns (); x++) {
			for (int y = 0; y < gameManager.getRows (); y++) {
				int distance = Mathf.Abs(startPt.x - x) + Mathf.Abs(startPt.y - y);
				string pt = (new Point3 (x, y, 0)).ToString ();
				if (!obs.Contains(pt) && !strtPt.Equals(pt) && distance < 3) {
					pts.Add (pt, distance);
				}
			}
		}

		int idx = 0;
		foreach (KeyValuePair<string, int> pair in pts)
		{
			string[] coor = pair.Key.Split(',');
			if (idx == spcNeeded) {
				break;
			}
			spcs [idx] = new Point3 (int.Parse(coor[0]), int.Parse(coor[1]), 0);
			idx++;
		}
		return spcs;
	}

	private void formatObjects(bool init){

		GameObject[] savedGenerals = new GameObject[0];

		if (!init) {
			turnOnCastleGUI ();
			BattleGeneralMeta[] bGenerals = BattleConverter.getSaveBGM (glossy);
			GameObject cGeneral = CastleConverter.getSave (glossy);
			GameObject[] tGenerals = CastleConverter.getBoughtTavernGenerals (glossy);

			if (cGeneral != null) {
				BattleGeneralMeta cgMeta = cGeneral.GetComponent<BattleGeneralMeta> ();
				DataStoreConverter.updateGeneral (glossy, "BoardSave", cgMeta);
				if (tGenerals != null) {
					//Place the new generals here
					Debug.Log("New generals bought at the tavern");
					//Now we need to find the closest spaces around the cGeneral
					Point3 pos = new Point3();
					foreach (GameObject unity in GameObject.FindGameObjectsWithTag("Unit")) {
						if (unity.name.Contains(cGeneral.name)) {
							pos = new Point3 (unity.transform.position);
						}
					}
					Point3[] spaces = getEmptySpaces(pos, tGenerals.Length);
					for (int i = 0; i < tGenerals.Length; i++) {
						//General appearing in the wrong place
						//Cannot select general
						GameObject instance = Instantiate (tGenerals[i], spaces[i].asVector3(), Quaternion.identity, boardHolder) as GameObject;
						instance.transform.localPosition = spaces [i].asVector3 ();
//						GameObject instance = Instantiate (tGenerals[i], spaces[i].asVector3(), Quaternion.identity) as GameObject;
//						instance.transform.SetParent (boardHolder);
					}
				}
			}
			if (bGenerals != null) {
				DataStoreConverter.updateGeneral (glossy, "BoardSave", bGenerals);
				BattleConverter.reset ();
			}

			savedGenerals = DataStoreConverter.getSave (glossy, "BoardSave");
		} else {
			DataStoreConverter.reset ("BoardSave");
			BattleConverter.reset ();
			CastleConverter.reset ();
		}

		//if (sharedPrefs || init) {
		foreach(GameObject unity in GameObject.FindGameObjectsWithTag("Unit")){
			//Transform unit = unity.transform;
			//Point3 pos = new Point3(unit.position);
			BattleGeneralMeta meta = unity.GetComponent<BattleGeneralMeta> ();
			if (!init && meta != null) {
				foreach (GameObject general in savedGenerals) {
					BattleGeneralMeta gen = general.GetComponent<BattleGeneralMeta> ();
					if (meta.name.Equals (gen.name)) {
						if (gen.getResources() != null) {
							meta.setResources (gen.getResources());
						}
						meta.setArmy(gen.getArmy());
						if (meta.getArmy().Count < 1) {
							meta.setDefeated (true);
						}
					}
				}
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
						if ((!meta.faction.Equals ("Neutral") && gmeta.lvl < 3) || (meta.faction.Equals ("Neutral") && gmeta.lvl < 5)) {
							GameObject instance = Instantiate (unit) as GameObject;
							BattleMeta bMet = instance.GetComponent<BattleMeta> ();
							bMet.setGUI (false);
							if (meta.getPlayer ()) {
								bMet.setPlayer (true);
								switch(bMet.lvl){
									case 1:
										bMet.setLives (Random.Range (15, 25));
										break;
									default:
										bMet.setLives (Random.Range (115, 200));
										break;
								}
							} else {
								bMet.setPlayer (false);
								if (meta.faction.Equals ("Neutral")) {
									switch(bMet.lvl){
//										case 1:
//											bMet.setLives (Random.Range (80, 100));
//											break;
//										case 2:
//											bMet.setLives (Random.Range (60, 80));
//											break;
//										case 3:
//											bMet.setLives (Random.Range (40, 60));
//											break;
										default:
											bMet.setLives (Random.Range (5, 10));
											break;
									}
								} else {
									switch(bMet.lvl){
										case 1:
											bMet.setLives (Random.Range (15, 25));
											break;
										default:
											bMet.setLives (Random.Range (5, 10));
											break;
									}
								}
							}

							instance.SetActive (false);
							newUnits.Add (instance);
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

	public void setupScene (AdventureGameManager gameManager)
	{
		Start ();
		this.gameManager = gameManager;
		this.generals = glossy.generals;
		boardHolder = board.transform;
		if (boardHolder.childCount > 0) {
			Debug.Log ("Refreshing board");
			boardHolder = board.transform;
			boardHolder.gameObject.SetActive (true);
			Coroutines.toggleVisibilityTransform(boardHolder,true);
			formatObjects (false);
			GameObject pMenu = GameObject.Find("PlayerMenu");
			AdventurePanel pMen = pMenu.GetComponent<AdventurePanel> ();
			pMen.FinishTurn ();
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

		List<string> foundFactions = new List<string> ();
		Coroutines.ShuffleArray (generals);
		/*
		Here is where the generals banners need to be selected from the color list
		*/ 

//		List<Color> banners = new List<Color> () {
//			Color.blue,
//			Color.cyan,
//			Color.green,
//			Color.magenta,
//			Color.red,
//			Color.yellow
//		};
		Color[] banners = new Color[]{
			Color.blue,
			Color.cyan,
			Color.green,
			Color.magenta,
			Color.red,
			Color.yellow
		};
		//Coroutines.ShuffleArray (banners);

		int count = 0;

		foreach (GameObject general in generals){
			BattleGeneralMeta gMeta = general.GetComponent<BattleGeneralMeta> ();
			if (gMeta != null && (!foundFactions.Contains(gMeta.faction) || (gMeta.faction.Equals("Neutral")))) {
				if (checkFaction (SharedPrefs.getPlayerFaction ()) == gMeta.faction) {
					LayoutObjectAtRandom (new GameObject[]{ general }, 1, 1, true, true);
				} else {
					LayoutObjectAtRandom (new GameObject[]{ general }, 1, 1, true, false);
				}
				if (!gMeta.faction.Equals("Neutral")) {
					gMeta.setBanner (banners[count]);
					count++;
				}
				foundFactions.Add (gMeta.faction);
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
			Vector3 pos = randomPosition.asVector3 ();
			GameObject instance = Instantiate (tileChoice, randomPosition.asVector3(), Quaternion.identity, boardHolder) as GameObject;
			BattleGeneralMeta gMeta = instance.GetComponent<BattleGeneralMeta> ();
			if (gMeta != null) {
				gMeta.setPlayer (isPlayer);
				if (isPlayer) {
					gMeta.startTurn ();
					gMeta.startMoving ();
				}
			}
			instance.transform.position = pos;
		}
	}

	private bool obsClick(Point3 click){
		foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Obstacle")) {
			if (click.Equals(new Point3(obs.transform.position))) {
				return true;
			}
		}
		return false;
	}

	public void clicked(Point3 click){
		if (SceneManager.GetActiveScene ().name.Equals("AdventureScene")){
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
				if (!click.Equals(lastClicked.position) && (!steps.walking () || !click.Equals(lastClick)) && !obsClick(click)) {
					steps.destroySteps ();
					Debug.Log ("Moving: " + lastClicked.name);
					StartCoroutine (steps.generateMapv2 (new Point3(lastClicked.position), click, gameManager.getRows (), gameManager.getColumns (), getObstacles(), setPath));
				} else if (steps.walking () && click.Equals(lastClick)) {
	//				BattleConverter.reset ();
					moveAdventurer (lastClicked, path);
					lastClicked = null;
					steps.destroySteps ();
				}
			}
		}
	}

	private List<Point3> getObstacles(){
		List<Point3> obstacles = new List<Point3> ();
		foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
			obstacles.Add (new Point3(unit.transform.position));
		}

		foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Obstacle")) {
			obstacles.Add (new Point3(obs.transform.position));
		}

		foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Entrance")) {
			obstacles.Add (new Point3(obs.transform.position));
		}
		return obstacles;
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
		BattleGeneralMeta player = lastClicked.gameObject.GetComponent<BattleGeneralMeta> ();
		if (enemy != null) {
			Debug.Log ("Enemy is not null!");
			path.Remove (edge);
			if (enemy.tag.Equals ("Unit")) {
				BattleGeneralMeta ai = enemy.GetComponent<BattleGeneralMeta> ();
				BattleConverter.putSave (player, ai, boardHolder);
				BattleConverter.putPrevScene ("AdventureScene");

				Debug.Log (PlayerPrefs.GetString ("battle"));

				attacking = true;
			}
		} else {
			Debug.Log ("Enemy is null!");
		}

		int stepsLeft = player.makeSteps (path.Count);
		if (stepsLeft < 0) {
			attacking = false;
			while (stepsLeft < 0) {
				path.RemoveAt(path.Count - 1);
				stepsLeft += 1;
			}
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
//	public List<Point3> compilePath(List<Point3> path){
//		List<Point3> smoothed = new List<Point3> ();
//		Point3 lastStep = null;
//		bool changeX = false;
//		foreach (Point3 step in path) {
//			bool lastX = changeX;
//			if (lastStep == null) { lastStep = step; }
//			if (lastStep.x != step.x) { changeX = true; }
//			if (lastStep.y != step.y) { changeX = false; }
//			if (lastX != changeX) { smoothed.Add (lastStep); } 
//			lastStep = step;
//		}
//		smoothed.Add (path[path.Count - 1]);
//		return smoothed;
//	}

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
//			Application.LoadLevel ("BattleScene");
			SceneManager.LoadScene ("BattleScene");
		}
	}
}
