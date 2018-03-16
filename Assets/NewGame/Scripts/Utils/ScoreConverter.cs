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

	public static int computeResults(GameObject unit){
		List <GameObject> units = new List<GameObject> (){unit};
		return computeResults(units);
	}

	public static int computeResults(List <GameObject> units){
		int score = 0;
		foreach(GameObject unit in units){
			BattleMeta pUnitMeta = unit.GetComponent<BattleMeta> ();
			score += (int) (pUnitMeta.getLives () * (pUnitMeta.getCharHp () * .05) * (pUnitMeta.getCharStrength () * .2) * (pUnitMeta.abilities.Length * .55));
		}
		return score;
	}
}
