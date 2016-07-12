using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleMeta : MonoBehaviour {

	//Game Meta
	public int movement;
	public int range;
	public int attack;
	public int hp;

	public string name;
	public string description;

	private bool canMove;
	private int currentHP;

	//Game Sprite Modifiers
	public Texture2D tex;

	private Animator animator;

	void Awake()
	{
		currentHP = hp;
		animator = GetComponent<Animator>();
	}

	public bool getMove(){
		return canMove;
	}

	private void OnGUI() {
		Vector3 guiPosition = Camera.main.WorldToScreenPoint(transform.position);
		guiPosition.y = Screen.height - guiPosition.y;
		Rect rect = new Rect(guiPosition.x - tex.width/2, guiPosition.y - 3f * tex.height, tex.width * (float) currentHP/ (float) hp, tex.height);
		GUI.DrawTexture(rect, tex);
	}

	public void isAttacked (int attack) {
		currentHP -= attack;
		if (currentHP <= 0) {
			gameObject.SetActive(false);
		}
	}

	public void atkAnim(){
		animator.SetTrigger ("UnitAttack");
	}
}

public class BattleMetaFactory : MonoBehaviour {
	
	void Start()
	{
		BattleMeta myScript = gameObject.AddComponent( typeof ( BattleMeta ) ) as BattleMeta;
	}
}
