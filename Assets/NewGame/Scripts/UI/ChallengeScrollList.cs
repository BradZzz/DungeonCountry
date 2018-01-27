using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class Challenge
{
	public string lvlName = "New Level";
	public string lvlScore = "High Score: 0";
	public Color lvlColor = Color.green;
	public GameObject challengeObject;
}

public class ChallengeScrollList : MonoBehaviour {

	public List<Challenge> challengeList;
	public Transform contentPanel;
	public SimpleObjectPool challengeObjectPool;


	// Use this for initialization
	void Start () 
	{
		RefreshDisplay ();
	}

	void RefreshDisplay()
	{
		RemoveButtons ();
		AddButtons ();
	}

	private void RemoveButtons()
	{
		while (contentPanel.childCount > 0) 
		{
			GameObject toRemove = transform.GetChild(0).gameObject;
			challengeObjectPool.ReturnObject(toRemove);
		}
	}

	private void AddButtons()
	{
		for (int i = 0; i < challengeList.Count; i++) 
		{
			Challenge challenge = challengeList[i];
			GameObject newButton = challengeObjectPool.GetObject();
			newButton.transform.SetParent(contentPanel);

			ChallengeButton sampleButton = newButton.GetComponent<ChallengeButton>();
			sampleButton.Setup(challenge, this);
		}
	}

//	public void TryTransferItemToOtherShop(Item item)
//	{
//		if (otherShop.gold >= item.price) 
//		{
//			gold += item.price;
//			otherShop.gold -= item.price;
//
//			AddItem(item, otherShop);
//			RemoveItem(item, this);
//
//			RefreshDisplay();
//			otherShop.RefreshDisplay();
//			Debug.Log ("enough gold");
//
//		}
//		Debug.Log ("attempted");
//	}

	void AddItem(Challenge itemToAdd, ChallengeScrollList shopList)
	{
		shopList.challengeList.Add (itemToAdd);
	}

	private void RemoveItem(Challenge itemToRemove, ChallengeScrollList shopList)
	{
		for (int i = shopList.challengeList.Count - 1; i >= 0; i--) 
		{
			if (shopList.challengeList[i] == itemToRemove)
			{
				shopList.challengeList.RemoveAt(i);
			}
		}
	}
}