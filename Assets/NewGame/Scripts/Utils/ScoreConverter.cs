using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreConverter : DataStoreConverter {

	public static void putSaveScoreObject(ScoreSerializeableStats gmScore){
		string scores = PlayerPrefs.GetString ("scores");
		Debug.Log (scores);
		if (scores.Length < 5) {
			ScoreSerializeableStats[] scrs = new ScoreSerializeableStats[1];
			scrs [0] = gmScore;
			string json = JsonHelper.ToJson (scrs);
			PlayerPrefs.SetString ("scores", json);
		} else {
			ScoreSerializeableStats[] scrs = JsonHelper.FromJson<ScoreSerializeableStats> (scores);
			List<ScoreSerializeableStats> new_scrs = new List<ScoreSerializeableStats> ();
			bool found = false;
			foreach (ScoreSerializeableStats scr in scrs) {
				if (scr.name.Equals (gmScore.name) && gmScore.score > scr.score) { 
					new_scrs.Add (gmScore);
					found = true;
				} else {
					new_scrs.Add (scr);
				}
			}
			if (!found) {
				new_scrs.Add (gmScore);
			}
			string scr_str = JsonHelper.ToJson (new_scrs.ToArray ());
			PlayerPrefs.SetString ("scores", scr_str);
		}
	}

	public static int getSaveScore(string level){
		string scores = PlayerPrefs.GetString ("scores");
		if (scores.Length > 5) {
			Debug.Log (scores);
			ScoreSerializeableStats[] scrs = JsonHelper.FromJson<ScoreSerializeableStats> (scores);
			foreach (ScoreSerializeableStats scr in scrs) {
				if (scr.name.Equals (level)) { 
					return scr.score;
				}
			}
		}
		return 0;
	}

	public static void setCurrentLvl(string lvl){
		Debug.Log ("Starting: " + lvl);
		PlayerPrefs.SetString ("scores_lvl", lvl);
	}

	public static string getCurrentLvl(){
		return PlayerPrefs.GetString ("scores_lvl");
	}

	public static void reset(){
		PlayerPrefs.SetString ("scores", "");
	}

	public static int computeResults(GameObject unit, bool single){
		if (single) {
			BattleMeta pUnitMeta = unit.GetComponent<BattleMeta> ();
			pUnitMeta.setLives (1);
		}
		List <GameObject> units = new List<GameObject> (){unit};
		return computeResults(units);
	}

	public static int computeResults(GameObject unit){
		List <GameObject> units = new List<GameObject> (){unit};
		return computeResults(units);
	}

	public static int computeResults(List <GameObject> units){
		int score = 0;
		foreach(GameObject unit in units){
			BattleMeta pUnitMeta = unit.GetComponent<BattleMeta> ();
//			float hpScore = pUnitMeta.getCharHp () * .25f < 1 ? 1 : pUnitMeta.getCharHp () * .25f;
//			float strengthScore = pUnitMeta.getCharStrength () * .35f < 1 ? 1 : pUnitMeta.getCharStrength () * .35f;
//			float abilityScore = pUnitMeta.abilities.Length * 1.2f < 1 ? 1 : pUnitMeta.abilities.Length * 1.2f;
			float hpScore = pUnitMeta.getCharHp ();
			float strengthScore = pUnitMeta.getICharStrength ();
			float abilityScore = pUnitMeta.abilities.Length * 3.5f > 0 ? pUnitMeta.abilities.Length * 3.5f : 1;
			score += (int) (pUnitMeta.getLives () * hpScore * strengthScore * abilityScore);
		}
		return score;
	}
}
