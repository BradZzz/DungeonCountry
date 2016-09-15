using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CastleLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {

		BattleGeneralMeta gMeta = CastlePrefs.getGeneralMeta ();
		CastleMeta dMeta = CastlePrefs.getCastleMeta ();

		init (gMeta, dMeta);
	}
	
	// Update is called once per frame
	void init (BattleGeneralMeta gMeta, CastleMeta dMeta) {
		//Pull the canvas from the hierarchy
		GameObject canvas = GameObject.Find ("Canvas"); 

		//Put the image into the image section
		Transform imageP = canvas.transform.Find("ImagePanel");

		Transform generalP = imageP.transform.Find("GeneralPanel");
		Transform cImage = imageP.transform.Find("CityImage");
		Transform cityP = imageP.transform.Find("CityDescription");
		Transform unitP = imageP.transform.Find("UnitPanel");
		/*
		 * Pull army from generalMeta
		 */ 
		if (unitP != null && gMeta != null) {

			for (int i = 1; i < 7; i++) {
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
			}

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
		}


		Transform resourceP = imageP.transform.Find("ResourcePanel");
	}
}
