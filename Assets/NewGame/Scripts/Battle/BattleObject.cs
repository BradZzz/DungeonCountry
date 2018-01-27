using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleObject : MonoBehaviour {
	public string player1;
	public BattleSerializeableStats stats1;
	public BattleSerializeableArmy[] army1;

	public string player2;
	public BattleSerializeableStats stats2;
	public BattleSerializeableArmy[] army2;
}
