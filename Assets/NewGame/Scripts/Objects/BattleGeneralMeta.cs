﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine.SceneManagement;
using System.Linq;

public class BattleGeneralMeta : MonoBehaviour {

	public string name = "none";
	[TextArea(15,20)]
	public string description = "none";
	public List<GameObject> army;
	public List<int> entranceUsed;
	public string faction;

	public enum AttributeList {attack , defense , tactics , level , movement, magic, currMovement};

	public int attack = 1;
	public int defense = 1;
	public int tactics = 2;
	public int level = 1;
	public int movement = 1;
	public int magic = 1;

	[SerializeField]
	private Color banner = Color.clear;

	[SerializeField]
	private bool isPlayer;

	//This field determines if the hero's information will be shown in the adventure bar
	[SerializeField]
	private bool isSelected;

	private int currentMove = 0;
	private bool isTurn = false;
	private bool isMoving = false;
	private BattleGeneralResources resources;
	private bool defeated;
	private Camera cam = null;
	private SpriteOutline outline = null;

	void Awake() {
		//DontDestroyOnLoad(this.gameObject);
		defeated = false;
		entranceUsed = new List<int> ();
		isPlayer = false;
		isSelected = false;
		init ();
		//resources = new BattleGeneralResources (this.GetInstanceID (), army);
	}

	void Start(){
		if (GameObject.Find("Main Camera") != null) {
			cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		}
	}

	public bool isSel(){
		return isSelected;
	}

	public void toggleSelected(bool selected){
		isSelected = selected;
		isMoving = selected;
	}

	public void setBanner(Color banner){
		if (banner != Color.clear) {
			Debug.Log ("New Color: " + banner.ToString());
			this.banner = banner;
			outline = gameObject.GetComponent<SpriteOutline>();
			if (outline != null) {
				outline.setColor (banner);
				outline.enabled = true;
			}
		}
	}

	public Color getBanner(){
		return banner;
	}

	private BattleGeneralResources getResource() {
		if (resources == null) {
			BattleGeneralResources bg = gameObject.GetComponent<BattleGeneralResources> ();

			//bg = gameObject.GetComponent<BattleGeneralResources> ();
			bg.init (this.GetInstanceID (), army);

//			if (gameObject.GetComponent<BattleGeneralResources> () != null) {
//				bg = gameObject.GetComponent<BattleGeneralResources> ();
//				bg.init (this.GetInstanceID (), army);
//			} else {
////				bg = gameObject.AddComponent<BattleGeneralResources> ();
////				bg.init (this.GetInstanceID (), army);
//			}
			resources = bg;
		}
		return resources;
	}

	public void init() {
		resources = getResource ();
	}

	void LateUpdate () 
	{
		if (isTurn && isMoving) {
			if (cam == null && GameObject.Find("Main Camera") != null) {
				cam = GameObject.Find("Main Camera").GetComponent<Camera>();
			}
			Vector3 vec = new Vector3 (this.transform.position.x, this.transform.position.y, -10);
			Vector3 next = vec;
			if (cam != null && cam.isActiveAndEnabled) {
				Vector3 current = cam.transform.position;
				if (next.x != current.x || next.y != current.y) {
					cam.transform.position = vec;
				}
			}
		}
	}

	public void startTurn() {
		currentMove = getAttribute (BattleGeneralMeta.AttributeList.movement);
		isTurn = true;
		isMoving = false;
	}

	public void startMoving() {
		isMoving = true;
	}

	public bool getMoving() {
		return isMoving;
	}

	public bool getTurn() {
		return isTurn;
	}

	public void endTurn() {
		isTurn = false;
		isMoving = false;
	}

	public int getCurrentMoves(){
		return currentMove;
	}

	public int makeSteps(int number) {
		int diff = currentMove - number;
		currentMove = currentMove - number;
		if (currentMove < 0) {
			currentMove = 0;
		}
		return diff;
	}

	public int getAttribute(BattleGeneralMeta.AttributeList att){
		switch(att){
			case AttributeList.attack:return attack;
			case AttributeList.defense:return defense;
			case AttributeList.tactics:return tactics;
			case AttributeList.movement:return movement;
			case AttributeList.magic:return magic;
			case AttributeList.level:return level;
			case AttributeList.currMovement:return currentMove;
			default:return attack;
		}
	}

	public void setAttribute(BattleGeneralMeta.AttributeList att, int value){
		switch(att){
			case AttributeList.attack:
				attack = value;
				break;
			case AttributeList.defense:
				defense = value;
				break;
			case AttributeList.tactics:
				tactics = value;
				break;
			case AttributeList.movement:
				movement = value;
				break;
			case AttributeList.magic:
				magic = value;
				break;
			case AttributeList.level:
				level = value;
				break;
			case AttributeList.currMovement:
				currentMove = value;
				break;
			default:break;
		}
	}

	public void setArmy(List<GameObject> army){
		this.army = army;
		resources.setarmy (army);
	}

	public void addUnit(GameObject unit, int amt){
		resources.addUnitFill(unit, amt);
	}

	public List<GameObject> getArmy(){
		return getResources().getarmy();
	}

	public void setPlayer(bool isPlayer){
		this.isPlayer = isPlayer;
	}

	public bool getPlayer(){
		return isPlayer;
	}

	public BattleGeneralResources getResources(){
		return getResource();
	}

	public void setResources(BattleGeneralResources resources){
		this.resources = resources;
	}

	public void setResources(Dictionary<string,int> resources){
		this.resources.setResources (resources);
	}

	public int addResource(string name, int quantity){
		return resources.setResources (name, quantity);
	}

	public bool useResource(string name, int quantity){
		return resources.useResource (name, quantity);
	}

	public int getResource(string name){
		return resources.getResource(name);
	}

	public BattleGeneralMeta(BattleGeneralMeta general){
		this.tactics = general.tactics;
		this.army = general.army;
	}

	public bool getDefeated(){
		return defeated;
	}

	public void setDefeated(bool defeated){
		this.defeated = defeated;
	}

	public class ArmyStore : MonoBehaviour
	{
		public int score;
		public GameObject unit;
		public ArmyStore(int score, GameObject unit){
			this.score = score;
			this.unit = unit;
		}
	}

	private List<GameObject> pruneArmy(List<GameObject> army, int oppArmyScore){
		// Since there is only a 1.25 score calculation between the attack and victory, 
		// we need to provide some leeway when it comes to which comp unit is discarded
		int chanceQual = Random.Range (0, 10);
		oppArmyScore = (int) (oppArmyScore * (.5 + (chanceQual * .35)));
//
//		List<ArmyStore> armSto = new List<ArmyStore> ();
//		foreach (GameObject unit in army) {
//			BattleMeta bU = unit.GetComponent<BattleMeta> ();
//			armSto.Add (new ArmyStore(ScoreConverter.computeResults(unit,true), unit));
//		}
//		ArmyStore[] aStore = armSto.OrderBy(o => o.score).ToArray();
//
		List<GameObject> newArmy = new List<GameObject> ();
		foreach (GameObject unit in army) {
			BattleMeta bU = unit.GetComponent<BattleMeta> ();
			int lives = bU.getLives ();
			int finalLives = 0;
			int unitScore = ScoreConverter.computeResults(unit,true);
			for (int i = 0; i < lives; i++) {
				if (oppArmyScore > unitScore) {
					oppArmyScore -= unitScore;
				} else {
					finalLives += 1;
				}
			}
			if (finalLives > 0) {
				bU.setLives (finalLives);
				newArmy.Add (unit);
			}
		}
		if (newArmy.Count == 0) {
			BattleMeta bU = army[0].GetComponent<BattleMeta> ();
			bU.setLives (1);
			newArmy.Add (army[0]);
		}

		return newArmy;
	}

	//This function is for when the computer battles itself
	private BattleGeneralMeta returnVictor(BattleGeneralMeta genA, BattleGeneralMeta genB){
		int scoreA = ScoreConverter.computeResults (genA.army);
		int scoreB = ScoreConverter.computeResults (genB.army);
		Debug.Log ("General: " + genA.name + " score: " + scoreA.ToString());
		Debug.Log ("General: " + genB.name + " score: " + scoreB.ToString());
		if (scoreA >= scoreB) {
			genB.setDefeated (true);
			genA.setArmy (pruneArmy(genA.getResources().getarmy(), scoreB));
			return genA;
		} else {
			genA.setDefeated (true);
			genB.setArmy (pruneArmy(genB.getResources().getarmy(), scoreA));
			return genB;
		}
	}

	private void turnOffCastleGUI(){
		foreach (GameObject ents in GameObject.FindGameObjectsWithTag("Entrance"))
		{
			EntranceMeta eMet = ents.GetComponent<EntranceMeta> ();
			if (eMet != null) {
				eMet.hideFlag ();
			}
		}
	}

	//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
	private void OnTriggerEnter2D (Collider2D other)
	{
		//Check if the tag of the trigger collided with is Exit.
		if (getPlayer ()) {
			if (other.tag == "Entrance" && !entranceUsed.Contains (other.GetInstanceID ())) {
				Debug.Log ("Entrance!");

				SharedPrefs.setPlayerName (gameObject.name);
				GameObject board = GameObject.Find ("Board");
				Coroutines.toggleVisibilityTransform (board.transform, false);

				EntranceMeta eMeta = other.gameObject.GetComponent<EntranceMeta> ();
				if (eMeta != null) {
					Debug.Log ("Dwelling name: " + eMeta.name);

					GameObject info = eMeta.entranceInfo;
					DwellingMeta dwell = info.GetComponent<DwellingMeta> ();
					if (dwell != null) {
						DwellingPrefs.setDwellingInfo (eMeta.image, dwell);
						Debug.Log ("Dwelling name: " + dwell.name);
						Debug.Log ("Dwelling description: " + dwell.description);
						DwellingPrefs.setPlayerName (gameObject.name);
						SceneManager.LoadScene ("DwellingScene");
						entranceUsed.Add (other.GetInstanceID ());
					}
					CastleMeta castle = info.GetComponent<CastleMeta> ();
					if (castle != null) {
						eMeta.plantFlag(banner);
						turnOffCastleGUI ();
						Debug.Log ("Castle name: " + castle.name);
						CastlePrefs.setCastleInfo (resources, castle, this.gameObject.GetInstanceID ());
						CastleConverter.putSave (this, board.transform);
						SceneManager.LoadScene ("CastleScene");
					}
				} else {
					Debug.Log ("No Dwelling");
				}
				//other.gameObject.SetActive (false);
			} else if (other.tag == "Resource") {
				ResourceMeta rMeta = other.gameObject.GetComponent<ResourceMeta> ();
				if (rMeta != null) {
					Debug.Log ("Resource Found: " + rMeta.getName ());
					Debug.Log ("Resource value: " + rMeta.getValue ());
					addResource (rMeta.getName (), rMeta.getValue ());
					other.gameObject.SetActive (false);
				} else {
					Debug.Log ("Resource Not Found!");
				}
			} else {
				Debug.Log ("Found: " + other.tag);
			}
		} else if (isMoving) {
			// Do the ai version of whatever interactions we need here
			Debug.Log ("AI Name: " + gameObject.name);
			if (other.tag.Equals ("Unit")) {
				Debug.Log ("Attacking!" + other.name);
				GameObject board = GameObject.Find ("Board");
				BattleGeneralMeta gen = other.GetComponent<BattleGeneralMeta> ();
				if (gen != null && gen.getPlayer() && faction.Equals("Neutral")) {
					BattleConverter.putSave (gen, this, board.transform);
					BattleConverter.putPrevScene ("AdventureScene");
					Coroutines.toggleVisibilityTransform(board.transform,false);
					if (cam != null) {
						cam.GetComponent<AdventureLoader>().adventureGameManager.gameObject.SetActive (false);
					}
					SceneManager.LoadScene ("BattleScene");
				} else if (gen != null && !gen.getPlayer()) {
					BattleGeneralMeta victor = returnVictor (this, gen);
					BattleConverter.putSave (gen, this, board.transform);
					BattleConverter.putPrevScene ("AdventureScene");

					// Reload adventure scene so that the blood shows up for the defeated player
					SceneManager.LoadScene ("AdventureScene");
				}
			} else if (other.tag.Equals ("Entrance")) {
				Debug.Log ("Entering: " + other.name);
				EntranceMeta eMeta = other.gameObject.GetComponent<EntranceMeta> ();
				if (eMeta != null) {
					//For now we are only having the ai visit castles and not dwellings...
					GameObject info = eMeta.entranceInfo;
					CastleMeta castle = info.GetComponent<CastleMeta> ();
					if (castle != null) {
						// Change the castle's banner to the ai's banner
						eMeta.plantFlag(banner);

						Debug.Log ("Castle name: " + castle.name);
						//Right here we need to figure out who we can recruit from the castle army-wise
						GameObject[] castleRecruitables = castle.affiliation.units;
						GameObject glossary = GameObject.Find ("Glossary");
						Glossary glossy = glossary.GetComponent<Glossary> ();
						//While ai still has money. buy the most expensive unit ai can afford
						Debug.Log ("Buying Shit");
						foreach(GameObject faction in glossy.factions){
							AffiliationMeta meta = faction.GetComponent<AffiliationMeta> ();
							if (meta.name.Equals(castle.affiliation.name)) {
								
								//If the ai doesn't have enough space in their army...
								if (getResources().getarmy().Count == 6) {
									List<GameObject> army = getResources ().getarmy ();
									//fire the weakest units and buy better ones
									int lowestScore = ScoreConverter.computeResults(army[0]);
									GameObject weakestUnit = army[0];
									for (int i = 1; i < army.Count; i++) {
										int score = ScoreConverter.computeResults(army[i]);
										if (score < lowestScore) {
											lowestScore = score;
											weakestUnit = army[i];
										}
									}
									//Make sure the computer is recouped for the losses since it's a computer and doesn't know fuck
									Dictionary<string, int> cost = weakestUnit.GetComponent<BattleMeta> ().getResourcesAsDict ();
									int lives = weakestUnit.GetComponent<BattleMeta> ().getLives ();

									//Add the units resources for as many times as the unit has lives
									foreach(KeyValuePair<string,int> res in cost)
									{
										getResources ().setResources (res.Key, res.Value * lives);
									}

									//Remove the unit from the army and reinstantiate
									army.Remove(weakestUnit);
									getResources().setarmy(army);
								}

								SortedDictionary<int, GameObject> sortedMeta = new SortedDictionary<int,GameObject>();
								foreach(GameObject recruit in meta.units) {
									BattleMeta bm = recruit.GetComponent<BattleMeta> ();
									if (!sortedMeta.ContainsKey(bm.lvl)) {
										sortedMeta.Add (bm.lvl, recruit);
									}
								}
								Debug.Log (sortedMeta.Keys.ToString());
								Debug.Log (sortedMeta.Values.ToString());
								foreach (KeyValuePair<int, GameObject> kvp in sortedMeta)
								{
									//textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
									Debug.Log("Key =" + kvp.Key + " Value = " + kvp.Value.name);
								}
								for (int i = sortedMeta.Count; i > 0; i--) {
									Dictionary<string, int> cost = sortedMeta[i].GetComponent<BattleMeta> ().getResourcesAsDict ();
									// Buy as many as you can afford
									while (getResources().purchaseUnit(cost, sortedMeta[i])) {
										// Do nothing. (canPurchaseUnit buys the unit...)
									}
								}
								Debug.Log ("Bought after");
							}
						}
						Debug.Log ("Bought Shit");
					} 
				}
			} else if (other.tag.Equals ("Resource")) {
				ResourceMeta rMeta = other.gameObject.GetComponent<ResourceMeta> ();
				if (rMeta != null) {
					addResource (rMeta.getName (), rMeta.getValue ());
					other.gameObject.SetActive (false);
				}
				Debug.Log ("Picked up resource");
			}
		}
	}
}
