using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

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

	//[MethodImpl(MethodImplOptions.Synchronized)]
	public void setTurn(bool isTurn){
		this.isTurn = isTurn;
	}

	public void startTurn(){
		thisActions = maxActions;
		thisAttacks = maxAttacks;
		isTurn = true;
	}

	//[MethodImpl(MethodImplOptions.Synchronized)]
	public bool checkTurn(){
		return isTurn;
	}

	//[MethodImpl(MethodImplOptions.Synchronized)]
	private bool checkEndTurn(){
		if (thisAttacks <= 0 && thisActions <= 0) {
			Debug.Log ("Ending turn...");
			isTurn = false;
			return true;
		}
		return false;
	}

	//[MethodImpl(MethodImplOptions.Synchronized)]
	public bool takeAttack(int cost){
		Debug.Log ("Attacks: " + thisAttacks + " Cost: " + cost);
		if (thisAttacks - cost >= 0) {
			thisAttacks -= cost;
			Debug.Log ("New Attacks: " + thisAttacks + " Cost: " + cost);
			checkEndTurn ();
			return true;
		} else {
			return false;
		}
	}

	//[MethodImpl(MethodImplOptions.Synchronized)]
	public int getAttacks(){
		return thisAttacks;
	}

	//[MethodImpl(MethodImplOptions.Synchronized)]
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

	//[MethodImpl(MethodImplOptions.Synchronized)]
	public int getActions(){
		return thisActions;
	}


}
