using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeButton : MonoBehaviour {

	public Text name;
	public Text score;
	public Image image;
	public Button button;

	private Challenge item;
	private ChallengeScrollList scrollList;
	private BattleObject battle;

	// Use this for initialization
	void Start () {
		button.onClick.AddListener (HandleClick);
	}

	public void Setup(Challenge currentItem, ChallengeScrollList currentScrollList)
	{
		item = currentItem;
		name.text = item.lvlName;
		score.text = item.lvlScore;
		image.color = item.lvlColor;
		battle = item.challengeObject.GetComponent<BattleObject> ();
		scrollList = currentScrollList;
	}

	public void HandleClick(){
		Debug.Log ("click");
		BattleConverter.putSaveBattleObject (battle);
		Application.LoadLevel ("BattleScene");
	}
}
