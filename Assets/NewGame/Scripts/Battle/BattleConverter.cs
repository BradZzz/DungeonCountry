using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleConverter : MonoBehaviour {

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
		PlayerPrefs.SetString ("battle", json);
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

		string json = JsonHelper.ToJson(battle);
		PlayerPrefs.SetString ("battle", json);
		Debug.Log ("json: " + json);
	}


	public void putSave(BattleGeneralMeta player, BattleGeneralMeta ai){
		BattleGeneralMeta playerGenMeta = player.GetComponent<BattleGeneralMeta> ();
		BattleGeneralMeta aiGenMeta = ai.GetComponent<BattleGeneralMeta> ();

		BattleSerializeable[] battle = new BattleSerializeable[2];
		battle [0] = serializeGeneral (playerGenMeta);
		battle [1] = serializeGeneral (aiGenMeta);

		string json = JsonHelper.ToJson(battle);
		PlayerPrefs.SetString ("battle", json);

		Debug.Log("before: " + json);
	}

	public static GameObject[] getSave(Glossary glossary){
		string newInfo = PlayerPrefs.GetString ("battle");
		BattleSerializeable[] thisBattle = JsonHelper.FromJson<BattleSerializeable>(newInfo);
		return new GameObject[]{ deserializeGeneral(thisBattle [0], glossary), deserializeGeneral(thisBattle [1], glossary) };
	}

	public static string getSaveWorld(){
		string newInfo = PlayerPrefs.GetString ("battle");
		BattleSerializeable[] thisBattle = JsonHelper.FromJson<BattleSerializeable>(newInfo);
		return thisBattle[0].level;
	}

	public static GameObject deserializeGeneral(BattleSerializeable battle, Glossary glossary){

		GameObject general = null;
		BattleGeneralMeta GenMeta = null;
		AffiliationMeta GenAff = null; 

		BattleSerializeable btl = battle;
		general = glossary.findGeneralGO (btl.name);
		GenMeta = general.GetComponent<BattleGeneralMeta>();
		GenAff = glossary.findFaction (GenMeta.faction);
		GenMeta.setResources (new BattleGeneralResources (1, new List<GameObject>()));

		BattleSerializeableArmy[] army = JsonHelper.FromJson<BattleSerializeableArmy> (btl.army);
		foreach (BattleSerializeableArmy arm in army) {
			for (int j = 0; j < GenAff.units.Length; j++) {
				if (GenAff.units[j].name.Equals(arm.name)) {
					GenMeta.getResources ().addUnit (GenAff.units [j], arm.qty);
				}
			}
		}
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
		BattleSerializeableArmy[] army = new BattleSerializeableArmy[general.army.Count];
		for (int i =0; i < general.army.Count; i++){
			BattleMeta armMeta = general.army[i].GetComponent<BattleMeta> ();
			army[i] = new BattleSerializeableArmy ();
			army[i].name = general.army[i].name;
			army[i].qty = armMeta.getLives();
		}
		battle.army = JsonHelper.ToJson(army);
		return battle;
	}
}
