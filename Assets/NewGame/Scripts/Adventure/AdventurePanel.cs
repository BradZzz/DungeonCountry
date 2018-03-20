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
	private GameObject GeneralPanel, ResourcePanel, ArmyPanel, Avatar;
	private BattleGeneralMeta player;
	private Image playerImg;

	// Use this for initialization
	void Start () {
		adventureManager = GameObject.Find ("AdventureManager(Clone)");
		agm = adventureManager.GetComponent<AdventureGameManager> ();
		steps = path.GetComponent<Footsteps> ();
		foreach (Transform child in transform)
		{
			if (child.name.Equals ("GeneralPanel")) {
				GeneralPanel = child.gameObject;
			}
			if (child.name.Equals ("ResourcePanel")) {
				ResourcePanel = child.gameObject;
			}
			if (child.name.Equals ("ArmyPanel")) {
				ArmyPanel = child.gameObject;
			}
			if (child.name.Equals ("Avatar")) {
				Avatar = child.gameObject;
			}
		}
		GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject unit in units) {
			BattleGeneralMeta bgm = unit.GetComponent<BattleGeneralMeta> ();
			if (bgm != null && bgm.getPlayer ()) {
				player = bgm;
				playerImg = unit.GetComponent<Image> ();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		setPlayerAvatar (playerImg);

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
		wakeAI ();
		makeDecision(getNextAI());
	}

	public void FinishTurn(){
		Debug.Log ("Finish Turn");
		Start ();
		makeDecision(getNextAI());
	}

	private void makeDecision(GameObject unit){
		if (unit == null) {
			Debug.Log ("Error Receiving Unit!");
			player.startTurn ();
			player.startMoving ();
		} else {
			BattleGeneralMeta bgm = unit.GetComponent<BattleGeneralMeta> ();
			if (bgm != null) {
				if (!bgm.getPlayer ()) {
					// TODO: Move the camera to where the ai is for 2 seconds
					// bgm.startTurn ();
					BattleGeneralAI ai = new BattleGeneralAI (unit);
					ai.moveGeneral (GameObject.Find ("Board").transform);
					StartCoroutine (steps.generateMapv2 (unit.transform, new Point3 (unit.transform.position), new Point3 (ai.getObjective ().transform.position), agm.getRows (), agm.getColumns (), ai.getObstacles (), getPath));
				}
			}
		}
	}

	private bool checkAIOver(){
		GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject unit in units) {
			BattleGeneralMeta bgm = unit.GetComponent<BattleGeneralMeta> ();
			if (bgm != null) {
				if (!bgm.getPlayer () && bgm.getTurn()) {
					return false;
				}
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

	private void setPlayerAvatar(Image sprite) {
		GameObject profImage = Avatar.transform.Find ("ProfileImage").gameObject;
		Image img = profImage.GetComponent<Image> ();
		img.sprite = sprite.sprite;
	}

	private void getPath(Transform ai, List<Point3> path, Point3 destination){
		Debug.Log ("Get Path");
		StartCoroutine (step_path (ai, path, destination, .5f));
	}

	private void wakeAI(){
		GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject unit in units) {
			BattleGeneralMeta bgm = unit.GetComponent<BattleGeneralMeta> ();
			if (bgm != null) {
				bgm.startTurn ();
			}
		}
	}
		
	private GameObject getNextAI(){
		GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject unit in units) {
			BattleGeneralMeta bgm = unit.GetComponent<BattleGeneralMeta> ();
			if (bgm != null) {
				if (!bgm.getPlayer () && bgm.getTurn() && bgm.getArmy().Count > 0) {
					if (bgm.makeSteps(0) > 0 && !bgm.getMoving()) {
						bgm.startMoving ();
					}
					return unit;
				}
			}
		}
		return null;
	}

	IEnumerator step_path(Transform ai, List<Point3> step_path, Point3 destination, float speed)
	{
		BattleGeneralMeta aiMeta = ai.gameObject.GetComponent<BattleGeneralMeta> ();
		//Neutral armies only attack the player and even then, only when the player is within a super short range
		if (aiMeta != null && aiMeta.faction.Equals("Neutral") && step_path.Count > aiMeta.getCurrentMoves()) {
			step_path = null;
		}
		if (step_path != null) {
			BattleGeneralMeta bgm = ai.gameObject.GetComponent<BattleGeneralMeta> ();
			int steps_left = bgm.makeSteps (step_path.Count);
			if (steps_left <= 0) {
				bgm.endTurn ();
				while (steps_left < 0) {
					step_path.RemoveAt (step_path.Count - 1);
					steps_left++;
				}
			}
			foreach (Point3 step in step_path) {
				yield return StartCoroutine (Coroutines.smooth_move (ai, step.asVector3 (), speed));
			}
		} else {
			BattleGeneralMeta bgm = ai.gameObject.GetComponent<BattleGeneralMeta> ();
			bgm.endTurn ();
		}

		//Start the player's turn after all the ai players have moved
		if (checkAIOver ()) {
			player.startTurn ();
			player.startMoving ();
		} else {
			makeDecision(getNextAI());
		}
	}
}
