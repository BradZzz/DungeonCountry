using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using AssemblyCSharp;

public class LevelUpMeta : MonoBehaviour {

	private LevelTree lvlTree;
	private LevelUpResources res;
	private Panel p1,p2,p3;
	private GameObject ch1, ch2, ch3;

	class Panel {

		public BonusItem item;
		Image icon;
		Text name;
		Text desc;

		public Panel(GameObject parent){
			icon = getObj(parent, "Image").GetComponent<Image>();
			name = getObj(parent, "TitleText").GetComponent<Text>();
			desc = getObj(parent, "DescText").GetComponent<Text>();
		}

		public void setPanel(BonusItem item){
			this.item = item;
			icon.sprite = item.getIcon ().sprite;
			name.text = item.getName ();
			desc.text = item.getDesc ();
		}

		private GameObject getObj(GameObject panel, string search){
			foreach (Transform child in panel.transform){
				if (child.name == search) {
					return child.gameObject;
				}
			}
			return null;
		}
	}

	public LevelUpMeta(GeneralAttributes general, GameObject[] upgrades) {
		res = new LevelUpResources (general, upgrades);
		lvlTree = new LevelTree (res);

		ch1 = GameObject.Find("Choice1");
		ch2 = GameObject.Find("Choice2");
		ch3 = GameObject.Find("Choice3");

		p1 = new Panel (ch1);
		p2 = new Panel (ch2);
		p3 = new Panel (ch3);

		initPanels ();
	}

	public void initPanels(){
		List<BonusItem> starters = lvlTree.returnStarters ();
		Coroutines.ShuffleArray(starters);
		if (starters.Count > 2) {
			p1.setPanel (starters [0]);
			p2.setPanel (starters [1]);
			p3.setPanel (starters [2]);
		} else if (starters.Count == 0) {
			ch1.SetActive(false);
		} else if (starters.Count == 1) {
			p1.setPanel (starters [0]);
			ch2.SetActive(false);
		} else if (starters.Count == 2) {
			p1.setPanel (starters [0]);
			p2.setPanel (starters [1]);
			ch3.SetActive(false);
		}
	}

	public List<BonusItem> returnStarters() {
		return lvlTree.returnStarters ();
	}

	public void selectStarters(int selected){
		switch(selected){
			case 1:
				lvlTree.selectStarters (p1.item);
				break;
			case 2:
				lvlTree.selectStarters (p2.item);
				break;
			case 3:
				lvlTree.selectStarters (p3.item);
				break;
			default:
				break;
		}
		initPanels ();
	}

	//This class holds the lvl tree that ties the bonus objects together
	public class LevelTree {
		
		public List<BonusItem> starters;
		public BonusItem selected;
		public List<BonusItem> all;

		public LevelTree (LevelUpResources res) {
			all = new List<BonusItem> (res.getItems());
//			starters = new List<BonusItem> (res.getItems());
//			foreach (BonusItem item in all) {
//				foreach (BonusItem child in item.getChildren()) {
//					starters.Remove(child);
//				}
//			}
		}

		public List<BonusItem> returnStarters(){
			return all;
		}

		public void selectStarters(BonusItem clicked){
			foreach (BonusItem item in all) {
				if  (item == clicked) {
					selected = clicked;
					break;
				}
			}

			if (selected != null){
				all.Remove (selected);
				foreach (BonusItem child in selected.getChildren()) {
					all.Add (child);
				}
			}
		}
	}

}
