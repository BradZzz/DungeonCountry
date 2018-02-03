using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHazard : MonoBehaviour {
	public int damage = 0;
	public bool slow = false;
	public bool teleport = false;
	public int lifetime = 1;

	public void eval(BattleMeta unit){
		if (slow) {
			unit.slowUnit();
		}
		if (damage > 0) {
			unit.isAttackedTrap (damage);
		}
		lifetime--;
		if (lifetime <= 0) {
			gameObject.SetActive (false);
		}
	}
}
