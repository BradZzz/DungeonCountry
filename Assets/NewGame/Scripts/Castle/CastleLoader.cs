using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CastleLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		init (CastlePrefs.getGeneralMeta (), CastlePrefs.getCastleMeta ());
	}
	
	// Update is called once per frame
	void init (BattleGeneralResources gMeta, CastleMeta dMeta) {
		//Pull the canvas from the hierarchy
		GameObject canvas = GameObject.Find ("Canvas"); 
		Transform imageP = canvas.transform.Find ("ImagePanel"); 
		Transform unitP = imageP.transform.Find ("UnitPanel"); 

		Transform marketP = imageP.transform.Find ("UnitPanelMarket"); 
		marketP.gameObject.SetActive (false);

		Transform marketPPurchase = imageP.transform.Find ("UnitPanelPurchase"); 
		marketPPurchase.gameObject.SetActive (false);

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
		foreach (GameObject unit in gMeta.getarmy()) {
			count += 1;
			Transform unitImage = unitP.transform.Find ("Unit" + count); 
			unitImage.gameObject.GetComponent<Image> ().sprite = unit.GetComponent<SpriteRenderer> ().sprite;
			Transform unitPanel = unitP.transform.Find ("Unit" + count + "Panel"); 
			Transform unitText = unitPanel.transform.Find ("Unit" + count + "Text"); 
			unitText.gameObject.GetComponent<Text> ().text = unit.GetComponent<BattleMeta> ().getLives().ToString();
			Debug.Log ("Name: " + unit.name);
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


		/*
		 * 
		 * 
		 *  resources.Add ("gold", 0);
		 *	resources.Add ("ore", 0);
			resources.Add ("wood", 0);
			resources.Add ("ruby", 0);
			resources.Add ("crystal", 0);
			resources.Add ("sapphire", 0);
		 * 
		 * 
		 * /

		//Put the image into the image section
		//Transform imageP = canvas.transform.Find("ImagePanel");

		//Transform generalP = imageP.transform.Find("GeneralPanel");
		//Transform cImage = imageP.transform.Find("CityImage");
		//Transform cityP = imageP.transform.Find("CityDescription");
		//Transform unitP = imageP.transform.Find("UnitPanel");
		/*
		 * Pull army from generalMeta
		 */ 
		//if (unitP != null && gMeta != null) {

			/*for (int i = 1; i < 7; i++) {
				Transform unitObject = imageP.transform.Find("Unit" + i);
				if (i <= gMeta.army.Count) {
					GameObject unit = gMeta.army [i];
					SpriteRenderer unitImage = unit.GetComponent<SpriteRenderer> ();
					unitObject.gameObject.GetComponent<Image>().sprite = unitImage.sprite;

					//Image image = unitObject.GetComponent<Image> ();
					//image.sprite = units [i - 1].GetComponent<SpriteRenderer> ().sprite;
				} else {
					unitObject.gameObject.SetActive (false);
				}
			}*/

			/*for (int i = 0; i < gMeta.army.Count; i++) {
				GameObject unit = gMeta.army [i];
				SpriteRenderer unitImage = unit.GetComponent<SpriteRenderer> ();

				Transform unit1Image = imageP.transform.Find("Unit" + i);
				imageP.gameObject.GetComponent<Image>.sprite = unitImage.sprite;
			}*/
			/*Transform unit1Image = imageP.transform.Find("Unit1");
			Transform unit1Panel = unit1Image.transform.Find("Unit1Panel");
			Transform unit1Text = unit1Panel.transform.Find("Unit1Text");

			Transform unit2Image = imageP.transform.Find("Unit2");
			Transform unit2Panel = unit1Image.transform.Find("Unit2Panel");
			Transform unit2Text = unit1Panel.transform.Find("Unit2Text");

			Transform unit3Image = imageP.transform.Find("Unit3");
			Transform unit3Panel = unit1Image.transform.Find("Unit3Panel");
			Transform unit3Text = unit1Panel.transform.Find("Unit3Text");

			Transform unit4Image = imageP.transform.Find("Unit4");
			Transform unit4Panel = unit1Image.transform.Find("Unit4Panel");
			Transform unit4Text = unit1Panel.transform.Find("Unit4Text");

			Transform unit5Image = imageP.transform.Find("Unit5");
			Transform unit5Panel = unit1Image.transform.Find("Unit5Panel");
			Transform unit5Text = unit1Panel.transform.Find("Unit5Text");

			Transform unit6Image = imageP.transform.Find("Unit6");
			Transform unit6Panel = unit1Image.transform.Find("Unit6Panel");
			Transform unit6Text = unit1Panel.transform.Find("Unit6Text");*/
		//}


		//Transform resourceP = imageP.transform.Find("ResourcePanel");
	}
}
