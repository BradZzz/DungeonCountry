using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine.UI;
using AssemblyCSharp;


public class BattleBoardManager : MonoBehaviour {

	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	private bool isMoving = false;

	private Transform lastClicked;
	private Transform boardHolder;
	private List <Vector3> gridPositions;
	private List <Transform> movePositions; 
	//This is for the sprite color overlays
	private List <Transform> characterPositions; 
	//This is to keep track of the units for quick activition between turns
	private List <Transform> unitPositions; 
	protected Dictionary<Vector2, Transform> dict;
	private BattleArmyManager armyManager;
	private BattleGameManager gameManager;
	public GameObject footsteps;
	private Footsteps steps;

	private bool playersTurn;

	void Awake(){
		lastClicked = null;
		gridPositions = new List <Vector3> ();
		movePositions = new List <Transform> (); 
		characterPositions = new List <Transform> (); 
		gameManager = GetComponent<BattleGameManager>();
		unitPositions = new List<Transform> ();
	}

	void Start(){
		steps = footsteps.GetComponent<Footsteps>();
	}

	void InitialiseList (int tactics)
	{
		gridPositions.Clear ();
		for(int x = gameManager.getColumns() - tactics; x < gameManager.getColumns(); x++)
		{
			for(int y = 0; y < gameManager.getRows(); y++)
			{
				gridPositions.Add (new Vector3(x, y, 0f));
			}
		}
	}
		
	Vector3 RandomPosition ()
	{
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum, bool active, bool playerArmy)
	{
		int objectCount = Random.Range (minimum, maximum+1);
		for(int i = 0; i < objectCount; i++)
		{
			Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
			GameObject instance = Instantiate (tileChoice, randomPosition, Quaternion.identity) as GameObject;
			BattleMeta meta = instance.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			meta.setPlayer (playerArmy);
			meta.setTurn (active);
			instance.transform.SetParent (boardHolder);
		}
	}
		
	public void boardClicked(Transform clickedObject){
		Debug.Log ("parent: " + clickedObject.name + ": " + clickedObject.position.x + "-" + clickedObject.position.y);
		//StartCoroutine (show_actions (clickedObject));
		//show_actions (clickedObject);
		generateWalkRadius(clickedObject);
	}

	public bool charMoving(){
		return lastClicked != null;
	}

	public void generateWalkRadius(Transform clickedObject){
		/*
		  StartCoroutine (steps.generateMapv2 (new Point3(lastClicked.position), click, gameManager.getRows (), gameManager.getColumns (), obstacles, setPath));
		 */
		if (!clickedObject.name.Contains ("Floor") && lastClicked == null) {
			BattleMeta meta = clickedObject.gameObject.GetComponent (typeof(BattleMeta)) as BattleMeta;
			if (meta != null) {
				lastClicked = clickedObject;
				if (meta.getActions() > 0 && meta.getTurn()) {
					StartCoroutine (steps.generateOverflowMapv1 (new Point3 (lastClicked.position), meta.movement, 
						gameManager.getRows (), gameManager.getColumns (), getObstacles(), show_actions));
				} else if (meta.getAttacks() > 0 && meta.getTurn()) {
					show_actions (new List<Point3>());
				}
			}
		}
	}

	public List<Point3> getObstacles(){
		List<Point3> obstacles = new List<Point3> ();
		for (int x = 0; x < gameManager.getColumns (); x++) {
			for (int y = 0; y < gameManager.getRows (); y++) {
				Vector2 pos = new Vector2 (x, y);
				Transform child = dict [pos];
				if (Coroutines.hasParent (child)) {
					obstacles.Add (new Point3 (pos));
				}
			}
		}
		return obstacles;
	}

	public void show_actions(List<Point3> range){
		
		//All the move positions are coming in through the range function now
		foreach (Point3 position in range) {
			Vector2 pos = new Vector2 (position.x, position.y);
			Transform child = dict[pos];
			SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
			sprRend.material.shader = Shader.Find ("Custom/OverlayShaderBlue");
			movePositions.Add(child); 
			Debug.Log ("Child: " + child.ToString());
		}

		BattleMeta meta = lastClicked.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
		for(int x = 0; x < gameManager.getColumns(); x++) {
			for (int y = 0; y < gameManager.getRows(); y++) {
				Vector2 pos = new Vector2 (x, y);
				Transform child = dict[pos];
				if ((x != lastClicked.position.x || y != lastClicked.position.y) && 
					(checkRange(lastClicked.position, pos, meta.range)) && meta.getAttacks() > 0 && meta.getTurn()) {
					bool hasParent = Coroutines.hasParent (child);
					//If the attack radius isn't in the walking radius or there is an enemy in the walking radius
					if (!Coroutines.containsPoint(movePositions,new Point3(child.position)) || hasParent) {
						SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
						sprRend.material.shader = Shader.Find ("Custom/OverlayShaderRed");
						//if (hasParent) {
							characterPositions.Add (child); 
						//}

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
		movePositions.Clear ();
		characterPositions.Clear ();
		lastClicked = null;
	}

	public void checkMovement(int movement, Transform child, Transform hit){
		BattleMeta meta = lastClicked.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
		if (hit.position.x == child.position.x && hit.position.y == child.position.y &&
			Math.Abs (hit.position.x - lastClicked.position.x) + Math.Abs (hit.position.y - lastClicked.position.y) <= movement && meta.getActions() > 0 && meta.getTurn()) {
			Vector3 start = new Vector3 ((float)lastClicked.position.x, (float)lastClicked.position.y, (float)lastClicked.position.z);
			Vector3 end = new Vector3 ((float)hit.position.x, (float)hit.position.y, (float)lastClicked.position.z);
			meta.isMoving ();
			StartCoroutine (smooth_move (lastClicked, end, 1f));
		}
	}

	public void checkAttack(BattleMeta meta, Transform child, Transform hit){
		if (hit.position.x == child.position.x && hit.position.y == child.position.y &&
			Math.Abs (hit.position.x - lastClicked.position.x) + Math.Abs (hit.position.y - lastClicked.position.y) <= meta.range) {

			BattleMeta enemy = hit.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			if (meta.checkAttacks()) {
				meta.isAttacking(enemy);

				if (enemy != null) {
					enemy.isAttacked (meta.getCharStrength());
				}
			}
			checkConditions ();
		}
	}
		
	public void checkConditions(){
		Debug.Log ("checkConditions");
		if (armyManager.iLost (boardHolder)) {
			Debug.Log ("You Lose");
			gameManager.gameOver (false, "You Lose");
		} else if (armyManager.theyLost (boardHolder)) {
			Debug.Log ("You Win!");
			gameManager.gameOver (true, "You Win!");
		} else {
			Debug.Log ("Game Continues");
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
	}
		
	public void setupScene (BattleGeneralMeta general, BattleArmyManager armyManager, Transform board, Dictionary<Vector2, Transform> dict, bool playersTurn)
	{
		this.armyManager = armyManager;
		this.dict = dict;
		this.playersTurn = playersTurn;

		boardHolder = board;

		InitialiseList (general.tactics);

		//foreach (GameObject army in armyManager.getMyArmy()) {
		//	LayoutObjectAtRandom (new GameObject[]{army}, 1, 1);
		//}

		foreach (GameObject army in armyManager.getTheirArmy()) {
			LayoutObjectAtRandom (new GameObject[]{army}, 1, 1, false, false);
		}

		foreach (Transform tile in boardHolder) {
			if (tile.tag.Contains ("Unit")) {
				unitPositions.Add (tile);
			}
		}
	}

	public void activateUnits(){

		Debug.Log ("!Activate Units!");

		checkConditions ();
		playersTurn = !playersTurn;

		List <Transform> aiUnits = new List<Transform>(); 
		foreach(Transform unit in unitPositions){
			BattleMeta meta = unit.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			if (playersTurn) {
				if (meta.getPlayer()) {
					meta.startTurn ();
				} else {
					meta.setTurn (false);
				}
			} else if (!playersTurn) {
				if (meta.getPlayer()) {
					meta.setTurn (false);
				} else {
					meta.startTurn ();
					if (unit.gameObject.activeInHierarchy) {
						aiUnits.Add (unit);
					}
				}
			}
		}
		if (!playersTurn) {
			Debug.Log ("AI turn start");

			BattleAI ai = gameObject.AddComponent<BattleAI> ();
			ai.init (getBoard(), aiUnits, gameManager.getColumns (), gameManager.getRows ());
			//BattleAI ai = new BattleAI (boardHolder, aiUnits);
			ai.moveUnits (activateUnits);
			//activateUnits (true);
		}
	}

	public Transform getBoard(){
		return boardHolder;
	}

	public void UnloadScene(){
		boardHolder.gameObject.SetActive (false);
	}
}
