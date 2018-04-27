using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CastleMenu : MonoBehaviour {
	private Glossary glossary = null;
	//private BattleGeneralResources bgMeta = null;
	private BattleGeneralResources castleResources = null;
	private CastleMeta cMeta = null;

	private GameObject imageP = null;
	private GameObject unitPMarket = null;
	private GameObject unitPPurchase = null;
	private GameObject unitModal = null;

	private GameObject townSelection = null;
	private GameObject upgradePurchase = null;
	private GameObject militiaPanel = null;

	private GameObject tavernHire = null;
	private GameObject tavernPanelDesc = null;
	private GameObject tavernNxtPanelDesc = null;
	private GameObject heroTavernModal = null;
	private GameObject[] tavernRoster = new GameObject[6];
	private int tavernSelected = -1;

	private CastleMeta dMeta = null;
	private BattleGeneralResources gMeta = null;
	private BattleGeneralMeta genMeta = null;
	private BattleGeneralMeta castleGenMeta = null;
	private CastleLoader cLoader = null;
	private int unitSelected = 1;
	private string res_id = "";
	GameObject purchaseUnit = null;

	private BattleGeneralMeta getGeneral(GameObject glossary){
		if (gMeta == null) {
			genMeta = cLoader.getBGM ();
			gMeta = cLoader.getBGM ().getResources();
		}
		return genMeta;
	}

	public void initVars(CastleLoader cLoader, GameObject glossary, BattleGeneralMeta castleGenMeta, CastleMeta cMeta, string res_id){
		this.glossary = glossary.GetComponent<Glossary>();

		// This is the castles general that acts as the holder for the miltia
		this.castleGenMeta = castleGenMeta;
		this.castleResources = castleGenMeta.getResources();
		this.cLoader = cLoader;

		this.cMeta = cMeta;
		this.res_id = res_id;

		if (imageP ==  null){
			imageP = GameObject.Find ("ImagePanel");
		}
		if (upgradePurchase == null) {
			upgradePurchase = imageP.transform.Find ("CastleUpgrades").gameObject; 
			upgradePurchase.SetActive (false);
		}
		if (townSelection == null) {
			townSelection = imageP.transform.Find ("TownSelection").gameObject; 
			townSelection.SetActive (false);
		}
		if (tavernHire == null) {
			tavernHire = imageP.transform.Find ("TavernSelection").gameObject; 
			tavernPanelDesc = tavernHire.transform.Find ("PanelDesc").gameObject; 
			tavernNxtPanelDesc = tavernHire.transform.Find ("PanelNxtDesc").gameObject; 
			tavernHire.SetActive (false);
		}
		if (unitPMarket == null) {
			unitPMarket = imageP.transform.Find ("UnitPanelMarket").gameObject; 
		}
		if (unitPPurchase == null) {
			unitPPurchase = imageP.transform.Find ("UnitPanelPurchase").gameObject; 
		}
		if (heroTavernModal == null) {
			heroTavernModal = imageP.transform.Find ("HeroTavernModal").gameObject; 
			heroTavernModal.SetActive (false);
		}
		if (militiaPanel == null) {
			militiaPanel = imageP.transform.Find ("MilitiaPanel").gameObject; 
			militiaPanel.SetActive (false);
		}
		if (unitModal == null) {
			unitModal = imageP.transform.Find ("UnitSelectionModal").gameObject; 
			unitModal.SetActive (false);
		}
		if (dMeta == null) {
			dMeta = CastlePrefs.getCastleMeta ();
		}
		if (genMeta == null) {
			genMeta = getGeneral(glossary);
		}
	}

	public void refresh() {
		refreshMilitia ();
	}

	public void onPurchaseBuy(){
		if (purchaseUnit != null) {
			BattleMeta pUnit = purchaseUnit.GetComponent<BattleMeta> ();
				
			Dictionary<string, int> resources = new Dictionary<string, int> ();
			resources.Add ("gold", pUnit.costGold);
			resources.Add ("ore", pUnit.costOre);
			resources.Add ("wood", pUnit.costWood);
			resources.Add ("ruby", pUnit.costRuby);
			resources.Add ("crystal", pUnit.costCrystal);
			resources.Add ("sapphire", pUnit.costSapphire);

			if (genMeta.buyUnit(resources, purchaseUnit)) {
				Debug.Log ("Purchased!");
				CastlePrefs.dirty = true;
			} else {
				Debug.Log ("Not Purchased!");
			}
		}
	}

	public void onFlipMenu() {
		CastlePrefs.showUnitCity = !CastlePrefs.showUnitCity;
		CastlePrefs.dirty = true;
	}

	/*
	 * Click Toggle UI Code
	 */ 

	public void onPurchaseCancel(){
		unitPPurchase.SetActive(false);
	}

	private void onCloseAll(){
		onCloseTown();
		onCloseMarket();
	}

	private void onCloseTown(){
		onLeaveTavern ();
		onLeaveUpgrades ();
		onLeaveMilitia ();
	}

	private void onCloseMarket(){
		unitPPurchase.SetActive(false);
		unitPMarket.SetActive(false);
	}

	public void openPurchaseView(int unit){
		unitPPurchase.SetActive(true);
		loadPurchase (unit);
	}

	public void onClickUnitToggle(){
		townSelection.SetActive (false);
		onCloseTown();
		unitPMarket.SetActive(!unitPMarket.activeSelf);
		if (!unitPMarket.activeSelf) {
			onCloseAll ();
		}
	}

	public void onClickTownToggle(){
		unitPMarket.SetActive (false);
		onCloseMarket();
		townSelection.SetActive(!townSelection.activeSelf);
		if (!townSelection.activeSelf) {
			onCloseAll ();
		}
	}

	public void onEnterTavern(){
		tavernHire.SetActive(true);
		tavernSelected = -1;

		// Populate tavern shit here
		Debug.Log("Tavern Occupants");
		int cnt = 1;
		foreach (GameObject met in glossary.generals) {
			BattleGeneralMeta bgm = met.GetComponent<BattleGeneralMeta> ();
			if (bgm.faction.Equals(cMeta.affiliation.name) && cnt < 7) {
				Transform genTrans = tavernHire.transform.Find("Hero_0" + cnt);
				genTrans.gameObject.SetActive (true);
				Image portrait = genTrans.transform.Find("Portrait").GetComponent<Image>();
				Text hrText = genTrans.transform.Find("TxtOverlay").Find("Text").GetComponent<Text>();
				portrait.sprite = met.GetComponent<Image> ().sprite;
				hrText.text = bgm.name;
				tavernRoster [cnt - 1] = met;
				cnt++;
			}
		}
		while (cnt < 7) {
			Transform genTrans = tavernHire.transform.Find("Hero_0" + cnt);
			genTrans.gameObject.SetActive (false);
			tavernRoster [cnt - 1] = null;
			cnt++;
		}

		//Add click actions here, and change descriptions if the hero has been clicked on
		tavernPanelDesc.GetComponent<Text>().text = "";
		tavernNxtPanelDesc.GetComponent<Text>().text = "";
	}

	public void onClickTavernHero(int pos){
		Debug.Log ("Click: " + pos);
		GameObject clicked = tavernRoster [pos - 1];
		if (clicked != null) {
			BattleGeneralMeta bgm = clicked.GetComponent<BattleGeneralMeta> ();
			tavernPanelDesc.GetComponent<Text>().text = bgm.name;
			tavernNxtPanelDesc.GetComponent<Text>().text = bgm.description;
			tavernSelected = pos;
		}
	}

	public void onToggleModal(bool on){
		heroTavernModal.SetActive (on);
	}

	public void onClickBuyTavernHero(){
		if (genMeta.getResources ().getResource("gold") >= 1000) {
			Debug.Log ("Currently Selected Hero: " + tavernSelected);
			BattleGeneralMeta tHero = tavernRoster [tavernSelected - 1].GetComponent<BattleGeneralMeta>();
			onClickBuyTavernPos (tHero, genMeta.getBanner(), glossary.findFaction(tHero.faction));
			genMeta.getResources ().useResource ("gold",1000);
			onToggleModal(false);
			CastlePrefs.dirty = true;
		}
	}

	public static void onClickBuyTavernPos(BattleGeneralMeta tHero, Color banner, AffiliationMeta affmet){
		//Make sure the hero is marked player and the hero's banner matches the player's banner
		tHero.setBanner (banner);
		tHero.setPlayer (true);

		List<GameObject> newUnits = new List<GameObject> ();
		foreach (GameObject unit in affmet.units) {
			BattleMeta gmeta = unit.GetComponent<BattleMeta> ();
			if (gmeta.lvl < 3) {
				GameObject instance = Instantiate (unit) as GameObject;
				BattleMeta bMet = instance.GetComponent<BattleMeta> ();
				bMet.setGUI (false);
				bMet.setPlayer (false);
				switch (bMet.lvl) {
				case 1:
					bMet.setLives (Random.Range (15, 25));
					break;
				default:
					bMet.setLives (Random.Range (5, 10));
					break;
				}
				instance.SetActive (false);
				newUnits.Add (instance);
			}
		}

		//If the player has enough resources, purchase hero and add it to the queue
		CastleConverter.putTavernGeneral(new BattleGeneralMeta[]{tHero});
	}

	public void onLeaveTavern(){
		tavernHire.SetActive(false);
	}

	public void onEnterUpgrades(){
		upgradePurchase.SetActive(true);
	}

	public void onLeaveUpgrades(){
		upgradePurchase.SetActive(false);
	}
		
	public void onEnterMiltia(){
		militiaPanel.SetActive(true);
		refreshMilitia ();
	}

	public void refreshMilitia(){
		for (int i = 0; i < 6; i++) {
			string rw_id = "Rw" + (i + 1).ToString ();
			GameObject rw = militiaPanel.transform.Find (rw_id).gameObject; 
			if (i < castleResources.getArmy ().Count) {
				rw.SetActive (true);
				GameObject img = rw.transform.Find ("Unit").gameObject; 
				GameObject name = rw.transform.Find ("NameTxt").gameObject; 
				GameObject quantity = rw.transform.Find ("QntyTxt").gameObject; 
				GameObject health = rw.transform.Find ("HealthTxt").gameObject; 
				GameObject attack = rw.transform.Find ("AttackTxt").gameObject; 
				GameObject range = rw.transform.Find ("RangeTxt").gameObject; 
				GameObject action = rw.transform.Find ("ActionTxt").gameObject; 

				img.GetComponent<Image>().sprite = castleResources.getArmy () [i].GetComponent<SpriteRenderer> ().sprite;

				BattleMeta bm = castleResources.getArmy () [i].GetComponent<BattleMeta> ();
				name.GetComponent<Text>().text = bm.name;
				quantity.GetComponent<Text>().text = bm.getLives().ToString();
				health.GetComponent<Text>().text = bm.getCharHp().ToString();
				attack.GetComponent<Text>().text = bm.attack.ToString();
				range.GetComponent<Text>().text = bm.range.ToString();
				action.GetComponent<Text>().text = bm.movement.ToString();
			} else {
				rw.SetActive (false);
			}
		}
	}

	public void onLeaveMilitia(){
		militiaPanel.SetActive(false);
	}

	public void onUnitToggleModal(bool on){
		unitModal.SetActive (on);
		CastlePrefs.toDelete = -1;
		CastlePrefs.dirty = true;
	}

	public void onClickModal(int unit){
		onUnitToggleModal (true);
		unitSelected = unit;
		Debug.Log ("Click Modal");
	}

	/*
	 * End toggle code
	 */ 

	public void onClickAccept(){
		if (genMeta.getArmy().Count > 0) {
			CastleConverter.putSave (castleGenMeta, genMeta, null);
		} else {
			Debug.Log("Error");
		}
		SceneManager.LoadScene ("AdventureScene");
	}

	// Adds the army to the miltia
	public void onClickMilitiaAdd(){
		int unit = unitSelected;
		if (castleResources != null && castleResources.getArmy().Count < 6) {
			List<GameObject> army = genMeta.getResources().getArmy (); 
			GameObject unitArmy = army[unit - 1];
			BattleMeta uMeta = unitArmy.GetComponent<BattleMeta>();
			if (uMeta.getLives () > 0) {
				castleGenMeta.addUnit (unitArmy, uMeta.getLives ());
				army.RemoveAt(unit - 1);
				genMeta.setArmy (army);
				onUnitToggleModal (false);
			}
		}
	}

	// Adds the militia back to the players army
	public void onPlayerArmyAdd(int selected){
		int unit = selected;
		if (castleResources != null && castleResources.getArmy().Count > 0 
			&& castleResources.getArmy().Count > unit && genMeta.getResources().getArmy ().Count < 6) {
			List<GameObject> army = castleResources.getArmy (); 
			GameObject unitArmy = army[unit];
			BattleMeta uMeta = unitArmy.GetComponent<BattleMeta>();
			if (uMeta.getLives () > 0) {
				genMeta.addUnit(unitArmy, uMeta.getLives());
				army.RemoveAt(unit);
				castleResources.setArmy (army);
				CastlePrefs.dirty = true;
			}
		}
	}

	public void onClickRemove(){
		int unit = unitSelected;
		if (CastlePrefs.toDelete == unit && genMeta.getResources().getArmy ().Count > 1) {
			// Remove here
			Debug.Log("Removing here: " + unit);
			CastlePrefs.toDelete = -1;
			List<GameObject> army = genMeta.getResources().getArmy (); 
			army.RemoveAt(unit - 1);
			genMeta.setArmy (army);
			onUnitToggleModal (false);
		} else {
			CastlePrefs.toDelete = unit;
		}
		CastlePrefs.dirty = true;
	}

	private void loadPurchase(int unitLvl){
		foreach (GameObject unit in dMeta.affiliation.units) {
			Debug.Log ("Unit: " + unit.name);
			if (unitLvl == unit.GetComponent<BattleMeta> ().lvl) {
				purchaseUnit = unit;
			}
		}

		if (purchaseUnit != null) {
			Transform mainParent = unitPPurchase.transform.Find("UnitParent");
			Transform mainPic = mainParent.transform.Find("Unit");
			mainPic.gameObject.GetComponent<Image> ().sprite = purchaseUnit.GetComponent<SpriteRenderer> ().sprite;

			Transform unitPanel = mainParent.transform.Find("UnitPanel");
			Transform unitLvlText = unitPanel.transform.Find("UnitLvl");
			unitLvlText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().lvl.ToString();

			Transform unitTitle = unitPPurchase.transform.Find("UnitTitle");
			unitTitle.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().name;

			Transform unitDescription = unitPPurchase.transform.Find("UnitDescription");
			unitDescription.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().description;

			Transform assets = mainParent.transform.Find("UnitAssets");
			Transform healthText = assets.transform.Find("HealthText");
			healthText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().hp.ToString();
			Transform attackText = assets.transform.Find("AttackText");
			attackText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().attack.ToString();
			Transform rangeText = assets.transform.Find("RangeText");
			rangeText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().range.ToString();
			Transform actionText = assets.transform.Find("ActionText");
			actionText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().getMovement().ToString();

			Transform resourceP = unitPPurchase.transform.Find("UnitResourcePanel");
			Transform sapphireText = resourceP.transform.Find ("SapphireText"); 
			sapphireText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costSapphire.ToString();
			Transform oreText = resourceP.transform.Find ("OreText"); 
			oreText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costOre.ToString();
			Transform goldText = resourceP.transform.Find ("GoldText"); 
			goldText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costGold.ToString();
			Transform woodText = resourceP.transform.Find ("WoodText"); 
			woodText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costWood.ToString();
			Transform rubyText = resourceP.transform.Find ("RubyText"); 
			rubyText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costRuby.ToString();
			Transform crystalText = resourceP.transform.Find ("CrystalText"); 
			crystalText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costCrystal.ToString();

		}
	}
}
