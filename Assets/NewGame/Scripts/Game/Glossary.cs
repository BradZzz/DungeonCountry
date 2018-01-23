using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glossary : MonoBehaviour {

	public GameObject[] factions;
	public GameObject[] generals;
	public GameObject[] levels;

	private AffiliationMeta[] affiliations;
	private BattleLevels[] stages;
	private BattleGeneralMeta[] leaders;

	public BattleLevels findLevels(string search) {
		BattleLevels.lvltype type = (BattleLevels.lvltype)System.Enum.Parse (typeof(BattleLevels.lvltype), search);
		foreach (BattleLevels stage in stages) {
			if (stage.lvl == type) {
				return stage;
			}
		}
		return stages[0];
	}

	public AffiliationMeta findFaction(string search) {
		foreach (AffiliationMeta aff in affiliations) {
			if (aff.name.Equals(search)) {
				return aff;
			}
		}
		return affiliations[0];
	}

	public BattleGeneralMeta findGeneral(string search) {
		foreach (BattleGeneralMeta lead in leaders) {
			if (lead.name.Equals(search)) {
				return lead;
			}
		}
		return leaders[0];
	}

	public GameObject findGeneralGO(string search) {
		foreach (GameObject gen in generals) {
			BattleGeneralMeta meta = gen.GetComponent<BattleGeneralMeta>();
			if (meta.name.Equals(search)) {
				return gen;
			}
		}
		return generals[0];
	}
		
	// Use this for initialization
	void Start () {
		affiliations = new AffiliationMeta[factions.Length];
		for(int i = 0; i < factions.Length; i++){
			affiliations[i] = factions[i].GetComponent<AffiliationMeta>();
		}

		leaders = new BattleGeneralMeta[generals.Length];
		for(int i = 0; i < generals.Length; i++){
			leaders[i] = generals[i].GetComponent<BattleGeneralMeta>();
		}

		stages = new BattleLevels[levels.Length];
		for(int i = 0; i < levels.Length; i++){
			stages[i] = levels[i].GetComponent<BattleLevels>();
		}
	}
}
