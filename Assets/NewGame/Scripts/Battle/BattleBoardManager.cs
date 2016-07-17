using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.
using System.Collections;
using UnityEngine.UI;


public class BattleBoardManager : MonoBehaviour {

	// Using Serializable allows us to embed a class with sub properties in the inspector.
	[Serializable]
	public class Count
	{
		public int minimum;             //Minimum value for our Count class.
		public int maximum;             //Maximum value for our Count class.


		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	private bool isMoving = false;
	public int columns = 10;                                         //Number of columns in our game board.
	public int rows = 10;                                            //Number of rows in our game board.
	public GameObject[] floorTiles;                                 //Array of floor prefabs.

	//public GameObject[] armyTiles;                                 //Array of floor prefabs.
	//public GameObject[] enemyTiles;                                 //Array of floor prefabs.

	private Transform lastClicked;
	private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
	private List <Vector3> gridPositions;   //A list of possible locations to place tiles.
	private List <Transform> movePositions; 
	private List <Transform> characterPositions; 
	private int level;
	protected Dictionary<Vector2, Transform> dict;
	private BattleArmyManager armyManager;
	private BattleGameManager gameManager;
	private Panel panel;
	private BattleGeneralMeta general;
	private int levely;

	void Awake(){
		lastClicked = null;
		gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.
		movePositions = new List <Transform> (); 
		characterPositions = new List <Transform> (); 
		gameManager = GetComponent<BattleGameManager>();
		dict = new Dictionary<Vector2, Transform>();
		levely = 1;
	}

	void Start(){
		//GameObject go = GameObject.Find ("Panel");
		//panel = (Panel) panel.GetComponent(typeof(Panel));
	}

	public Dictionary<Vector2, Transform> getDict(){
		return dict;
	}
		
	void InitialiseList (int level)
	{
		gridPositions.Clear ();
		for(int x = 0; x < getColumns(level); x++)
		{
			for(int y = 0; y < getRows(level); y++)
			{
				gridPositions.Add (new Vector3(x, y, 0f));
			}
		}
	}

	private int getColumns(int level) {
		return columns + level;
	}

	private int getRows(int level) {
		return rows + level;
	}

	//Sets up the outer walls and floor (background) of the game board.
	void BoardSetup (int level)
	{
		Debug.Log ("BoardSetup");

		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;

		for(int x = 0; x < getColumns(level); x++)
		{
			for(int y = 0; y < getRows(level); y++)
			{
				GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
			}
		}
		foreach (Transform child in boardHolder) {
			Vector2 pos = new Vector2 (child.transform.position.x, child.transform.position.y);
			dict[pos] = child;
		}
	}


	//RandomPosition returns a random position from our list gridPositions.
	Vector3 RandomPosition ()
	{
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		int objectCount = Random.Range (minimum, maximum+1);
		for(int i = 0; i < objectCount; i++)
		{
			Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
			GameObject instance = Instantiate (tileChoice, randomPosition, Quaternion.identity) as GameObject;
			//GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
			instance.transform.SetParent (boardHolder);
		}
	}

	public void boardClicked(Transform clickedObject){
		Debug.Log ("parent: " + clickedObject.name + ": " + clickedObject.position.x + "-" + clickedObject.position.y);
		//StartCoroutine (show_actions (clickedObject));
		show_actions (clickedObject);
	}

	public bool hasParent(Transform parent, Transform child){
		foreach (GameObject children in GameObject.FindGameObjectsWithTag("Unit")) {
			if (children.transform.position.x == child.position.x && children.transform.position.y == child.position.y) {
				return true;
			}
		}
		return false;
	}

	public bool charMoving(){
		return lastClicked != null;
	}

	public void show_actions(Transform clickedObject){
		if (!clickedObject.name.Contains("Floor") && lastClicked == null) {
			BattleMeta meta = clickedObject.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			if (meta != null){
				lastClicked = clickedObject;
				for(int x = 0; x < getColumns(level); x++) {
					for (int y = 0; y < getRows(level); y++) {
						if (Math.Abs(clickedObject.position.x - x) + Math.Abs(clickedObject.position.y - y) <= Mathf.Max(meta.movement, meta.range)) {
							Vector2 pos = new Vector2 (x, y);
							Transform child = dict[pos];
							if (meta.movement >= meta.range) {
								if (!hasParent(boardHolder, child) && checkRange(clickedObject.position, pos, meta.movement)) {
									SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
									sprRend.material.shader = Shader.Find ("Custom/OverlayShaderBlue");
									movePositions.Add(child); 
								} else if ((x != clickedObject.position.x || y != clickedObject.position.y) && 
									(checkRange(clickedObject.position, pos, meta.range))) {
									SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
									sprRend.material.shader = Shader.Find ("Custom/OverlayShaderRed");
									characterPositions.Add(child); 
								}
							} else {
								if ((x != clickedObject.position.x || y != clickedObject.position.y) && 
									(checkRange(clickedObject.position, pos, meta.range))) {
									SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
									sprRend.material.shader = Shader.Find ("Custom/OverlayShaderRed");
									if (hasParent (boardHolder, child)) {
										characterPositions.Add (child); 
									} else {
										movePositions.Add(child); 
									}
								}
								if (!hasParent(boardHolder, child) && checkRange(clickedObject.position, pos, meta.movement)) {
									Debug.Log ("Movement: " + meta.movement + " - " + Math.Abs(pos.x - x) + ":" + Math.Abs(pos.y - y));
									SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
									sprRend.material.shader = Shader.Find ("Custom/OverlayShaderBlue");
									movePositions.Add(child); 
								}
							}
						}
					}
				}
			}
		}
	}

	public bool checkRange(Vector2 pos, Vector2 sqr, int range){
		return Math.Abs(pos.x - sqr.x) + Math.Abs(pos.y - sqr.y) <= range;
	}

	public void moveClick(Transform hit){
		BattleMeta meta = lastClicked.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;

		foreach (Transform child in movePositions)
		{
			SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
			sprRend.material.shader = Shader.Find ("Sprites/Default");
			if (meta != null) {
				checkMovement (meta.movement, child, hit);
			}
		}
		foreach (Transform child in characterPositions)
		{
			SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
			sprRend.material.shader = Shader.Find ("Sprites/Default");
			if (meta != null) {
				checkAttack (meta, child, hit);
			}
		}
		//Remove the valid positions from the map
		movePositions.Clear ();
		characterPositions.Clear ();

		//Set the click object to null so we can click again
		lastClicked = null;
	}

	public void checkMovement(int movement, Transform child, Transform hit){
		if (hit.position.x == child.position.x && hit.position.y == child.position.y &&
		    Math.Abs (hit.position.x - lastClicked.position.x) + Math.Abs (hit.position.y - lastClicked.position.y) <= movement) {
			Vector3 start = new Vector3 ((float)lastClicked.position.x, (float)lastClicked.position.y, (float)lastClicked.position.z);
			Vector3 end = new Vector3 ((float)hit.position.x, (float)hit.position.y, (float)lastClicked.position.z);
			StartCoroutine (smooth_move (lastClicked, end, 1f));
		}
	}

	public void checkAttack(BattleMeta meta, Transform child, Transform hit){
		if (hit.position.x == child.position.x && hit.position.y == child.position.y &&
			Math.Abs (hit.position.x - lastClicked.position.x) + Math.Abs (hit.position.y - lastClicked.position.y) <= meta.range) {

			BattleMeta enemy = hit.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			meta.isAttacking(enemy);

			if (enemy != null) {
				enemy.isAttacked (meta.attack);
			}
			checkConditions ();
		}
	}

	//Check the win/lose conditions
	public void checkConditions(){
		if (armyManager.iLost(boardHolder)) {
			Debug.Log ("You Lose");
			gameManager.gameOver ("You Lose");
		} else if (armyManager.theyLost(boardHolder)) {
			Debug.Log ("You Win!");
			gameManager.gameOver ("You Win!");
		}
	}

	IEnumerator smooth_move(Transform origin, Vector3 direction,float speed){
		float startime = Time.time;
		Vector3 start_pos = new Vector3(origin.position.x, origin.position.y, origin.position.z); //Starting position.
		Vector3 end_pos = direction; //Ending position.
		while (origin.position != end_pos/* && ((Time.time - startime)*speed) < 1f*/) { 
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

	//SetupScene initializes our level and calls the previous functions to lay out the game board
	public void SetupScene (int level, BattleArmyManager armyManager)
	{
		this.level = level;
		this.armyManager = armyManager;

		BoardSetup (level);
		InitialiseList (level);
		levely = 2;
		//foreach (GameObject army in armyManager.getMyArmy()) {
		//	LayoutObjectAtRandom (new GameObject[]{army}, 1, 1);
		//}
		//foreach (GameObject army in armyManager.getTheirArmy()) {
		//	LayoutObjectAtRandom (new GameObject[]{army}, 1, 1);
		//}
		//showTacticsMap();
	}

	void showTacticsMap(){
		//boardHolder
	}

	public void panelClicked(GameObject unit, BattleGeneralMeta general){
		Debug.Log ("Panel Clicked");
		Debug.Log ("Board Received Clicked: " + unit.name);
		Debug.Log ("Tactics: " + general.tactics);
		Debug.Log ("Dictionary: " + dict.Count);
		Debug.Log ("Levely: " + levely);
		for(int x = 0; x < general.tactics; x++) {
			for (int y = 0; y < getRows(level); y++) {
				Vector2 pos = new Vector2 (x, y);
				Debug.Log ("Key: " + pos.ToString());
				Transform child = dict[pos];
				SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
				sprRend.material.shader = Shader.Find ("Custom/OverlayShaderOrange");
			}
		}
	}

	public void populateUIPanel(List<GameObject> units){
		//GameObject panel = GameObject.Find ("Panel");

		//GameObject NewObj = new GameObject(); //Create the GameObject
		//Image NewImage = NewObj.AddComponent<Image>(); //Add the Image Component script
		//Debug.Log("Name: " + units[0].name);
		//NewImage.sprite = units[0].GetComponent<SpriteRenderer> ().sprite; //Set the Sprite of the Image Component on the new GameObject
		//NewObj.GetComponent<RectTransform>().SetParent(panel.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
		//NewObj.SetActive(true); //Activate the GameObject

		Debug.Log ("Count: " + units.Count);

		for (int i = 1; i <= units.Count; i++) {
			GameObject unit1 = GameObject.Find ("Unit"+i);
			Image image = unit1.GetComponent<Image> ();
			image.sprite = units[i-1].GetComponent<SpriteRenderer> ().sprite;

			Debug.Log ("Changing: " + units[i-1].name + ":" + image.sprite.name);
		}


		/*foreach (GameObject unit in units) {
			
			GameObject NewObj = new GameObject(); //Create the GameObject
			Image NewImage = NewObj.AddComponent<Image>(); //Add the Image Component script
			NewImage.sprite = unit.GetComponent<SpriteRenderer>().sprite; //Set the Sprite of the Image Component on the new GameObject
			NewObj.GetComponent<RectTransform>().SetParent(panel.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
			NewObj.SetActive(true); //Activate the GameObject
		}*/

		//SpriteRenderer unity = units [0].GetComponent<SpriteRenderer> ();
		//SpriteRenderer ren = units[0].GetComponent<SpriteRenderer>();
		//NewImage.sprite = Sprite.Create (ren.sharedMaterial.mainTexture as Texture2D, new Rect(0, 0, ren.sharedMaterial.mainTexture.width, ren.sharedMaterial.mainTexture.height), new Vector2(0f, 0f)); //Set the Sprite of the Image Component on the new GameObject
		//NewObj.GetComponent<RectTransform>().SetParent(panel.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
		//NewObj.SetActive(true); //Activate the GameObject

		/*GameObject unit1 = GameObject.Find ("Unit1");
		if (unit1 != null) {
			// Loop through each image and set it's Sprite to the other Sprite.
			Image image = unit1.GetComponent<Image> ();
			image.sprite = units[0].GetComponent<SpriteRenderer> ().sprite;
		} else {
			Debug.Log ("Unit1 null");
		}*/
	}

	public void UnloadScene(){
		boardHolder.gameObject.SetActive (false);
	}
}
