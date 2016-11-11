using UnityEngine;
using System.Collections;

public class CastlePrefs : MonoBehaviour {

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

	void Start()
	{
		cHolder = new castleHolder ();
	}

	public void setCastleInfo(BattleGeneralMeta bMeta, CastleMeta cMeta){
		cHolder.bMeta = bMeta;
		cHolder.cMeta = cMeta;
	}

	public static BattleGeneralMeta getGeneralMeta(){
		return cHolder.bMeta;
	}

	public static CastleMeta getCastleMeta(){
		return cHolder.cMeta;
	}

	public class castleHolder{
		public BattleGeneralMeta bMeta = null;
		public CastleMeta cMeta = null;
	}
}
