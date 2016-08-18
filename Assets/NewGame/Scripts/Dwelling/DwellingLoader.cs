using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DwellingLoader : MonoBehaviour {

	void Start ()
	{
		DwellingMeta dMeta = DwellingPrefs.getDwellingMeta ();
		Sprite dImage = DwellingPrefs.getDwellingRenderer();
		string player = DwellingPrefs.getPlayerName();
		init (dMeta, dImage, player);
	}

	void init(DwellingMeta dMeta, Sprite dImage, string player){

		//Pull the canvas from the hierarchy
		GameObject canvas = GameObject.Find ("Canvas"); 

		//Put the image into the image section
		Transform image = canvas.transform.Find("Image");

		if (dImage != null) {
			Debug.Log ("Something in image");
			if (image != null) {
				Debug.Log ("Something in image canvas");
				image.gameObject.GetComponent<Image> ().sprite = dImage;
			}
		} else {
			Debug.Log ("Nothing in entrance spriterenderer");
		}

		if (dMeta != null) {
			Debug.Log ("Something in meta");
			Transform dataP = canvas.transform.Find ("DataPanel");
			Transform cityP = canvas.transform.Find ("City Description");

			//Find Header
			Transform hGO = dataP.gameObject.transform.Find ("Header");
			Text hText = hGO.gameObject.GetComponent<Text> ();
			hText.text = dMeta.description;

			//Find Details
			Transform dGO = dataP.gameObject.transform.Find ("Description");
			Text dText = dGO.gameObject.GetComponent<Text> ();
			dText.text = dMeta.question;

			//Find Details
			Transform cGO = cityP.gameObject.transform.Find ("CityText");
			Text cText = cGO.gameObject.GetComponent<Text> ();
			cText.text = dMeta.name;

			//Find Details
			Transform gGO = dataP.gameObject.transform.Find ("GoldText");
			Text gText = gGO.gameObject.GetComponent<Text> ();
			gText.text = "1,200";

			//Find Details
			Transform sGO = dataP.gameObject.transform.Find ("SpellText");
			Text sText = sGO.gameObject.GetComponent<Text> ();
			sText.text = "64%";
		} else {
			Debug.Log ("Nothing in meta");
		}
	}
}
