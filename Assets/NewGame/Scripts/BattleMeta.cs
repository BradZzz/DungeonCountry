using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleMeta : MonoBehaviour {

	public int movement;
	public int range;
	public int attack;
	public int hp;

	public string name;
	public string description;

	private bool canMove;

	void Awake()
	{
		//movement = 2;
	}

	public bool getMove(){
		return canMove;
	}
}

public class BattleMetaFactory : MonoBehaviour {
	
	void Start()
	{
		GameObject thisGameObject = this.gameObject; //just to show that every Monobehaviour being attached to a GameObject, this line is useless but valid.
		BattleMeta myScript; // declare the reference.
		myScript = thisGameObject.AddComponent( typeof ( BattleMeta ) ) as BattleMeta;
	}
}
