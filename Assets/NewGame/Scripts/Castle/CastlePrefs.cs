using UnityEngine;
using System.Collections;

public class CastlePrefs : MonoBehaviour {

	public static bool dirty = false;
	public static CastlePrefs Instance;
	public static castleHolder cHolder;

	void Awake ()   
	{
		if (Instance == null)
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy (gameObject);
		}
	}

	public static void setCastleInfo(BattleGeneralResources bMeta, CastleMeta cMeta){
		if (cHolder == null) {
			cHolder = new castleHolder ();
		}
		cHolder.bMeta = bMeta;
		cHolder.cMeta = cMeta;
	}

	public static BattleGeneralResources getGeneralMeta(){
		if (cHolder == null) {
			return (new castleHolder ()).bMeta;
		}
		return cHolder.bMeta;
	}

	public static CastleMeta getCastleMeta(){
		if (cHolder == null) {
			return (new castleHolder ()).cMeta;
		}
		return cHolder.cMeta;
	}

	public class castleHolder{
		public BattleGeneralResources bMeta;
		public CastleMeta cMeta;
		public castleHolder() {
			bMeta = null;
			cMeta = null;
		}
	}
}
