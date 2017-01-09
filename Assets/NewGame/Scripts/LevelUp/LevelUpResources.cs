using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelUpResources : MonoBehaviour {

	private List<BonusItem> bonusItems;

	public LevelUpResources(){
		init (new GeneralAttributes());
	}

	public LevelUpResources (GeneralAttributes general) {
		init (general);
	}

	public List<BonusItem> getItems(){
		return bonusItems;
	}
		
	public void init(GeneralAttributes general){
		
		GameObject atk = GameObject.Find("AttackUp");
		GameObject atk1a = GameObject.Find("AttackRngUp");
		GameObject atk1b = GameObject.Find("AttackNRngUp");
		GameObject atk1c = GameObject.Find("AttackPshUp");

		GameObject def = GameObject.Find("DefenseUp");
		GameObject def1a = GameObject.Find("DefenseRngUp");
		GameObject def1b = GameObject.Find("DefenseNRngUp");
		GameObject def1c = GameObject.Find("DefenseSpllUp");

		GameObject intel = GameObject.Find("IntUp");
		GameObject intel1a = GameObject.Find("IntDmgUp");
		GameObject intel1b = GameObject.Find("IntCstUp");
		GameObject intel1c = GameObject.Find("IntTwcUp");

		GameObject luck = GameObject.Find("LuckUp");
		GameObject luck1a = GameObject.Find("LuckAtkUp");
		GameObject luck1b = GameObject.Find("LuckDefUp");
		GameObject luck1c = GameObject.Find("LuckDblResUp");

		GameObject tact = GameObject.Find("TacticsUp");
		GameObject tact1a = GameObject.Find("TacticsUMvUp");
		GameObject tact1b = GameObject.Find("TacticsURngUp");
		GameObject tact1c = GameObject.Find("TacticsHMvUp");

		SpriteRenderer atkspr1 = atk.GetComponent<SpriteRenderer>();
		SpriteRenderer atkspr1a = atk1a.GetComponent<SpriteRenderer>();
		SpriteRenderer atkspr1b = atk1b.GetComponent<SpriteRenderer>();
		SpriteRenderer atkspr1c = atk1c.GetComponent<SpriteRenderer>();

		SpriteRenderer defspr1 = def.GetComponent<SpriteRenderer>();
		SpriteRenderer defspr1a = def1a.GetComponent<SpriteRenderer>();
		SpriteRenderer defspr1b = def1b.GetComponent<SpriteRenderer>();
		SpriteRenderer defspr1c = def1c.GetComponent<SpriteRenderer>();

		SpriteRenderer intelspr1 = intel.GetComponent<SpriteRenderer>();
		SpriteRenderer intelspr1a = intel1a.GetComponent<SpriteRenderer>();
		SpriteRenderer intelspr1b = intel1b.GetComponent<SpriteRenderer>();
		SpriteRenderer intelspr1c = intel1c.GetComponent<SpriteRenderer>();

		SpriteRenderer luckspr1 = luck.GetComponent<SpriteRenderer>();
		SpriteRenderer luckspr1a = luck1a.GetComponent<SpriteRenderer>();
		SpriteRenderer luckspr1b = luck1b.GetComponent<SpriteRenderer>();
		SpriteRenderer luckspr1c = luck1c.GetComponent<SpriteRenderer>();

		SpriteRenderer tactspr1 = tact.GetComponent<SpriteRenderer>();
		SpriteRenderer tactspr1a = tact1a.GetComponent<SpriteRenderer>();
		SpriteRenderer tactspr1b = tact1b.GetComponent<SpriteRenderer>();
		SpriteRenderer tactspr1c = tact1c.GetComponent<SpriteRenderer>();

		bonusItems = new List<BonusItem> ();

		BonusItem strength1 = new BonusItem ("Strength", "Add one attack to your hero", atkspr1, general.addAttack, 1);
		strength1.addChild (new BonusItem ("Archer Might", "Increase ranged unit strength", atkspr1a, general.addAttack, 1));
		strength1.addChild (new BonusItem ("Fighter Might", "Increase non-ranged unit strength", atkspr1b, general.addAttack, 1));
		strength1.addChild (new BonusItem ("Mighty Blow", "Unit attacks have a chance to push enemies back", atkspr1c, general.addAttack, 1));
		bonusItems.Add (strength1);

		BonusItem defense1 = new BonusItem ("Defense", "Add one defense to your hero", defspr1, general.addDefense, 1);
		defense1.addChild (new BonusItem ("Aphemeral Defense", "Decrease unit damage from ranged", defspr1a, general.addDefense, 1));
		defense1.addChild (new BonusItem ("Iron Defense", "Decrease unit damage from non-ranged", defspr1b, general.addDefense, 1));
		defense1.addChild (new BonusItem ("Mystical Defense", "Decrease unit damage from spells", defspr1c, general.addDefense, 1));
		bonusItems.Add (defense1);

		BonusItem intelligence1 = new BonusItem ("Intelligence", "Add one intelligence to your hero", intelspr1, general.addIntelligence, 1);
		intelligence1.addChild (new BonusItem ("Mystical Conduit", "Increase damage from spells", intelspr1a, general.addIntelligence, 1));
		intelligence1.addChild (new BonusItem ("Archane Knowledge", "Decrease spell cost", intelspr1b, general.addIntelligence, 1));
		intelligence1.addChild (new BonusItem ("Spell Mirror", "Damage spells have a slight chance of being cast twice", intelspr1c, general.addIntelligence, 1));
		bonusItems.Add (intelligence1);

		BonusItem luck1 = new BonusItem ("Luck", "Add one luck to your hero", luckspr1, general.addLuck, 1);
		luck1.addChild (new BonusItem ("Backslash", "Units have an increased chance of dealing double damage ", luckspr1a, general.addLuck, 1));
		luck1.addChild (new BonusItem ("Tempering", "Units have an increased chance of being dealt half damage", luckspr1b, general.addLuck, 1));
		luck1.addChild (new BonusItem ("Fortune's Bounty", "Heroes have a slight chance of gaining double resources when picked up", luckspr1c, general.addLuck, 1));
		bonusItems.Add (luck1);

		BonusItem tactics1 = new BonusItem ("Tactics", "Add one tactics to your hero", tactspr1, general.addTactics, 1);
		tactics1.addChild (new BonusItem ("Swift Assault", "Increase all units movements by 1", tactspr1a, general.addTactics, 1));
		tactics1.addChild (new BonusItem ("Favorable Winds", "Increase all ranged character's range by 1", tactspr1b, general.addTactics, 1));
		tactics1.addChild (new BonusItem ("Pathfinder", "Increase your hero's movement by 20%", tactspr1c, general.addTactics, 1));
		bonusItems.Add (tactics1);
	}
}
