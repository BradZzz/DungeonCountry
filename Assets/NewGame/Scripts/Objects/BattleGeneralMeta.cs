using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class BattleGeneralMeta : MonoBehaviour {

	public string name = "none";
	public string description = "none";
	public List<GameObject> army;
	public List<int> entranceUsed;
	public string faction;

	public enum AttributeList {attack , defense , tactics , level , movement, magic};

	public int attack = 1;
	public int defense = 1;
	public int tactics = 2;
	public int level = 1;
	public int movement = 1;
	public int magic = 1;

	private bool isPlayer;
	private BattleGeneralResources resources;
	private bool defeated;
	private Camera cam = null;

	void Awake() {
		//DontDestroyOnLoad(this.gameObject);
		defeated = false;
		entranceUsed = new List<int> ();
		isPlayer = false;
		init ();
		//resources = new BattleGeneralResources (this.GetInstanceID (), army);
	}

	void Start(){
		if (GameObject.Find("Main Camera") != null) {
			cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		}
	}

	private BattleGeneralResources getResource() {
		if (resources == null) {
			BattleGeneralResources bg = gameObject.AddComponent<BattleGeneralResources> ();
			bg.init (this.GetInstanceID (), army);
			resources = bg;
		}
		return resources;
	}

	public void init() {
		resources = getResource ();
	}

	void LateUpdate () 
	{
		if (isPlayer) {
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

	public int getAttribute(BattleGeneralMeta.AttributeList att){
		switch(att){
			case AttributeList.attack:return attack;
			case AttributeList.defense:return defense;
			case AttributeList.tactics:return tactics;
			case AttributeList.movement:return movement;
			case AttributeList.magic:return magic;
			case AttributeList.level:return level;
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
		return resources;
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

	//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
	private void OnTriggerEnter2D (Collider2D other)
	{
		//Check if the tag of the trigger collided with is Exit.
		if (other.tag == "Entrance" && !entranceUsed.Contains (other.GetInstanceID ())) {
			Debug.Log ("Entrance!");
			//SharedPrefs.playerArmy = Instantiate (this.gameObject, this.gameObject.transform.position, Quaternion.identity) as GameObject;
			//SharedPrefs.playerArmy.SetActive (false);

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
					Application.LoadLevel ("DwellingScene");
					entranceUsed.Add (other.GetInstanceID ());
				}

				CastleMeta castle = info.GetComponent<CastleMeta> ();
				if (castle != null) {
					Debug.Log ("Castle name: " + castle.name);
					CastlePrefs.setCastleInfo (resources, castle, this.gameObject.GetInstanceID());
					//Debug.Log ("Castle affiliation: " + castle.castleAffiliation);
					//DwellingPrefs.setPlayerName (gameObject.name);
					CastleConverter.putSave(this);
					Application.LoadLevel ("CastleScene");
				} 
				//return eMeta.GetComponent<DwellingMeta> ();
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
	}
}
