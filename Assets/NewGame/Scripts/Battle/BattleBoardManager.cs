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

	//private bool isMoving = false;

	private Transform lastClicked;
	private Transform boardHolder;
	private List <Vector3> gridPositions;
	private List <Transform> movePositions; 
	//This is for the sprite color overlays
	private List <Transform> characterPositions; 
	//This is to keep track of the units for quick activition between turns
	private List <Transform> unitPositions; 
	private List <Transform> obstaclePositions; 
	protected Dictionary<Vector2, Transform> dict;

	private GeneralAttributes aiAttribs;
	private GeneralAttributes playerAttribs;
	private BattleLevels level;
	private BattleArmyManager armyManager;
	private BattleGameManager gameManager;
	public GameObject footsteps;
	private Footsteps steps;
	Transform playerImage, enemyImage;

	private bool playersTurn;

	void Awake(){
		lastClicked = null;
		gridPositions = new List <Vector3> ();
		movePositions = new List <Transform> (); 
		characterPositions = new List <Transform> (); 
		gameManager = GetComponent<BattleGameManager>();
		unitPositions = new List<Transform> ();
		obstaclePositions = new List<Transform> ();
	}

	void Start(){
		steps = footsteps.GetComponent<Footsteps>();

		GameObject playerPanel = GameObject.Find ("PlayerGPanel");
		playerImage = playerPanel.transform;

		GameObject aiPanel = GameObject.Find ("AIGPanel");
		enemyImage = aiPanel.transform;

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

	void LayoutAIArmy (GameObject[] tiles, bool active, bool playerArmy)
	{
		//Here is where we look at our terrain and figure out the best place to start our army

		//Look at what we have in our army and use it to plan accordingly
		int ranged = 0;
		int melee = 0;
		foreach (GameObject tile in tiles) {
			BattleMeta ai = tile.GetComponent<BattleMeta>();
			if (ai.getRange () > 1) {
				ranged++;
			} else {
				melee++;
			}
		}
			
		int top = gameManager.getRows () / 4;
		int bottom = 3 * gameManager.getRows () / 4;
		int middle = Random.Range (top + 1, bottom - 1);
		int front = gameManager.getColumns () - 2;
		int z = (int) tiles [0].transform.position.z;

		Vector3[,] formations;
		switch(Random.Range (0, 3)) {
			case 0:
				formations = new Vector3[3, 2] {{new Vector3(front,top,z),new Vector3(front-1,top,z)},
				{new Vector3(front,middle,z),new Vector3(front-1,middle,z)}, 
				{new Vector3(front,bottom,z),new Vector3(front-1,bottom,z)}};
				break;
			case 1:
				formations = new Vector3[3, 2] {{new Vector3(front,middle - 1,z),new Vector3(front-1,middle-1,z)},
				{new Vector3(front,middle,z),new Vector3(front-1,middle,z)}, 
				{new Vector3(front,middle+1,z),new Vector3(front-1,middle+1,z)}};
				break;
			default:
				formations = new Vector3[3, 2] {{new Vector3(front,middle - 3,z),new Vector3(front-1,middle - 1,z)},
				{new Vector3(front,middle,z),new Vector3(front-1,middle,z)}, 
				{new Vector3(front,middle + 3,z),new Vector3(front-1,middle + 1,z)}};
				break;
		}

		Vector3 pos = RandomPosition ();
		int[] iter = new int[2]{ 0, 0 };

		//Here is where we set the computer bonuses
		foreach(GameObject tile in tiles){
			BattleMeta metaU = tile.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			if (metaU.getRange () > 1) {
				if (iter [1] < 3) {
					pos = formations[iter [1], 1];
					iter [1]++;
				} else {
					pos = formations[iter [0], 0];
					iter [0]++;
				}
			} else {
				if (iter [0] < 3) {
					pos = formations[iter [0], 0];
					iter [0]++;
				} else {
					pos = formations[iter [1], 1];
					iter [1]++;
				}
			}
			GameObject instance = Instantiate (tile, pos, Quaternion.identity) as GameObject;
			instance.SetActive (true);
			BattleMeta meta = instance.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			meta.init ();
			meta.setPlayer (playerArmy);
			meta.setTurn (active);
			meta.setLives (metaU.getLives());
			meta.setGeneralAttributes (aiAttribs);
			instance.transform.SetParent (boardHolder);
		}
	}
		
	public void boardClicked(Transform clickedObject){
		//Debug.Log ("parent: " + clickedObject.name + ": " + clickedObject.position.x + "-" + clickedObject.position.y);
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
					StartCoroutine (steps.generateOverflowMapv1 (new Point3 (lastClicked.position), meta.getMovement(), 
						gameManager.getRows (), gameManager.getColumns (), getObstacles(), show_actions));
				} else if (meta.getAttacks() > 0 && meta.getTurn()) {
					show_actions (new List<Point3>());
				}
			}
		}
	}

	public List<Transform> getObsPos(){
		return obstaclePositions;
	}

	public List<Point3> getObstacles(){
		List<Point3> obstacles = new List<Point3> ();
		for (int x = 0; x < gameManager.getColumns (); x++) {
			for (int y = 0; y < gameManager.getRows (); y++) {
				Vector2 pos = new Vector2 (x, y);
				Transform child = dict [pos];
				if (Coroutines.hasParent (child)) {
					Debug.Log ("Obstacle");
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
			//Debug.Log ("Child: " + child.ToString());
		}

		BattleMeta meta = lastClicked.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
		for(int x = 0; x < gameManager.getColumns(); x++) {
			for (int y = 0; y < gameManager.getRows(); y++) {
				Vector2 pos = new Vector2 (x, y);
				Transform child = dict[pos];
				if ((x != lastClicked.position.x || y != lastClicked.position.y) && 
					(checkRange(lastClicked.position, pos, meta.getRange(), meta.sniper())) && meta.getAttacks() > 0 && meta.getTurn()) {
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

	public bool checkRange(Vector2 pos, Vector2 sqr, int range, bool sniper){
		bool rng = (Math.Abs (pos.x - sqr.x) + Math.Abs (pos.y - sqr.y) <= range);
		return sniper ? rng && ( pos.x == sqr.x || pos.y == sqr.y ) : rng;
	}

	public void moveClick(Transform hit){
		Debug.Log ("moveClick");

		BattleMeta meta = lastClicked.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
		bool moved = false;
		foreach (Transform child in movePositions)
		{
			SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
			sprRend.material.shader = Shader.Find ("Sprites/Default");
			if (meta != null && !moved) {
				if (checkMovement (meta.getMovement(), child, hit)) {
					moved = true;
				}
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

		if (meta.atkAll () && !moved && meta.getAttacks() > 0 && !hit.position.Equals(lastClicked.position)) {
			bool attacked = false;
			foreach (Transform unit in unitPositions){
				Debug.Log ("Unit: " + unit.gameObject.name);
				//bool range = Math.Abs (unit.position.x - lastClicked.position.x) + Math.Abs (unit.position.y - lastClicked.position.y) <= meta.getRange();
				bool range = checkRange(new Vector2(unit.position.x,unit.position.y), 
					new Vector2(lastClicked.position.x,lastClicked.position.y), 
					meta.getRange(), meta.sniper());
				if (range) {
					Debug.Log ("In range");
					BattleMeta enemy = unit.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
					if (enemy != null && !enemy.getPlayer()) {
						meta.isAttacking(enemy, true);
						enemy.isAttacked (meta, aiAttribs, playerAttribs);
						attacked = true;
					}
				}

			}
			if (attacked) {
				meta.takeAttacks (1);
			}
			checkConditions ();
		}

		movePositions.Clear ();
		characterPositions.Clear ();
		lastClicked = null;
	}

	public bool checkMovement(int movement, Transform child, Transform hit){
		BattleMeta meta = lastClicked.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
		if (hit.position.x == child.position.x && hit.position.y == child.position.y &&
			Math.Abs (hit.position.x - lastClicked.position.x) + Math.Abs (hit.position.y - lastClicked.position.y) 
				<= movement && meta.getActions() > 0 && meta.getTurn()) {
			Vector3 end = new Vector3 ((float)hit.position.x, (float)hit.position.y, (float)lastClicked.position.z);
			meta.isMoving ();
			StartCoroutine (smooth_move (lastClicked, end, 1f));
			GameObject parent = Coroutines.findUnitParent (new Point3 (hit.position));
			if (parent != null) {
				BattleHazard hazard = parent.GetComponent<BattleHazard> ();
				if (hazard != null) {
					hazard.eval (meta);
				}
			}
			return true;
		}
		return false;
	}

	public void coverRadius(GameObject tile, Point3 hit, int radius) {
		for (int x = hit.x - radius; x < hit.x + radius + 1; x++){
			for (int y = hit.y - radius; y < hit.y + radius + 1; y++){
				Point3 pnt = new Point3 (x, y, hit.z);
				if (checkAll(pnt)) {
					makeTile (pnt, tile);
				}
			}
		}
	}

	public bool checkAll(Point3 pos) {
		if (pos.x >= 0 && pos.x < gameManager.getColumns () &&
			pos.y >= 0 && pos.y < gameManager.getRows ()) {
			GameObject parent = Coroutines.findUnitParent (pos);
			if (parent == null) {
				return true;
			}
		}
		return false;
	}

	public void makeTile(Point3 pos, GameObject tile) {
		GameObject instance = Instantiate (tile, pos.asVector3(), Quaternion.identity) as GameObject;
		instance.SetActive (true);
		instance.transform.SetParent (boardHolder);
	}

	public void checkAttack(BattleMeta meta, Transform child, Transform hit){
		bool range = Math.Abs (hit.position.x - lastClicked.position.x) + Math.Abs (hit.position.y - lastClicked.position.y) <= meta.getRange();
		bool hitChild = hit.position.x == child.position.x && hit.position.y == child.position.y;

		if (!meta.atkAll() && hitChild && range) {
			Debug.Log ("checkAttack atkSingle");
			BattleMeta enemy = hit.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			Debug.Log (meta.sap());
			if (meta.checkAttacks ()) {
				if (enemy != null) {
					meta.isAttacking (enemy, false);
					enemy.isAttacked (meta, aiAttribs, playerAttribs);
				} else if (meta.sap()) {
					GameObject parent = Coroutines.findUnitParent (new Point3 (hit.position));
					if (parent != null) {
						meta.takeAttacks (1);
						parent.SetActive (false);
						if (meta.sapSludge() > 0) {
							GameObject sludge = level.sludgeTiles [0];
							coverRadius(sludge, new Point3(hit.position.x, hit.position.y, hit.position.z), meta.sapSludge());
						} 
						if (meta.sapLava() > 0) {
							GameObject lava = level.lavaTiles [0];
							coverRadius(lava, new Point3(hit.position.x, hit.position.y, hit.position.z), meta.sapLava());
						} 
						if (!meta.sapSpawn().Equals("")) {
							foreach (GameObject obj in gameManager.getGlossary().factions) {
								AffiliationMeta affiliation = obj.gameObject.GetComponent( typeof(AffiliationMeta) ) as AffiliationMeta;
								foreach (GameObject unit in affiliation.units) {
									if (unit.name.Equals(meta.sapSpawn())) {
										GameObject instance = Instantiate (unit, hit.position, Quaternion.identity) as GameObject;
										instance.SetActive (true);
										BattleMeta summon = instance.GetComponent( typeof(BattleMeta) ) as BattleMeta;
										summon.init ();
										summon.setPlayer (true);
										summon.setTurn (true);
										summon.setLives (meta.getLives());
										summon.setGeneralAttributes (aiAttribs);
										instance.transform.SetParent (boardHolder);
										unitPositions.Add (instance.transform);
									}
								}
							}
						}
					}
				}
			}
			checkConditions ();
		}
	}

	public void addUnit(Transform unit) {
		unitPositions.Add(unit);
	}
		
	public void checkConditions(){
		Debug.Log ("checkConditions");
		if (armyManager.iLost (boardHolder)) {
			Debug.Log ("You Lose");
			gameManager.gameOver (false, "You Lose", armyManager.getResults(boardHolder));
		} else if (armyManager.theyLost (boardHolder)) {
			Debug.Log ("You Win!");
			gameManager.gameOver (true, "You Win!", armyManager.getResults(boardHolder));
		} else {
			//Debug.Log ("Game Continues");
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
		
	public void setupScene (BattleGeneralMeta player, BattleGeneralMeta ai, BattleArmyManager armyManager, 
		Transform board, Dictionary<Vector2, Transform> dict, bool playersTurn, BattleLevels level)
	{
		this.armyManager = armyManager;
		this.dict = dict;
		this.playersTurn = playersTurn;
		this.level = level;
		boardHolder = board;
		InitialiseList (ai.tactics);

		playerAttribs = player.getResources ().getAttribs ();
		aiAttribs = ai.getResources ().getAttribs ();

		LayoutAIArmy (armyManager.getTheirArmy().ToArray(), false, false);

		foreach (Transform tile in boardHolder) {
			if (tile.tag.Contains ("Unit")) {
				unitPositions.Add (tile);
			} else if (tile.tag.Contains ("Obstacle")) {
				obstaclePositions.Add (tile);
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

				switchTurn (true);

				if (meta.getPlayer()) {
					meta.startTurn ();
				} else {
					meta.setTurn (false);
				}
			} else if (!playersTurn) {
				switchTurn (false);
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
			//Debug.Log ("AI turn start");
			BattleAI ai = gameObject.AddComponent<BattleAI> ();
			ai.init (getBoard(), aiUnits, gameManager.getColumns (), gameManager.getRows (), this, gameManager, aiAttribs, playerAttribs);
			ai.moveUnits (activateUnits);
		}
	}

	public void switchTurn(bool playersTurn){
		if (playersTurn){
			Outline pimage = playerImage.gameObject.GetComponent<Outline> ();
			Color c = pimage.effectColor;
			c.a = 1;
			pimage.effectColor = c;

			Outline eimage = enemyImage.gameObject.GetComponent<Outline> ();
			c = eimage.effectColor;
			c.a = .1f;
			eimage.effectColor = c;
		} else {
			Outline pimage = playerImage.gameObject.GetComponent<Outline> ();
			Color c = pimage.effectColor;
			c.a = .1f;
			pimage.effectColor = c;

			Outline eimage = enemyImage.gameObject.GetComponent<Outline> ();
			c = eimage.effectColor;
			c.a = 1;
			eimage.effectColor = c;
		}
	}

	public Transform getBoard(){
		return boardHolder;
	}

	public void UnloadScene(){
		boardHolder.gameObject.SetActive (false);
	}
}
