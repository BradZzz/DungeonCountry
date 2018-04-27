using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGeneralResources {

	[SerializeField]
	private List<GameObject> army;
	private Dictionary<string, int> resources;
	private GeneralAttributes attribs;

	public BattleGeneralResources(){
		army = new List<GameObject> ();
		attribs = new GeneralAttributes ();

		resources = new Dictionary<string, int> ();
		resources.Add ("gold", 1000);
		resources.Add ("ore", 5);
		resources.Add ("wood", 5);
		resources.Add ("ruby", 0);
		resources.Add ("crystal", 0);
		resources.Add ("sapphire", 0);
	}

	public void setArmy(List<GameObject> army){
		this.army = army;
	}

	public List<GameObject> getArmy(){
		return army;
	}

	public void setResources(Dictionary<string, int> resources){
		this.resources = resources;
	}

	public Dictionary<string, int> getResources(){
		return resources;
	}

	public int getResource(string name){
		if (resources.ContainsKey (name)) {
			return resources [name];
		}
		return -1;
	}

	public GeneralAttributes getAttribs(){
		if (attribs == null) {
			attribs = new GeneralAttributes();
		}
		return attribs;
	}

	public bool canPurchase(Dictionary<string, int> cost){
		foreach(KeyValuePair<string, int> entry in cost)
		{
			if (resources[entry.Key] < entry.Value) {
				return false;
			}
		}
		return true;
	}

	public bool makePurchase(Dictionary<string, int> cost){
		foreach(KeyValuePair<string, int> entry in cost)
		{
			if (!useResource (entry.Key, entry.Value)) {
				return false;
			}
		}
		return true;
	}

	public bool setResource(string name, int quantity){
		if (resources.ContainsKey(name)) {
			resources [name] = quantity;
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

	public bool hasSpaceArmy(){
		return army.Count < 6;
	}

	public void addUnit(GameObject unit){
		if (hasSpaceArmy()) {
			army.Add (unit);
		}
	}

	public bool removeUnitAtIndex(int idx){
		if (army.Count > idx) {
			army.RemoveAt (idx);
			return true;
		}
		return false;
	}
}
