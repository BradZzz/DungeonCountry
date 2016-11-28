using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CastleMenu : MonoBehaviour {

	private GameObject imageP = null;
	private GameObject unitPMarket = null;
	private GameObject unitPPurchase = null;
	private CastleMeta dMeta = null;
	private BattleGeneralResources gMeta = null;

	private void initVars(){
		if (imageP ==  null){
			imageP = GameObject.Find ("ImagePanel");
		}
		if (unitPMarket == null) {
			unitPMarket = imageP.transform.Find ("UnitPanelMarket").gameObject; 
		}
		if (unitPPurchase == null) {
			unitPPurchase = imageP.transform.Find ("UnitPanelPurchase").gameObject; 
		}
		if (dMeta == null) {
			dMeta = CastlePrefs.getCastleMeta ();
		}
		if (gMeta == null) {
			gMeta = CastlePrefs.getGeneralMeta ();
		}
	}

	public void openPurchaseView(int unit){
		initVars ();
		unitPPurchase.SetActive(true);
		loadPurchase (unit);
	}

	public void onClickUnitToggle(){
		initVars ();
		unitPMarket.SetActive(!unitPMarket.activeSelf);
	}

	public void onPurchaseBuy(){
		initVars ();
		unitPPurchase.SetActive(false);
	}

	public void onPurchaseCancel(){
		initVars ();
		unitPPurchase.SetActive(false);
	}

	public void onCloseAll(){
		initVars ();
		unitPPurchase.SetActive(false);
		unitPMarket.SetActive(false);
	}

	public void onClickAccept(){
		Application.LoadLevel ("AdventureScene");
	}

	public void onClickDecline(){
		Application.LoadLevel ("AdventureScene");
	}

	private void loadPurchase(int unitLvl){
		GameObject purchaseUnit = null;
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
			actionText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().movement.ToString();

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
