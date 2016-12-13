using UnityEngine;
using System.Collections;

public class GeneralAttributes {
	
	private int tactics, attack, defense, intelligence, luck, exp;

	public GeneralAttributes(){
		tactics = 2;
		attack = 1;
		defense = 1;
		intelligence = 1;
		luck = 1;
		exp = 0;
	}

	public void addTactics(int tactics){
		this.tactics = tactics;
	}

	public int getTactics(){
		return tactics;
	}

	public void addAttack(int attack){
		this.attack = attack;
	}

	public int getAttack(){
		return attack;
	}

	public void getDefense(int defense){
		this.defense = defense;
	}

	public int getDefense(){
		return defense;
	}

	public void addIntelligence(int intelligence){
		this.intelligence = intelligence;
	}

	public int getIntelligence(){
		return intelligence;
	}

	public void addLuck(int luck){
		this.luck = luck;
	}

	public int getLuck(){
		return luck;
	}

	public void addExp(int exp){
		this.exp = exp;
	}

	public int getExp(){
		return exp;
	}

	public int getLvlExp(int lvl){
		return (int) Mathf.Pow (5, lvl);
	}

	public int getLvl(){
		int tmp = exp;
		int lvl = 0;
		while (tmp > 0) {
			lvl += 1;
			tmp -= getLvlExp(lvl);
		}
		return lvl;
	}
}
