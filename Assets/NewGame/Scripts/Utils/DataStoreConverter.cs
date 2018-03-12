using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStoreConverter : MonoBehaviour {

	public static void processBoard(Transform board) {
		List<BattleGeneralMeta> gens = new List<BattleGeneralMeta> ();
		foreach (Transform item in board) {
			if (item.tag.Equals("Unit")) {
				BattleGeneralMeta bgm = item.GetComponent<BattleGeneralMeta> ();
				if (bgm != null) {
					gens.Add (bgm);
				}
			}
		}
		putSave (gens.ToArray(), "BoardSave");
	}

	public static void putSave(BattleGeneralMeta[] generals, string saveKey){
		BattleSerializeable[] battle = new BattleSerializeable[generals.Length];
		for (int i = 0; i < generals.Length; i++) {
			BattleGeneralMeta gen = generals[i].GetComponent<BattleGeneralMeta> ();
			battle [i] = serializeGeneral (gen);
			battle [i].level = "World";
		}

		string json = JsonHelper.ToJson(battle);
		PlayerPrefs.SetString (saveKey, json);
		Debug.Log("before: " + json);
	}

	public static GameObject[] getSave(Glossary glossary, string saveKey){
		string newInfo = PlayerPrefs.GetString (saveKey);
		Debug.Log("after: " + newInfo);
		if (newInfo.Length == 0) {
			return null;
		}
		BattleSerializeable[] thisBattle = JsonHelper.FromJson<BattleSerializeable>(newInfo);
		if (thisBattle != null) {
			GameObject[] gens = new GameObject[thisBattle.Length];
			for (int i = 0; i < thisBattle.Length; i++) {
				gens [i] = deserializeGeneral (thisBattle [i], glossary);
			}
			return gens;
		}
		return null;
	}

	public static void reset(string saveKey){
		PlayerPrefs.SetString (saveKey, "");
	}

	public static void updateGeneral(Glossary glossary, string saveKey, BattleGeneralMeta bgMet){
		Debug.Log ("Update General!");
		//		BattleGeneralMeta bgMet = replaceGeneral.GetComponent<BattleGeneralMeta> ();
		BattleSerializeable rpSrz = serializeGeneral (bgMet);
		Debug.Log ("From: " + JsonUtility.ToJson(rpSrz));
		GameObject[] generals = getSave(glossary, saveKey);

		BattleSerializeable[] newGenerals = new BattleSerializeable[generals.Length];
		for (int i = 0; i < generals.Length; i++) {
			BattleGeneralMeta gm = generals[i].GetComponent<BattleGeneralMeta>();
			if (bgMet.name.Equals (gm.name)) {
				newGenerals [i] = rpSrz;
			} else {
				newGenerals [i] = serializeGeneral (gm);
			}
		}
		string json = JsonHelper.ToJson(newGenerals);
		PlayerPrefs.SetString (saveKey, json);
		Debug.Log ("update: " + json);
	}

	public static void updateGeneral(Glossary glossary, string saveKey, BattleGeneralMeta[] bgMet){
		Debug.Log ("Update General!");
		//		BattleGeneralMeta bgMet = replaceGeneral.GetComponent<BattleGeneralMeta> ();
		BattleSerializeable[] rpSrz = new BattleSerializeable[bgMet.Length];
		for(int i =0; i < bgMet.Length; i++){
			rpSrz [i] = serializeGeneral (bgMet [i]);
		}
		//BattleSerializeable rpSrz = serializeGeneral (bgMet);
		Debug.Log ("From: " + JsonUtility.ToJson(rpSrz));
		GameObject[] generals = getSave(glossary, saveKey);
		BattleSerializeable[] newGenerals = new BattleSerializeable[generals.Length];
		for (int i = 0; i < generals.Length; i++) {
			BattleGeneralMeta gm = generals[i].GetComponent<BattleGeneralMeta>();
			bool found = false;
			foreach(BattleSerializeable rps in rpSrz){
				if (rps.name.Equals (gm.name)) {
					newGenerals [i] = rps;
					found = true;
				}
			}
			if (!found) {
				newGenerals [i] = serializeGeneral (gm);
			}
		}
		string json = JsonHelper.ToJson(newGenerals);
		PlayerPrefs.SetString (saveKey, json);
		Debug.Log ("update: " + json);
	}

	public static GameObject deserializeGeneral(BattleSerializeable battle, Glossary glossary){
		GameObject general = null;
		BattleGeneralMeta GenMeta = null;
		//AffiliationMeta GenAff = null; 

		BattleSerializeable btl = battle;
		general = glossary.findGeneralGO (btl.name);
		GenMeta = general.GetComponent<BattleGeneralMeta>();
		GenMeta.init ();
		BattleSerializeableStats thisStats = JsonUtility.FromJson<BattleSerializeableStats>(btl.stats);
		GenMeta.setPlayer (thisStats.isPlayer);

		BattleSerializeableResource[] resources = JsonHelper.FromJson<BattleSerializeableResource> (btl.resources);
		Dictionary<string,int> resMap = new Dictionary<string,int> ();
		foreach (BattleSerializeableResource res in resources) {
			resMap.Add (res.resource,res.qty);
		}
		GenMeta.setResources (resMap);

		List<GameObject> newUnits = new List<GameObject> ();
		BattleSerializeableArmy[] army = JsonHelper.FromJson<BattleSerializeableArmy> (btl.army);
		foreach (BattleSerializeableArmy arm in army) {
			GameObject unit = glossary.findUnit (arm.name.Replace("(Clone)",""));
			GameObject instance = Instantiate (unit) as GameObject;
			instance.SetActive (false);
			BattleMeta bMet = instance.GetComponent<BattleMeta> ();
			bMet.setLives (arm.qty);
			newUnits.Add (instance);
		}
		GenMeta.setArmy (newUnits);

		return general;
	}

	public static BattleSerializeable serializeGeneral(BattleGeneralMeta general){
		BattleSerializeable battle = new BattleSerializeable();
		battle.name = general.name;
		BattleSerializeableStats stats = new BattleSerializeableStats ();
		stats.attack = 1;
		stats.defense = 1;
		stats.speed = 1;
		stats.range = 1;
		stats.isPlayer = general.getPlayer ();
		battle.stats = JsonUtility.ToJson(stats);
		BattleSerializeableResource[] res = new BattleSerializeableResource[general.getResources().getResources().Count];
		int cnt = 0;
		foreach(KeyValuePair<string,int> resStat in general.getResources().getResources())
		{
			res [cnt] = new BattleSerializeableResource ();
			res [cnt].resource = resStat.Key;
			res [cnt].qty = resStat.Value;
			cnt++;
		}
		battle.resources = JsonHelper.ToJson(res);
		BattleSerializeableArmy[] army = new BattleSerializeableArmy[general.getArmy().Count];
		for (int i =0; i < general.getArmy().Count; i++){
			BattleMeta armMeta = general.getArmy()[i].GetComponent<BattleMeta> ();
			army[i] = new BattleSerializeableArmy ();
			army[i].name = general.getArmy()[i].name;
			army[i].qty = armMeta.getLives();
		}
		battle.army = JsonHelper.ToJson(army);
		return battle;
	}
}
