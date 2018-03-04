using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventurePanel : MonoBehaviour {

	private GameObject GeneralPanel, ResourcePanel, ArmyPanel;
	private BattleGeneralMeta player;

	// Use this for initialization
	void Start () {
		foreach (Transform child in transform)
		{
			if (child.name.Equals ("GeneralPanel")) {
				GeneralPanel = child.gameObject;
				Debug.Log ("GeneralPanel");
				foreach (Transform aChild in GeneralPanel.transform) {
					Debug.Log (aChild.name);
				}
			}
			if (child.name.Equals ("ResourcePanel")) {
				ResourcePanel = child.gameObject;
				Debug.Log ("ResourcePanel");
				foreach (Transform aChild in ResourcePanel.transform) {
					Debug.Log (aChild.name);
				}
			}
			if (child.name.Equals ("ArmyPanel")) {
				ArmyPanel = child.gameObject;
				Debug.Log ("ArmyPanel");
				foreach (Transform aChild in ArmyPanel.transform) {
					Debug.Log (aChild.name);
				}
			}
		}
		GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject unit in units) {
			BattleGeneralMeta bgm = unit.GetComponent<BattleGeneralMeta> ();
			if (bgm.getPlayer ()) {
				player = bgm;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		List<GameObject> army = player.getArmy ();
		for(int i = 0; i < 6; i++){
			if (i < army.Count) {
				BattleMeta arm = army [i].GetComponent<BattleMeta> ();
				SpriteRenderer armyspr = army [i].GetComponent<SpriteRenderer> ();
				GameObject panel = ArmyPanel.transform.Find ("Image_0" + (i + 1)).gameObject;
				panel.SetActive (true);
				GameObject image = panel.transform.Find ("ImageUnit").gameObject;
				Image spr = image.GetComponent<Image> ();
				spr.sprite = armyspr.sprite;
				GameObject textPnl = panel.transform.Find ("Qty").gameObject;
				GameObject text = textPnl.transform.Find ("QtyTxt").gameObject;
				Text tm = text.GetComponent<Text> ();
				tm.text = arm.getLives ().ToString ();
			} else {
				GameObject panel = ArmyPanel.transform.Find ("Image_0" + (i + 1)).gameObject;
				panel.SetActive (false);
			}
		}

		addResource ("Image_01", "gold");
		addResource ("Image_02", "ore");
		addResource ("Image_03", "wood");
		addResource ("Image_04", "ruby");
		addResource ("Image_05", "sapphire");
		addResource ("Image_06", "crystal");

		//Attack
		addGeneralAtt ("Image_01", player.getAttribute(BattleGeneralMeta.AttributeList.attack).ToString());
		//Magic
		addGeneralAtt ("Image_02", player.getAttribute(BattleGeneralMeta.AttributeList.magic).ToString());
		//Movement
		addGeneralAtt ("Image_03",  "<color='#ff0000ff'>" + player.getAttribute(BattleGeneralMeta.AttributeList.currMovement) + "</color> (<color='#008000ff'>" + player.getAttribute(BattleGeneralMeta.AttributeList.movement) + "</color>)");
		//Defense
		addGeneralAtt ("Image_04", player.getAttribute(BattleGeneralMeta.AttributeList.defense).ToString());
		//Tactics
		addGeneralAtt ("Image_05", player.getAttribute(BattleGeneralMeta.AttributeList.tactics).ToString());
		//Level
		addGeneralAtt ("Image_06", player.getAttribute(BattleGeneralMeta.AttributeList.level).ToString());

//		GameObject ore = ResourcePanel.transform.Find ("Image_02").gameObject;
//		GameObject wood = ResourcePanel.transform.Find ("Image_03").gameObject;
//		GameObject ruby = ResourcePanel.transform.Find ("Image_04").gameObject;
//		GameObject crystal = ResourcePanel.transform.Find ("Image_05").gameObject;
//		GameObject sapphire = ResourcePanel.transform.Find ("Image_06").gameObject;


//		for(int i = 0; i < 6; i++){
//			if (i < army.Count) {
//				BattleMeta arm = army [i].GetComponent<BattleMeta> ();
//				SpriteRenderer armyspr = army [i].GetComponent<SpriteRenderer> ();
//				GameObject panel = ArmyPanel.transform.Find ("Image_0" + (i + 1)).gameObject;
//				panel.SetActive (true);
//				GameObject image = panel.transform.Find ("ImageUnit").gameObject;
//				Image spr = image.GetComponent<Image> ();
//				spr.sprite = armyspr.sprite;
//				GameObject textPnl = panel.transform.Find ("Qty").gameObject;
//				GameObject text = textPnl.transform.Find ("QtyTxt").gameObject;
//				Text tm = text.GetComponent<Text> ();
//				tm.text = arm.getLives ().ToString ();
//			}
//		}
	}

	public List<GameObject> getAI(){
		List<GameObject> ai = new List<GameObject> ();
		GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject unit in units) {
			BattleGeneralMeta bgm = unit.GetComponent<BattleGeneralMeta> ();
			if (!bgm.getPlayer ()) {
				ai.Add (unit);
			}
		}
		return ai;
	}

	public void EndTurn(){
		GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject unit in units) {
			BattleGeneralMeta bgm = unit.GetComponent<BattleGeneralMeta> ();
			if (!bgm.getPlayer ()) {
				// Move the camera to where the ai is for 2 seconds

			}
		}
		player.startTurn ();
	}

	private void addGeneralAtt(string panel, string amount) {
		GameObject res = GeneralPanel.transform.Find (panel).gameObject;
		GameObject pnl = res.transform.Find ("Qty").gameObject;
		GameObject txt = pnl.transform.Find ("QtyTxt").gameObject;
		Text txtm = txt.GetComponent<Text> ();
		txtm.text = amount;
	}

	private void addResource(string panel, string resource) {
		GameObject res = ResourcePanel.transform.Find (panel).gameObject;
		GameObject pnl = res.transform.Find ("Qty").gameObject;
		GameObject txt = pnl.transform.Find ("QtyTxt").gameObject;
		Text txtm = txt.GetComponent<Text> ();
		txtm.text = player.getResource(resource).ToString();
	}
}
