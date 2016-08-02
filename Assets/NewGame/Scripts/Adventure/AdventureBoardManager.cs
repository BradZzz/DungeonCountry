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
	public GameObject footsteps;

	private static Transform boardHolder;
	private Transform lastClicked;
	private Vector3 lastClick;
	private AdventureGameManager gameManager;
	private List<Vector3> gridPositions;
	protected Dictionary<Vector3, Transform> dict;
	private Camera cam;
	private Footsteps steps;
	private List<Vector3> path;

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
		gridPositions = new List <Vector3> ();
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
					gridPositions.Add (new Vector3(x,y,0f));
					Vector3 pos = new Vector3 (x, y, 0f);
					dict[pos] = instance.transform;
				}*/
			}
		}
		formatObjects ();
	}

	private void formatObjects(){

		Vector3 playerPos = new Vector3(), enemyPos = new Vector3();
		bool sharedPrefs = SharedPrefs.playerArmy != null && SharedPrefs.enemyArmy != null;

		if (sharedPrefs) {
			playerPos = SharedPrefs.playerArmy.transform.position;
			Debug.Log ("Player: " + SharedPrefs.playerArmy.name + "Position: " + enemyPos.ToString ());
			enemyPos = SharedPrefs.enemyArmy.transform.position;
			Debug.Log ("Enemy: " + SharedPrefs.enemyArmy.name + "Position: " + enemyPos.ToString ());
		} else {
			Debug.Log("** Warning ** Shared prefs invalid");
		}

		dict = new Dictionary<Vector3, Transform> ();
		foreach(Transform item in boardHolder) {
			Vector3 pos = item.position;
			if (item.name.Contains ("Floor") && pos.x > -1 && pos.y > -1 && pos.x < gameManager.getColumns () && pos.y < gameManager.getRows ()) {
				gridPositions.Add (pos);
				dict [pos] = item;
			} 
			if (sharedPrefs && item.tag.Equals ("Unit")) {
				BattleGeneralMeta meta = null;
				if (item.transform.position.Equals(playerPos)) {
					//(item.transform.position.Equals(playerPos) || item.transform.position.Equals(enemyPos))
					meta = SharedPrefs.playerArmy.gameObject.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
				} else if (item.transform.position.Equals(enemyPos)) {
					meta = SharedPrefs.enemyArmy.gameObject.GetComponent( typeof(BattleGeneralMeta) ) as BattleGeneralMeta;
				}
				Debug.Log ("Checking SharedPrefs: " + item.name + " Position: " + item.position);

				if (meta != null && meta.getDefeated ()) {
					item.gameObject.SetActive (false);
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
			foreach (GameObject general in generals) {
				placeGeneral (general);
			}
		}
	}

	private void placeGeneral (GameObject general)
	{
		LayoutObjectAtRandom (new GameObject[]{general}, 1, 1);
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
			Debug.Log ("Laying down, name: " + instance.name + " position: " + instance.transform.position);
			/*if (instance.name.Contains("Rock(Crag)")) {
				Debug.Log ("Found instance name");
				instance.transform.position = new Vector3(instance.transform.position.x, instance.transform.position.y + .15f, instance.transform.position.z);
			}*/
			instance.transform.SetParent (boardHolder);
		}
	}

	public void clicked(Vector3 click){
		Debug.Log ("Clicked: " + click.ToString());
		if (lastClicked == null) {
			foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
				Debug.Log ("UnitPos: " + unit.transform.position.ToString());
				if (inScene(unit.transform.position)) {
					if  (unit.transform.position == click) {
						Debug.Log ("Clicked: " + click.ToString());
						lastClicked = unit.transform;
					}
				}
			}
		} else if (lastClicked != null) { 
			//if (!Coroutines.hasParentVector3 (click)) {
				if (!click.Equals (lastClicked.position) && (!steps.walking () || click != lastClick)) {
					steps.destroySteps ();
					Debug.Log ("Moving: " + lastClicked.name);
					List<Vector3> obstacles = new List<Vector3> ();
					foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
						obstacles.Add (unit.transform.position);
					}

					foreach (Vector3 unit in obstacles) {
						Debug.Log("Unit Position: " + unit);
					}

					path = steps.generateMap (lastClicked.position, click, gameManager.getRows (), gameManager.getColumns (), obstacles);
					if (path != null) {
						steps.createSteps (lastClicked.position, boardHolder, path);
						lastClick = click;
					}
				} else if (steps.walking () && click == lastClick) {
					moveAdventurer (lastClicked, path);
					lastClicked = null;
					steps.destroySteps ();
				}
			//}
		}
	}

	public bool inScene(Vector3 targetPosition){
		//Vector3 screenPoint = cam.WorldToViewportPoint(targetPosition);
		//return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

		return true;
	}

	public void moveAdventurer(Transform lastClicked, List<Vector3> path) {
		//Check to make sure the last step isn't an enemy here
		int prevTotal = path.Count;

		Vector3 edge  = path [path.Count - 1];

		GameObject enemy = Coroutines.findUnitParent (edge);

		if (enemy != null) {
			Debug.Log ("Enemy is not null!");

			path.Remove (edge);

			SharedPrefs.playerArmy = Instantiate (lastClicked.gameObject, lastClicked.position, Quaternion.identity) as GameObject;
			SharedPrefs.playerArmy.SetActive (false);
			SharedPrefs.enemyArmy = Instantiate (enemy, enemy.transform.position, Quaternion.identity) as GameObject;
			SharedPrefs.enemyArmy.SetActive (false);
			Debug.Log ("Player: " + SharedPrefs.playerArmy.name);
			Debug.Log ("Enemy: " + SharedPrefs.enemyArmy.name);
		} else {
			Debug.Log ("Enemy is null!");
		}

		StartCoroutine (step_path (lastClicked, path, 1f, prevTotal != path.Count));
		//If the last step is an enemy, we need to fight it here
	}

	IEnumerator step_path(Transform origin, List<Vector3> path, float speed, bool battle)
	{
		foreach(Vector3 step in path){
			yield return StartCoroutine( smooth_move(origin, step, speed));
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
		while (origin.position != end_pos) { 
			float move = Mathf.Lerp (0,1, (Time.time - startime) * speed);

			Vector3 position = origin.position;

			position.x += (end_pos.x - start_pos.x) * move;
			position.y += (end_pos.y - start_pos.y) * move;

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

			yield return null;
		}
			
		//yield return null;
	}
}
