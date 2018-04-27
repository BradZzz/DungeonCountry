using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGeneralResourcesOld : MonoBehaviour {
//
//	private Dictionary<string, int> resources;
//	[SerializeField]
//	private int instanceID;
//	[SerializeField]
//	private List<GameObject> army;
//	private GeneralAttributes attribs;
//	//private LevelUpMeta 
//
//	public BattleGeneralResourcesOld(BattleGeneralResources clone){
//		this.clone(clone);
//	}
//
//	public void init(int instanceID, List<GameObject> army){
//		fill (instanceID, army);
//	}
//
//	public BattleGeneralResourcesOld(int instanceID, List<GameObject> army){
//		fill (instanceID, army);
//	}
//
//	private void fill(int instanceID, List<GameObject> army){
//		this.instanceID = instanceID;
//		this.army = army;
//		initRes ();
//		attribs = new GeneralAttributes();
//	}
//
//	private void initRes(){
//		resources = new Dictionary<string, int> ();
//		resources.Add ("gold", 1000);
//		resources.Add ("ore", 5);
//		resources.Add ("wood", 5);
//		resources.Add ("ruby", 0);
//		resources.Add ("crystal", 0);
//		resources.Add ("sapphire", 0);
//	}
//
//	public void clone(BattleGeneralResources clone){
//		resources = clone.getResources ();
//		instanceID = clone.getInstanceID ();
//		army = clone.getarmy ();
//	}
//
//	public int getInstanceID(){
//		return instanceID;
//	}
//
//	public List<GameObject> getarmy(){
//		return army;
//	}
//
//	public void setarmy(List<GameObject> army){
//		this.army = army;
//	}
//
//	public GeneralAttributes getAttribs(){
//		if (attribs == null) {
//			attribs = new GeneralAttributes();
//		}
//		return attribs;
//	}
//
//	public void setResources(Dictionary<string,int> resources){
//		this.resources = resources;
//	}
//
//	public int setResources(string name, int quantity){
//		if (resources == null) {
//			initRes ();
//		}
//		return resources[name] += quantity;
//	}
//
//	public int getResource(string name){
//		if (resources == null) {
//			initRes ();
//		}
//		return resources[name];
//	}
//
//	public Dictionary<string, int> getResources(){
//		if (resources == null) {
//			initRes ();
//		}
//		return resources;
//	}
//
//	public bool addUnitFill(GameObject unit, int amount){
//		BattleMeta pUnitMeta = unit.GetComponent<BattleMeta> ();
//		if (army.Count < 6) {
//			GameObject instance = Instantiate (unit) as GameObject;
//			BattleMeta meta = instance.GetComponent( typeof(BattleMeta) ) as BattleMeta;
//			meta.setPlayer (true);
//			meta.setLives (amount);
//			meta.setGUI (false);
//			instance.SetActive (false);
//			army.Add (instance);
//			return true;
//		}
//		return false;
//	}
//
//	public bool addUnit(GameObject unit, int amount){
//		//Debug.Log ("Searching for unit");
//		BattleMeta pUnitMeta = unit.GetComponent<BattleMeta> ();
//
//		foreach (GameObject arm in army) {
//			BattleMeta armMeta = arm.GetComponent<BattleMeta> ();
//			if (pUnitMeta.name == armMeta.name) {
//				armMeta.addLives (amount);
//				return true;
//			}
//		}
//
//		if (army.Count < 6) {
//			Debug.Log ("");
//			GameObject instance = Instantiate (unit) as GameObject;
//			BattleMeta meta = instance.GetComponent( typeof(BattleMeta) ) as BattleMeta;
//			meta.setPlayer (true);
//			meta.setLives (amount);
//			meta.setGUI (false);
//			instance.SetActive (false);
//			army.Add (instance);
//			return true;
//		}
//
//		return false;
//	}
//
//	public bool canPurchaseUnit(Dictionary<string, int> cost, GameObject unit){
//
//		foreach(KeyValuePair<string, int> entry in cost)
//		{
//			if (resources[entry.Key] < entry.Value) {
//				return false;
//			}
//		}
//
//		Debug.Log ("Resources good");
//
//		if (addUnit (unit, 1)) {
//			foreach(KeyValuePair<string, int> entry in cost)
//			{
//				resources [entry.Key] -= entry.Value;
//			}
//			return true;
//		} 
//
//		return false;
//	}
//
//	public bool purchaseUnit(Dictionary<string, int> cost, GameObject unit){
//		if (checkCanPurchase (cost, unit)) {
//			foreach (KeyValuePair<string, int> entry in cost) {
//				useResource (entry.Key, entry.Value);
//			}
//			if (army.Count < 6) {
//				bool found = false;
//				foreach (GameObject arm in army) {
//					if (arm.name.Contains (unit.name)) {
//						BattleMeta bMet = arm.GetComponent<BattleMeta> ();
//						bMet.setLives (bMet.getLives () + 1);
//						found = true;
//					}
//				}
//				if (!found) {
//					GameObject instance = Instantiate (unit) as GameObject;
//					BattleMeta meta = instance.GetComponent( typeof(BattleMeta) ) as BattleMeta;
//					meta.setLives (1);
//					instance.SetActive (false);
//					meta.setGUI (false);
//					meta.setPlayer (false);
//					army.Add (instance);
//				}
//			} else {
//				foreach (GameObject arm in army) {
//					if (arm.name.Contains (unit.name)) {
//						BattleMeta bMet = arm.GetComponent<BattleMeta> ();
//						bMet.setLives (bMet.getLives () + 1);
//					}
//				}
//			}
//			return true;
//		} else {
//			return false;
//		}
//	}
//
//	public bool checkCanPurchase(Dictionary<string, int> cost, GameObject unit){
//		//Check to make sure we have enough $ to purchase
//		foreach(KeyValuePair<string, int> entry in cost)
//		{
//			if (resources[entry.Key] < entry.Value) {
//				return false;
//			}
//		}
//
//		//Do we still have empty space in our army?
//		if (army.Count < 6) {
//			return true;
//		} else {
//			//If there isn't empty space, do we have that unit in our army so we can add to it?
//			foreach (GameObject arm in army) {
//				if (arm.name.Equals(unit.name)) {
//					return true;
//				}
//			}
//			return false;
//		}
//	}
//
//	public bool useResource(string name, int quantity){
//		if (resources[name] >= quantity) {
//			resources [name] -= quantity;
//			return true;
//		}
//		return false;
//	}
}
