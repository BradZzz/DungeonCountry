using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleConverter : MonoBehaviour {

	public static void putSaveBattleObject(BattleObject game){
		BattleSerializeable[] battle = new BattleSerializeable[2];
		battle[0] = new BattleSerializeable ();
		battle[0].level = game.level;
		battle[0].name = game.player1;
		battle[0].stats = JsonUtility.ToJson(game.stats1);
		battle[0].army = JsonHelper.ToJson(game.army1);

		battle[1] = new BattleSerializeable ();
		battle[1].level = game.level;
		battle[1].name = game.player2;
		battle[1].stats = JsonUtility.ToJson(game.stats2);
		battle[1].army = JsonHelper.ToJson(game.army2);

		string json = JsonHelper.ToJson(battle);
		PlayerPrefs.SetString ("castle", json);
		Debug.Log ("json: " + json);
		Debug.Log ("json: " + game.army1);
		Debug.Log ("json: " + game.army2);
	}

	public static void putSaveDemo(){
		BattleSerializeable[] battle = new BattleSerializeable[2];
		battle[0] = new BattleSerializeable ();
		battle[0].name = "Zarlock";
		BattleSerializeableStats stats_1 = new BattleSerializeableStats ();
		stats_1.attack = 1;
		stats_1.defense = 100;
		stats_1.speed = 1;
		stats_1.range = 1;
		battle[0].stats = JsonUtility.ToJson(stats_1);
		BattleSerializeableArmy[] army_1 = new BattleSerializeableArmy[1];
		army_1[0] = new BattleSerializeableArmy ();
		army_1[0].name = "Necropolis_LichLord";
		army_1[0].qty = 200;
		battle[0].army = JsonHelper.ToJson(army_1);
		battle[0].level = "Magical";

		battle[1] = new BattleSerializeable ();
		battle[1].name = "Zarlock";
		BattleSerializeableStats stats_2 = new BattleSerializeableStats ();
		stats_2.attack = 1;
		stats_2.defense = 100;
		stats_2.speed = 1;
		stats_2.range = 1;
		battle[1].stats = JsonUtility.ToJson(stats_1);
		BattleSerializeableArmy[] army_2 = new BattleSerializeableArmy[1];
		army_2[0] = new BattleSerializeableArmy ();
		army_2[0].name = "Necropolis_LichLord";
		army_2[0].qty = 200;
		battle[1].army = JsonHelper.ToJson(army_2);
		battle[1].level = "Magical";

		string json = JsonHelper.ToJson(battle);
		PlayerPrefs.SetString ("castle", json);
		Debug.Log ("json: " + json);
	}


	public static void putSave(BattleGeneralMeta player){
		BattleGeneralMeta playerGenMeta = player.GetComponent<BattleGeneralMeta> ();

		BattleSerializeable battle = new BattleSerializeable();
		battle = serializeGeneral (playerGenMeta);
		battle.level = "World";

		string json = JsonUtility.ToJson(battle);
		PlayerPrefs.SetString ("castle", json);

		Debug.Log("before: " + json);
	}

	public static void reset(){
		PlayerPrefs.SetString ("castle", "");
	}

	public static GameObject getSave(Glossary glossary){
		string newInfo = PlayerPrefs.GetString ("castle");
		Debug.Log("after: " + newInfo);
		if (newInfo.Length == 0) {
			return null;
		}
		BattleSerializeable thisBattle = JsonUtility.FromJson<BattleSerializeable>(newInfo);
		if (thisBattle != null) {
			return deserializeGeneral (thisBattle, glossary);
		}
		return null;
	}

	public static string getSaveWorld(){
		string newInfo = PlayerPrefs.GetString ("castle");
		BattleSerializeable[] thisBattle = JsonHelper.FromJson<BattleSerializeable>(newInfo);
		return thisBattle[0].level;
	}

	public static GameObject deserializeGeneral(BattleSerializeable battle, Glossary glossary){
		GameObject general = null;
		BattleGeneralMeta GenMeta = null;

		BattleSerializeable btl = battle;
		general = glossary.findGeneralGO (btl.name);
		GenMeta = general.GetComponent<BattleGeneralMeta>();
		GenMeta.init ();
		BattleSerializeableResource[] resources = JsonHelper.FromJson<BattleSerializeableResource> (btl.resources);
		foreach (BattleSerializeableResource res in resources) {
			GenMeta.addResource (res.resource,res.qty);
		}

		List<GameObject> newUnits = new List<GameObject> ();
		BattleSerializeableArmy[] army = JsonHelper.FromJson<BattleSerializeableArmy> (btl.army);
		foreach (BattleSerializeableArmy arm in army) {
			GameObject unit = glossary.findUnit (arm.name.Replace("(Clone)",""));
			BattleMeta bMet = unit.GetComponent<BattleMeta> ();
			bMet.setLives (arm.qty);
			newUnits.Add (unit);
		}
		GenMeta.setArmy (newUnits);
		GenMeta.init ();

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
