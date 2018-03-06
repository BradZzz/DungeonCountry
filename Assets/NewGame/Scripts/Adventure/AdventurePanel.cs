using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;

public class AdventurePanel : MonoBehaviour {
	public GameObject path;

	private GameObject adventureManager;
	private Footsteps steps;
	private AdventureGameManager agm;
	private GameObject GeneralPanel, ResourcePanel, ArmyPanel;
	private BattleGeneralMeta player;

	// Use this for initialization
	void Start () {
		adventureManager = GameObject.Find ("AdventureManager(Clone)");
		agm = adventureManager.GetComponent<AdventureGameManager> ();
		steps = path.GetComponent<Footsteps> ();
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
		Debug.Log ("End Turn");
		GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject unit in units) {
			BattleGeneralMeta bgm = unit.GetComponent<BattleGeneralMeta> ();
			if (!bgm.getPlayer ()) {
				// Move the camera to where the ai is for 2 seconds
				bgm.startTurn();
				BattleGeneralAI ai = new BattleGeneralAI (unit);
				ai.moveGeneral (GameObject.Find ("Board").transform);
				Debug.Log ("Move AI");
				StartCoroutine (steps.generateMapv2(unit.transform, new Point3(unit.transform.position), new Point3(ai.getObjective().transform.position), agm.getRows(), agm.getColumns(), ai.getObstacles(), getPath));
//				bgm.endTurn ();
			}
		}
//		player.startTurn ();
	}

	private bool checkAIOver(){
		GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject unit in units) {
			BattleGeneralMeta bgm = unit.GetComponent<BattleGeneralMeta> ();
			if (!bgm.getPlayer () && bgm.getTurn()) {
				return false;
			}
		}
		return true;
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

	public void getPath(Transform ai, List<Point3> path, Point3 destination){
		Debug.Log ("Get Path");
		StartCoroutine (step_path (ai, path, destination, .5f));
	}

	IEnumerator step_path(Transform ai, List<Point3> step_path, Point3 destination, float speed)
	{
		foreach(Point3 step in step_path){
			yield return StartCoroutine( Coroutines.smooth_move(ai, step.asVector3(), speed));
		}

		BattleGeneralMeta bgm = ai.gameObject.GetComponent<BattleGeneralMeta> ();
		bgm.endTurn ();

		//Start the player's turn after all the ai players have moved
		if (checkAIOver()) {
			player.startTurn ();
		}
	}
}
