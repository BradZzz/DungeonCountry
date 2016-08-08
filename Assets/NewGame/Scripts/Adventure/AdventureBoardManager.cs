using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class AdventureBoardManager : MonoBehaviour {

	public static AdventureBoardManager instance = null;

	public GameObject[] outerWallTiles;
	public GameObject[] innerWallTiles;
	public GameObject[] outerFloorTiles;
	public GameObject[] floorTiles;
	public GameObject[] roadTiles;
	public GameObject[] bloodTiles;
	public GameObject[] castleTiles;
	public GameObject[] foundationTiles;
	public GameObject[] cliffTiles;
	public GameObject[] waterTiles;
	public GameObject footsteps;

	private static Transform boardHolder;
	private Transform lastClicked;
	private Point3 lastClick;
	private AdventureGameManager gameManager;
	private List<Point3> gridPositions;
	protected Dictionary<Point3, Transform> dict;
	private Camera cam;
	private Footsteps steps;
	private List<Point3> path;

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
		gridPositions = new List <Point3> ();
	}
		
	private void BoardSetup ()
	{
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;
		DontDestroyOnLoad (boardHolder);

		for(int x = -1; x <= gameManager.getColumns(); x++)
		{
			for(int y = -1; y <= gameManager.getRows(); y++)
			{
				bool outer = y == -1 || x == -1 || y == gameManager.getRows () || x == gameManager.getColumns ();
				GameObject toInstantiate;
				if (outer) {
					toInstantiate = outerFloorTiles [UnityEngine.Random.Range (0, outerFloorTiles.Length)];
				} else {
					toInstantiate = floorTiles [UnityEngine.Random.Range (0, floorTiles.Length)];
				}

				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);

				if (outer) {
					//Debug.Log ("Got here!");
					toInstantiate = outerWallTiles [UnityEngine.Random.Range (0, outerWallTiles.Length)];
					instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);
				} /*else {
					gridPositions.Add (new Point3(x,y,0f));
					Point3 pos = new Point3 (x, y, 0f);
					dict[pos] = instance.transform;
				}*/
			}
		}
		formatObjects ();
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

		dict = new Dictionary<Point3, Transform> ();
		foreach(Transform item in boardHolder) {
			Point3 pos = new Point3(item.position);
			if (item.name.Contains ("Floor") && pos.x > -1 && pos.y > -1 && pos.x < gameManager.getColumns () && pos.y < gameManager.getRows ()) {
				gridPositions.Add (pos);
				dict [pos] = item;
			} 
			if (sharedPrefs && item.tag.Equals ("Unit")) {
				BattleGeneralMeta meta = null;
				if (item.position.Equals(playerPos)) {
					//(item.transform.position.Equals(playerPos) || item.transform.position.Equals(enemyPos))
					meta = SharedPrefs.playerArmy.gameObject.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
				} else if (item.position.Equals(enemyPos)) {
					meta = SharedPrefs.enemyArmy.gameObject.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
				}
				if (meta != null && meta.getDefeated ()) {
					item.gameObject.SetActive (false);

					//Add blood splatter where hero was killed
					GameObject tileChoice = bloodTiles[UnityEngine.Random.Range (0, bloodTiles.Length)];
					GameObject instance = Instantiate (tileChoice, item.position, Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);
				} else {
					Debug.Log ("Meta is null");
				}
			}
		}
	}

	public void setupScene (AdventureGameManager gameManager, GameObject[] generals)
	{

		Start ();

		this.gameManager = gameManager;

		if (boardHolder != null) {
			Debug.Log ("Refreshing board");
			boardHolder.gameObject.SetActive (true);
			formatObjects ();
		} else {
			Debug.Log ("Creating board");
			BoardSetup ();
			constructRiverMap(TerrainGeneratorV2.generateMap (new Point3 (gameManager.getColumns (), gameManager.getRows (), 0)));
			foreach (GameObject general in generals) {
				placeGeneral (general);
			}
		}
	}

	private void constructRiverMap(int[,] map){
		//Find empty at top
		//Find empty at bottom
		//Remove every other road

		int width = map.GetLength (0);
		int height = map.GetLength (1);

		List<Point3> bottom = new List<Point3> ();
		List<Point3> tops = new List<Point3> ();
		List<Point3> obstacles = new List<Point3> ();

		Point3 currentPos = new Point3(-1,0,0);

		for (int x = 1; x < width -1; x++) {
			if (map[x - 1,0] == 0 && map[x,0] == 0 && map[x + 1,0] == 0) {
				currentPos = new Point3(x,0,0);
				bottom.Add (currentPos);
			}
			if (map[x - 1,height - 1] == 0 && map[x,height - 1] == 0 && map[x + 1,height - 1] == 0) {
				currentPos = new Point3(x,height - 1,0);
				tops.Add (currentPos);
			}
		}

		Debug.Log ("Bottom: " + bottom.Count);
		Debug.Log ("Tops: " + tops.Count);

		Coroutines.ShuffleArray (bottom);
		Coroutines.ShuffleArray (tops);

		bool switchy = false;

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if ((map[x,y] != 0 && map[x,y] != 2) || (x == 0 || y == 0 || x == width - 1 || y == height - 1)) {
					obstacles.Add (new Point3 (x, y, 0));
				} else if (map[x,y] == 2) {
					if (!solid (map, new Point3 (x, y, 0))) {
						switchy = !switchy;
						if (switchy) {
							obstacles.Add (new Point3 (x, y, 0));
						}
					} else {
						obstacles.Add (new Point3 (x, y, 0));
					}
				}
			}
		}

		if (bottom.Count == 0 || tops.Count == 0) {
			deployTerrainSprites (new List<Point3> (), map);
		} else {
			StartCoroutine (steps.generateMapv2 (bottom[0], tops[0], height, width, obstacles, map, deployTerrainSprites));
		}
	}

	private bool solid (int[,] map, Point3 position){
		if (position.awayFromEdge(map) && position.crowded(map, 0, 3)){
			return true;
		}
		return false;
	}

	private void deployTerrainSprites (List<Point3> path, int[,] map){

		foreach(Point3 water in path){
			if (map [water.x, water.y] == 0) {
				map [water.x, water.y] = 3;
			}
		}

		for (int y = 0; y < map.GetLength(1); y++) {
			for (int x = 0; x < map.GetLength(0); x++) {
				Point3 pos = new Point3 (x,y,0);

				if (map[x,y] != 0) {
					gridPositions.Remove (pos);
				}

				//Wall
				if (map[x,y] == 1) {
					GameObject tileChoice = innerWallTiles[UnityEngine.Random.Range (0, innerWallTiles.Length)];
					GameObject instance = Instantiate (tileChoice, pos.asVector3(), Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);
					gridPositions.Remove (pos);
				}
				//Road
				if (map[x,y] == 2) {
					GameObject tileChoice = roadTiles[UnityEngine.Random.Range (0, roadTiles.Length)];
					GameObject instance = Instantiate (tileChoice, pos.asVector3(), Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);
				}
				//water
				if (map[x,y] == 3) {
					GameObject tileChoice = waterTiles[UnityEngine.Random.Range (0, waterTiles.Length)];
					GameObject instance = Instantiate (tileChoice, pos.asVector3(), Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);
				}

				//Castle
				if (map[x,y] == 11 || map[x,y] == 12) {
					GameObject tileChoice = foundationTiles[UnityEngine.Random.Range (0, foundationTiles.Length)];
					GameObject instance = Instantiate (tileChoice, pos.asVector3(), Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);

					if (map[x,y] == 11) {
						Vector3 vect = pos.asVector3 ();
						vect.x -= .5f;
						vect.y -= .5f;
						tileChoice = castleTiles[UnityEngine.Random.Range (0, castleTiles.Length)];
						instance = Instantiate (tileChoice, vect, Quaternion.identity) as GameObject;
						instance.transform.SetParent (boardHolder);
					}
				}
			}
		}
	}

	private void placeGeneral (GameObject general)
	{
		LayoutObjectAtRandom (new GameObject[]{general}, 1, 1);
	}

	Point3 RandomPosition ()
	{
		int randomIndex = UnityEngine.Random.Range (0, gridPositions.Count);
		Point3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		int objectCount = UnityEngine.Random.Range (minimum, maximum+1);
		for(int i = 0; i < objectCount; i++)
		{
			Point3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray[UnityEngine.Random.Range (0, tileArray.Length)];
			GameObject instance = Instantiate (tileChoice, randomPosition.asVector3(), Quaternion.identity) as GameObject;
			Debug.Log ("Laying down, name: " + instance.name + " position: " + instance.transform.position);
			/*if (instance.name.Contains("Rock(Crag)")) {
				Debug.Log ("Found instance name");
				instance.transform.position = new Point3(instance.transform.position.x, instance.transform.position.y + .15f, instance.transform.position.z);
			}*/
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
			//if (!Coroutines.hasParentPoint3 (click)) {
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

					//step_generate
					//StartCoroutine (step_generate(new Point3(lastClicked.position), click, gameManager.getRows (), gameManager.getColumns (), obstacles));

					StartCoroutine (steps.generateMapv2 (new Point3(lastClicked.position), click, gameManager.getRows (), gameManager.getColumns (), obstacles, setPath));

					/*path = steps.generateMap (new Point3(lastClicked.position), new Point3(click), gameManager.getRows (), gameManager.getColumns (), obstacles);
					if (path != null) {
						steps.createSteps (new Point3(lastClicked.position), boardHolder, path);
						lastClick = click;
					} else {
						lastClicked = null;
					}*/
			} else if (steps.walking () && click.Equals(lastClick)) {
					moveAdventurer (lastClicked, path);
					lastClicked = null;
					steps.destroySteps ();
			}
			//}
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

	/*IEnumerator step_generate(Point3 startingPos, Point3 destination, int rows, int columns, List<Point3> obs)
	{
		CoroutineWithData cd = new CoroutineWithData(this, steps.generateMap (startingPos, destination, gameManager.getRows (), gameManager.getColumns (), obs) );
		yield return cd.coroutine;
		//path = cd.result;
		Debug.Log("result is " + cd.result);  //  'success' or 'fail'


		//yield return StartCoroutine( steps.generateMap (startingPos, destination, gameManager.getRows (), gameManager.getColumns (), obs));
		//path = steps.getPath ();
		if (path != null) {
			steps.createSteps (new Point3(lastClicked.position), boardHolder, path);
			lastClick = destination;
		} else {
			lastClicked = null;
		}
		yield return null;
	}*/

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

			if (((Time.time - startime)*speed) > 1f) {
				origin.position = end_pos;
			}

			yield return null;
		}
			
		//yield return null;
	}
}
