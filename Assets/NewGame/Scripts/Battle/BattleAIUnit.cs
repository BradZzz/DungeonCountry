using UnityEngine;
using System.Collections;

public class BattleAIUnit : MonoBehaviour {

	private int actions;
	private int attacks;

	public void init(int actions, int attacks){
		this.actions = actions;
		this.attacks = attacks;
	}

	public void decActions(){
		actions--;
	}

	public int getActions(){
		return actions;
	}

	public void decAttacks(){
		attacks--;
	}

	public int getAttacks(){
		return attacks;
	}

	public string print(){
		return "Actions: " + actions + " Attacks: " + attacks;
	}
}
