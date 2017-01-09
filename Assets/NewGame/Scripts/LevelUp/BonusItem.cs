using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BonusItem {

	private string name;
	private string description;
	private Action<int> modifier;
	private List<BonusItem> children;
	private int bonus;
	private SpriteRenderer icon;

	public BonusItem(string name, string description, SpriteRenderer icon, Action<int> modifier, int bonus) {
		children = new List<BonusItem> ();
		this.icon = icon;
		this.name = name;
		this.description = description;
		this.modifier = modifier;
		this.bonus = bonus;
	}

	public string getName(){
		return name;
	}

	public string getDesc(){
		return description;
	}

	public SpriteRenderer getIcon(){
		return icon;
	}

	public void addChild(BonusItem child){
		children.Add (child);
	}

	public List<BonusItem> getChildren(){
		return children;
	}

	public void activateBonus(){
		this.modifier (bonus);
	}

}
