using UnityEngine;
using System.Collections;

public class BattleActions {

	private int maxActions, maxAttacks;
	private int thisActions, thisAttacks;
	private bool isTurn;

	public BattleActions (int maxActions, int maxAttacks, bool isTurn) {
		this.maxActions = maxActions;
		this.maxAttacks = maxAttacks;
		thisActions = maxActions;
		thisAttacks = maxAttacks;
		this.isTurn = isTurn;
	}

	public void startTurn(){
		thisActions = maxActions;
		thisAttacks = maxAttacks;
		isTurn = true;
	}

	public bool checkTurn(){
		return isTurn;
	}

	private bool checkEndTurn(){
		if (thisAttacks <= 0 && thisActions <= 0) {
			Debug.Log ("Ending turn...");
			isTurn = false;
			return true;
		}
		return false;
	}

	public bool takeAttack(int cost){
		if (thisAttacks - cost >= 0) {
			thisAttacks -= cost;
			checkEndTurn ();
			return true;
		} else {
			return false;
		}
	}

	public int getAtacks(){
		return thisAttacks;
	}

	public bool takeAction(int cost){

		Debug.Log ("Taking Action");

		if (thisActions - cost >= 0) {
			thisActions -= cost;
			checkEndTurn();
			return true;
		} else {
			return false;
		}
	}

	public int getActions(){
		return thisActions;
	}


}
