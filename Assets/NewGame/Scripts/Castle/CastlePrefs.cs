using UnityEngine;
using System.Collections;

public class CastlePrefs : MonoBehaviour {

	public static bool dirty = false;
	public static bool showUnitCity = true;
	public static int toDelete = -1;
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

	public static void setCastleInfo(BattleGeneralResources bMeta, CastleMeta cMeta, int id){
		if (cHolder == null) {
			cHolder = new castleHolder ();
		}
		cHolder.bMeta = bMeta;
		cHolder.cMeta = cMeta;
		cHolder.id = id;
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

	public static int getGeneralID(){
		if (cHolder == null) {
			return (new castleHolder ()).id;
		}
		return cHolder.id;
	}

	public class castleHolder{
		public int id;
		public BattleGeneralResources bMeta;
		public CastleMeta cMeta;
		public castleHolder() {
			id = -1;
			bMeta = null;
			cMeta = null;
		}
	}
}
