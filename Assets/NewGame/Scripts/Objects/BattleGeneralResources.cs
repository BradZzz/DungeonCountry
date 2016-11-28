using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleGeneralResources : MonoBehaviour {

	private Dictionary<string, int> resources;
	private Attributes attribs;
	private int instanceID;
	private List<GameObject> army;

	public BattleGeneralResources(int instanceID, List<GameObject> army){
		this.instanceID = instanceID;
		this.army = army;

		resources = new Dictionary<string, int> ();
		resources.Add ("gold", 0);
		resources.Add ("ore", 0);
		resources.Add ("wood", 0);
		resources.Add ("ruby", 0);
		resources.Add ("crystal", 0);
		resources.Add ("sapphire", 0);

		attribs = new Attributes ();
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

	public int setResources(string name, int quantity){
		return resources[name] += quantity;
	}

	public int getResource(string name){
		return resources[name];
	}

	public bool useResource(string name, int quantity){
		if (resources[name] >= quantity) {
			resources [name] -= quantity;
			return true;
		}
		return false;
	}
}
