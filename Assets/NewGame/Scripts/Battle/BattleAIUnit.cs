using UnityEngine;
using System.Collections;

public class BattleAIUnit : MonoBehaviour {

	private int actions;
	private int attacks;

	public void init(int actions, int attacks){
		this.actions = actions;
		this.attacks = attacks;
	}
}
