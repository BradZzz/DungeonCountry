using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGeneralResources : MonoBehaviour {

	private Dictionary<string, int> resources;
	private int instanceID;
	private List<GameObject> army;
	private GeneralAttributes attribs;
	//private LevelUpMeta 

	public BattleGeneralResources(BattleGeneralResources clone){
		this.clone(clone);
	}

	public void init(int instanceID, List<GameObject> army){
		fill (instanceID, army);
	}

	public BattleGeneralResources(int instanceID, List<GameObject> army){
		fill (instanceID, army);
	}

	private void fill(int instanceID, List<GameObject> army){
		this.instanceID = instanceID;
		this.army = army;
		initRes ();
		attribs = new GeneralAttributes();
	}

	private void initRes(){
		resources = new Dictionary<string, int> ();
		resources.Add ("gold", 0);
		resources.Add ("ore", 0);
		resources.Add ("wood", 0);
		resources.Add ("ruby", 0);
		resources.Add ("crystal", 0);
		resources.Add ("sapphire", 0);
	}

	public void clone(BattleGeneralResources clone){
		resources = clone.getResources ();
		instanceID = clone.getInstanceID ();
		army = clone.getarmy ();
	}

	public int getInstanceID(){
		return instanceID;
	}

	public List<GameObject> getarmy(){
		return army;
	}

	public void setarmy(List<GameObject> army){
		this.army = army;
	}

	public GeneralAttributes getAttribs(){
		if (attribs == null) {
			attribs = new GeneralAttributes();
		}
		return attribs;
	}

	public int setResources(string name, int quantity){
		if (resources == null) {
			initRes ();
		}
		return resources[name] += quantity;
	}

	public int getResource(string name){
		if (resources == null) {
			initRes ();
		}
		return resources[name];
	}

	public Dictionary<string, int> getResources(){
		if (resources == null) {
			initRes ();
		}
		return resources;
	}

	public bool addUnitFill(GameObject unit, int amount){
		BattleMeta pUnitMeta = unit.GetComponent<BattleMeta> ();
		if (army.Count < 6) {
			GameObject instance = Instantiate (unit) as GameObject;
			BattleMeta meta = instance.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			meta.setPlayer (true);
			meta.setLives (amount);
			meta.setGUI (false);
			instance.SetActive (false);
			army.Add (instance);
			return true;
		}
		return false;
	}

	public bool addUnit(GameObject unit, int amount){
		//Debug.Log ("Searching for unit");
		BattleMeta pUnitMeta = unit.GetComponent<BattleMeta> ();

		foreach (GameObject arm in army) {
			BattleMeta armMeta = arm.GetComponent<BattleMeta> ();
			if (pUnitMeta.name == armMeta.name) {
				armMeta.addLives (amount);
				return true;
			}
		}

		if (army.Count < 6) {
//			GameObject instance = Instantiate (unit) as GameObject;
//			BattleMeta meta = instance.GetComponent( typeof(BattleMeta) ) as BattleMeta;
//			meta.setPlayer (true);
//			meta.setLives (amount);
//			meta.setGUI (false);
//			instance.SetActive (false);
//			army.Add (instance);
//			return true;

			GameObject instance = Instantiate (unit) as GameObject;
			BattleMeta meta = unit.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			meta.setPlayer (true);
			meta.setLives (amount);
			meta.setGUI (false);
			unit.SetActive (false);
			army.Add (unit);
			return true;
		}

		return false;
	}

	public bool canPurchaseUnit(Dictionary<string, int> cost, GameObject unit){

		foreach(KeyValuePair<string, int> entry in cost)
		{
			if (resources[entry.Key] < entry.Value) {
				return false;
			}
		}

		Debug.Log ("Resources good");

		if (addUnit (unit, 1)) {
			foreach(KeyValuePair<string, int> entry in cost)
			{
				resources [entry.Key] -= entry.Value;
			}
			return true;
		} 

		return false;
	}

	public bool useResource(string name, int quantity){
		if (resources[name] >= quantity) {
			resources [name] -= quantity;
			return true;
		}
		return false;
	}
}
