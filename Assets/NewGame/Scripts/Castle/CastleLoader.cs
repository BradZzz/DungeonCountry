using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CastleLoader : MonoBehaviour {

	public GameObject glossary;
	public GameObject castleBtn;

	private Glossary glossy;
	private BattleGeneralMeta player_gen_meta;
	private BattleGeneralMeta castleGeneral;
	private BattleGeneralResources castleResources;
	private CastleMenu menuHolder = null;

	// Use this for initialization
	void Awake () {
		/*
		 * Read converter here:
		 */

		glossy = glossary.GetComponent<Glossary> ();
		GameObject[] generals = CastleConverter.getSave (glossy);
		GameObject castle_general = generals [0];
		GameObject player_general = generals [1];

		string res_id = CastleConverter.getRawPref ("castle_res");
		Debug.Log("Castle Loading: " + res_id);
		player_gen_meta = player_general.GetComponent<BattleGeneralMeta> ();
		castleResources = player_gen_meta.getResources ();
		init (castleResources, CastlePrefs.getCastleMeta (), true);

//		castleGeneral = CastleConverter.getEntrance (res_id, glossy);
//		bool info = DataStoreConverter.checkKey (res_id.ToString());
//
//		init (btlRes.getResources(), CastlePrefs.getCastleMeta (), true);
		menuHolder = castleBtn.GetComponent<CastleMenu>();
//		castleGeneral = CastleConverter.getEntrance (res_id, glossy);
		menuHolder.initVars (this, glossary, castle_general.GetComponent<BattleGeneralMeta>(), CastlePrefs.getCastleMeta (), res_id);
	}

	void Update(){
		if (CastlePrefs.dirty) {
			CastlePrefs.dirty = false;
			init (castleResources, CastlePrefs.getCastleMeta (), false);
			if (menuHolder != null) {
				menuHolder.refresh ();
			}
		}
	}

	public BattleGeneralMeta getBGM(){
		return player_gen_meta;
	}

	void init (BattleGeneralResources gMeta, CastleMeta dMeta, bool reset) {
		//Pull the canvas from the hierarchy
		GameObject canvas = GameObject.Find ("Canvas"); 
		Transform imageP = canvas.transform.Find ("ImagePanel"); 
		Transform unitP = imageP.transform.Find ("UnitPanel"); 

		Transform marketP = imageP.transform.Find ("UnitPanelMarket"); 
		Transform marketPPurchase = imageP.transform.Find ("UnitPanelPurchase"); 
		Transform generalP = imageP.transform.Find ("GeneralPanel"); 

		Transform generalInfoP = generalP.transform.Find ("GeneralInfoPanel"); 
		Transform unitTownP = generalP.transform.Find ("UnitTownPanel"); 

		if (CastlePrefs.showUnitCity) {
			generalInfoP.gameObject.SetActive (false);
			unitTownP.gameObject.SetActive (true);
		} else {
			generalInfoP.gameObject.SetActive (true);
			unitTownP.gameObject.SetActive (false);
		}

		if (reset) {
			marketP.gameObject.SetActive (false);
			marketPPurchase.gameObject.SetActive (false);
		}

		Transform resourceP = imageP.transform.Find ("ResourcePanel"); 

		Transform cityDesc = imageP.transform.Find ("CityDescription"); 
		Transform cityText = cityDesc.transform.Find ("CityText"); 
		cityText.gameObject.GetComponent<Text> ().text = dMeta.castleName;

		Transform cityI = imageP.transform.Find ("CityImage"); 
		cityI.gameObject.GetComponent<Image> ().sprite = dMeta.castleImage;
	
		int count = 0;
		foreach (GameObject unit in dMeta.affiliation.units) {
			count = unit.GetComponent<BattleMeta> ().lvl;
			Transform unitParent = marketP.transform.Find ("Unit" + count + "Parent"); 
			Transform uPanel = unitParent.transform.Find("Unit" + count + "Panel");
			Transform ulvl = uPanel.transform.Find("Unit" + count + "Text");
			ulvl.gameObject.GetComponent<Text> ().text = count.ToString();

			Transform assets = unitParent.transform.Find("Unit" + count + "Assets");

			Transform healthText = assets.transform.Find("HealthText");
			healthText.gameObject.GetComponent<Text> ().text = unit.GetComponent<BattleMeta> ().hp.ToString();
			Transform attackText = assets.transform.Find("AttackText");
			attackText.gameObject.GetComponent<Text> ().text = unit.GetComponent<BattleMeta> ().attack.ToString();
			Transform rangeText = assets.transform.Find("RangeText");
			rangeText.gameObject.GetComponent<Text> ().text = unit.GetComponent<BattleMeta> ().range.ToString();
			Transform actionText = assets.transform.Find("ActionText");
			actionText.gameObject.GetComponent<Text> ().text = unit.GetComponent<BattleMeta> ().movement.ToString();

			Transform imagey = unitParent.transform.Find("Unit" + count);
			imagey.gameObject.GetComponent<Image> ().sprite = unit.GetComponent<SpriteRenderer> ().sprite;
			Debug.Log ("Name: " + unit.name);
		}
		count = 0;

		foreach (GameObject unit in gMeta.getArmy()) {
			if (count < 6) {
				count += 1;

				BattleMeta bMeta = unit.GetComponent<BattleMeta> ();

				Debug.Log ("bMeta: " + bMeta.name);

				Transform unitPanel = unitP.transform.Find ("Unit" + count + "Panel"); 
				unitPanel.gameObject.SetActive (true);
				Transform unitImage = unitPanel.transform.Find ("Unit" + count); 
				Image sprImg = unitImage.gameObject.GetComponent<Image> ();
				sprImg.sprite = unit.GetComponent<SpriteRenderer> ().sprite;

				Transform unitSubPanel = unitPanel.transform.Find ("Q" + count); 
				Transform unitText = unitSubPanel.transform.Find ("Unit" + count + "Text"); 
				unitText.gameObject.GetComponent<Text> ().text = bMeta.getLives().ToString();

				Color temp = sprImg.color;
				if (count == CastlePrefs.toDelete) {
					temp.a=0.5f;
				} else {
					temp.a=1f;
				}
				sprImg.color = temp;
			}

		}

		while (count < 6) {
			count += 1;
			Transform unitPanel = unitP.transform.Find ("Unit" + count + "Panel"); 
			unitPanel.gameObject.SetActive (false);
		}

		Transform sapphireText = resourceP.transform.Find ("SapphireText"); 
		sapphireText.gameObject.GetComponent<Text> ().text = gMeta.getResource ("sapphire").ToString();
		Transform oreText = resourceP.transform.Find ("OreText"); 
		oreText.gameObject.GetComponent<Text> ().text = gMeta.getResource ("ore").ToString();
		Transform goldText = resourceP.transform.Find ("GoldText"); 
		goldText.gameObject.GetComponent<Text> ().text = gMeta.getResource ("gold").ToString();
		Transform woodText = resourceP.transform.Find ("WoodText"); 
		woodText.gameObject.GetComponent<Text> ().text = gMeta.getResource ("wood").ToString();
		Transform rubyText = resourceP.transform.Find ("RubyText"); 
		rubyText.gameObject.GetComponent<Text> ().text = gMeta.getResource ("ruby").ToString();
		Transform crystalText = resourceP.transform.Find ("CrystalText"); 
		crystalText.gameObject.GetComponent<Text> ().text = gMeta.getResource ("crystal").ToString();
	}
}
