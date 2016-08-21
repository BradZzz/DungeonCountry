using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class BattleGeneralMeta : MonoBehaviour {

	public int tactics = 2;
	public string name = "none";
	public string description = "none";
	public List<GameObject> army;
	public List<int> entranceUsed;

	private Attributes attribs;
	private Dictionary<string, int> resources;

	public class Attributes
	{
		private int tactics, attack, defense, intelligence, luck;
		public Attributes(){
			tactics = 2;
			attack = 1;
			defense = 1;
			intelligence = 1;
			luck = 1;
		}
		public int getTactics(){
			return tactics;
		}
		public int getAttack(){
			return attack;
		}
		public int getDefense(){
			return defense;
		}
		public int getIntelligence(){
			return intelligence;
		}
		public int getLuck(){
			return luck;
		}
	}

	/*public class resource{
		private int quantity;
		private string name;
		public resource(string name, int quantity){
			this.name = name;
			this.quantity = quantity;
		}
		public string getName(){ return this.name; }
		public int getQuantity(){ return this.quantity; }
		public void setResource(int value){ this.quantity = value; }
		public bool spendResource(int value){
			if (value > quantity) {
				return false;
			} else {
				quantity -= value;
				return true;
			}
		}
	}*/

	private bool defeated;

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
		defeated = false;
		entranceUsed = new List<int> ();
	}

	void Start(){
		resources = new Dictionary<string, int> ();
		resources.Add ("gold", 0);
		resources.Add ("ore", 0);

		attribs = new Attributes ();
	}

	public Attributes getAttributes(){
		return attribs;
	}

	public int addResource(string name, int quantity){
		return resources[name] += quantity;
	}

	public bool useResource(string name, int quantity){
		if (resources[name] >= quantity) {
			resources [name] -= quantity;
			return true;
		}
		return false;
	}

	public int getResource(string name){
		return resources[name];
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
				DwellingPrefs.setDwellingInfo (eMeta.image, dwell);
				if (dwell != null) {
					Debug.Log ("Dwelling name: " + dwell.name);
					Debug.Log ("Dwelling name: " + dwell.description);
				}
				//return eMeta.GetComponent<DwellingMeta> ();
			} else {
				Debug.Log ("No Dwelling");
			}
				
			DwellingPrefs.setPlayerName (gameObject.name);

			Application.LoadLevel ("DwellingScene");
			entranceUsed.Add (other.GetInstanceID ());
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
