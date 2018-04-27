using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleConverter : DataStoreConverter {

	//Save all heros in here

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


	public static void putSave(BattleGeneralMeta castle, BattleGeneralMeta player, Transform board){

		if (board != null) {
			processBoard(board);
		}

		BattleSerializeable[] battle_ary = new BattleSerializeable[2];

		BattleGeneralMeta cstlGenMeta = castle.GetComponent<BattleGeneralMeta> ();
		BattleSerializeable cstle_ser = new BattleSerializeable();
		cstle_ser = serializeGeneral (cstlGenMeta);
		battle_ary [0] = cstle_ser;

		BattleGeneralMeta playerGenMeta = player.GetComponent<BattleGeneralMeta> ();
		BattleSerializeable ply_ser = new BattleSerializeable();
		ply_ser = serializeGeneral (playerGenMeta);
		battle_ary [1] = ply_ser;

		string json = JsonHelper.ToJson(battle_ary);
		PlayerPrefs.SetString ("castle", json);

		Debug.Log("before: " + json);
	}

//	public static BattleGeneralMeta[] getSaveBGM(Glossary glossary){
//		string newInfo = PlayerPrefs.GetString ("battle");
//		Debug.Log("after: " + newInfo);
//		if (newInfo.Length == 0) {
//			return null;
//		}
//		BattleSerializeable[] thisBattle = JsonHelper.FromJson<BattleSerializeable>(newInfo);
//		if (thisBattle != null) {
//			BattleGeneralMeta[] generals = new BattleGeneralMeta[2];
//			generals [0] = deserializeGeneral (thisBattle [0], glossary).GetComponent<BattleGeneralMeta> ();
//			generals [1] = deserializeGeneral (thisBattle [1], glossary).GetComponent<BattleGeneralMeta> ();
//
//			return generals;
//		}
//		return null;
//	}
//
//	{"Items":[{"level":"World","name":"Strength","stats":"{\"attack\":1,\"defense\":1,\"speed\":1," +
//		"\"range\":1,\"isPlayer\":false}","army":"{\"Items\":[]}","resources":"{\"Items\":[{\"resource\":" +
//		"\"gold\",\"qty\":1000},{\"resource\":\"ore\",\"qty\":5},{\"resource\":\"wood\",\"qty\":5},{\"resource" +
//		"\":\"ruby\",\"qty\":0},{\"resource\":\"crystal\",\"qty\":0},{\"resource\":\"sapphire\",\"qty\":0}]}"},
//		{"level":"World","name":"Strategy","stats":"{\"attack\":1,\"defense\":1,\"speed\":1,\"range\":1,\"isPlayer" +
//			"\":true}","army":"{\"Items\":[{\"name\":\"Human_Peasant(Clone)\",\"qty\":19},{\"name\":\"Human_Rogue(Clone)" +
//			"\",\"qty\":151}]}","resources":"{\"Items\":[{\"resource\":\"gold\",\"qty\":1000},{\"resource\":\"ore\",\"qty\":5}," +
//			"{\"resource\":\"wood\",\"qty\":5},{\"resource\":\"ruby\",\"qty\":0},{\"resource\":\"crystal\",\"qty\":0},{\"resource\":\"sapphire\",\"qty\":0}]}"}]}

	public static GameObject[] getSave(Glossary glossary){
		string newInfo = PlayerPrefs.GetString ("castle");
		Debug.Log("after: " + newInfo);
		if (newInfo.Length == 0) {
			return null;
		}
		BattleSerializeable[] thisBattle = JsonHelper.FromJson<BattleSerializeable>(newInfo);
		if (thisBattle != null) {
			GameObject[] generals = new GameObject[2];
			//castle
			generals [0] = deserializeGeneral (thisBattle [0], glossary);
			//player
			generals [1] = deserializeGeneral (thisBattle [1], glossary);
			return generals;
		}
		return null;
	}

//	public static void saveEntrance(string id, BattleGeneralMeta castleGeneral){
//		BattleSerializeable bGen = DataStoreConverter.serializeGeneral (castleGeneral);
//		DataStoreConverter.putKey (JsonUtility.ToJson (bGen), id);
//	}

//	public static BattleGeneralMeta getEntrance(string id, Glossary glossy){
//		Debug.Log ("Getting Entrance Key: " + id);
//
//		string gen = DataStoreConverter.getKey (id);
//		if (gen.Length > 0) {
//			BattleSerializeable b_gen = JsonUtility.FromJson<BattleSerializeable> (gen);
//			DataStoreConverter.resetKey (id);
//			GameObject general = DataStoreConverter.deserializeGeneral (b_gen, glossy);
//			BattleGeneralMeta bgm = general.GetComponent<BattleGeneralMeta> ();
//			return bgm;
//		}
//		return null;
//	}

	public static void reset(){
		PlayerPrefs.SetString ("castle", "");
		PlayerPrefs.SetString ("tavern", "");
//		PlayerPrefs.SetString ("castle_res", "");
	}

	public static bool hasData(){
		return PlayerPrefs.GetString ("castle").Length > 0;
	}

	public static string getRawPref(string key){
		return PlayerPrefs.GetString (key);
	}

	public static void putTavernGeneral(BattleGeneralMeta[] newGeneral){
		BattleSerializeable[] battle = new BattleSerializeable[newGeneral.Length];
		for (int i = 0; i < newGeneral.Length; i++) {
			battle [i] = serializeGeneral (newGeneral[i]);
			battle[i].level = "World";
		}
		string json = JsonHelper.ToJson(battle);
		PlayerPrefs.SetString ("tavern", json);
	}

	public static GameObject[] getBoughtTavernGenerals(Glossary glossary){
		string tavernInfo = PlayerPrefs.GetString ("tavern");
		if (tavernInfo.Length == 0) {
			return null;
		}
		BattleSerializeable[] thisTavern = JsonHelper.FromJson<BattleSerializeable>(tavernInfo);
		if (thisTavern != null) {
			GameObject[] generals = new GameObject[thisTavern.Length];
			for (int i = 0; i < generals.Length; i++) {
				generals [i] = deserializeGeneral (thisTavern [i], glossary);
			}
			return generals;
		}
		return null;

	}

	public static string getSaveWorld(){
		string newInfo = PlayerPrefs.GetString ("castle");
		BattleSerializeable[] thisBattle = JsonHelper.FromJson<BattleSerializeable>(newInfo);
		return thisBattle[0].level;
	}
}
