using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdventureBoardManager : MonoBehaviour {

	public GameObject[] outerWallTiles;
	public GameObject[] innerWallTiles;
	public GameObject[] floorTiles;
	public GameObject footsteps;

	private Transform boardHolder;
	private Transform lastClicked;
	private AdventureGameManager gameManager;
	private List<Vector3> gridPositions;
	protected Dictionary<Vector3, Transform> dict;
	private Camera cam;
	private Footsteps steps;

	void Awake(){
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		steps = footsteps.GetComponent<Footsteps>();
		gridPositions = new List <Vector3> ();
		dict = new Dictionary<Vector3, Transform> ();
	}
		
	private void BoardSetup ()
	{
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;

		for(int x = -1; x <= gameManager.getColumns(); x++)
		{
			for(int y = -1; y <= gameManager.getRows(); y++)
			{
				GameObject toInstantiate = floorTiles[UnityEngine.Random.Range (0,floorTiles.Length)];
				bool outer = y == -1 || x == -1 || y == gameManager.getRows () || x == gameManager.getColumns ();
				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);

				if (outer) {
					Debug.Log ("Got here!");
					toInstantiate = outerWallTiles [UnityEngine.Random.Range (0, outerWallTiles.Length)];
					instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);
				} else {
					gridPositions.Add (new Vector3(x,y,0f));
					Vector3 pos = new Vector3 (x, y, 0f);
					dict[pos] = instance.transform;
				}
			}
		}
	}

	public void setupScene (AdventureGameManager gameManager, GameObject general)
	{
		this.gameManager = gameManager;

		BoardSetup ();
		placeGeneral (general);
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
		} else if (lastClicked != null && !steps.walking()) {
			//generateMap(Vector3 startingPos, Vector3 destination, int rows, int columns, List<Vector3> obstacles)
			List<Vector3> obstacles = new List<Vector3>();
			foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
				obstacles.Add (unit.transform.position);
			}

			List<Vector3> path = steps.generateMap (lastClicked.position, click, gameManager.getRows(), gameManager.getColumns(), obstacles);

			steps.createSteps (boardHolder,path);

			Debug.Log ("Returned: " + path.Count);

			//steps.printList (path);

			//moveAdventurer (click);
			//lastClicked = null;
		} else if (lastClicked != null && steps.walking()) {
			moveAdventurer (click);
			lastClicked = null;
			steps.destroySteps();
		}
	}

	public bool inScene(Vector3 targetPosition){
		Vector3 screenPoint = cam.WorldToViewportPoint(targetPosition);
		return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
	}

	public void moveAdventurer(Vector3 click) {
		StartCoroutine (smooth_move (lastClicked, click, 1f));
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
	}
}
