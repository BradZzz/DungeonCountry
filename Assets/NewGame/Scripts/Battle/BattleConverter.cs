using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleConverter : DataStoreConverter {

	public static void putSaveBattleObject(BattleObject game){
		BattleSerializeable[] battle = new BattleSerializeable[2];
		battle[0] = new BattleSerializeable ();
		battle[0].level = game.level;
		battle[0].name = game.player1;
		battle[0].stats = JsonUtility.ToJson(game.stats1);
		battle[0].army = JsonHelper.ToJson(game.army1);
		BattleSerializeableResource[] blank = new BattleSerializeableResource[0];
		battle[0].resources = JsonHelper.ToJson (blank);

		battle[1] = new BattleSerializeable ();
		battle[1].level = game.level;
		battle[1].name = game.player2;
		battle[1].stats = JsonUtility.ToJson(game.stats2);
		battle[1].army = JsonHelper.ToJson(game.army2);
		battle[1].resources = JsonHelper.ToJson (blank);

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
		PlayerPrefs.SetString ("battle", json);
		Debug.Log ("json: " + json);
	}


	public static void putSave(BattleGeneralMeta player, BattleGeneralMeta ai, Transform board){
		if (board != null) {
			processBoard(board);
		}

		BattleGeneralMeta playerGenMeta = player.GetComponent<BattleGeneralMeta> ();
		BattleGeneralMeta aiGenMeta = ai.GetComponent<BattleGeneralMeta> ();

		BattleSerializeable[] battle = new BattleSerializeable[2];
		battle [0] = serializeGeneral (playerGenMeta);
		battle [1] = serializeGeneral (aiGenMeta);
		battle[0].level = "World";
		battle[1].level = "World";

		string json = JsonHelper.ToJson(battle);
		PlayerPrefs.SetString ("battle", json);

		Debug.Log("before: " + json);
	}

	public static void reset(){
		PlayerPrefs.SetString ("battle", "");
	}

	public static void putPrevScene(string prev_scene){
		PlayerPrefs.SetString ("prev_battle_scene", prev_scene);
	}

	public static string prevScene(){
		return PlayerPrefs.GetString ("prev_battle_scene");
	}

	public static bool hasData(){
		return PlayerPrefs.GetString ("battle").Length > 0;
	}

	public static GameObject[] getSave(Glossary glossary){
		string newInfo = PlayerPrefs.GetString ("battle");
		Debug.Log("after: " + newInfo);
		if (newInfo.Length == 0) {
			return null;
		}
		BattleSerializeable[] thisBattle = JsonHelper.FromJson<BattleSerializeable>(newInfo);
		if (thisBattle != null) {
			return new GameObject[] {
				deserializeGeneral (thisBattle [0], glossary),
				deserializeGeneral (thisBattle [1], glossary)
			};
		}
		return null;
	}

	public static string getSaveWorld(){
		string newInfo = PlayerPrefs.GetString ("battle");
		BattleSerializeable[] thisBattle = JsonHelper.FromJson<BattleSerializeable>(newInfo);
		return thisBattle[0].level;
	}
}
