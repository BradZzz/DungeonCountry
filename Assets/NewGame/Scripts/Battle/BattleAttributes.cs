using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAttributes : MonoBehaviour {
	public string name;
	public string description;

	public int atkLVL = 0;
	public int defLVL = 0;

	public bool atk_all = false;
	public bool sap_obs = false;
	public bool sniper = false;
	public bool stop = false;

	public int extra_atk = 0;
	public int extra_act = 0;
	public int extra_rng = 0;

	public string sap_spawn = "";
	public int sap_sludge_radius = 0;
	public int sap_lava_radius = 0;
}
