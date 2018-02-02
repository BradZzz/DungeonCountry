using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using AssemblyCSharp;

/***
*	TODO: Make sure that units can only be placed on the orange parts of the board on setup
***/
using System.Runtime.CompilerServices;
using System;


public class BattleMeta : MonoBehaviour {

	//Game Meta
	public int lvl;
	public int movement;
	public int range;
	public int attack;
	public int defense;
	public int dMin;
	public int dMax;
	public int hp;
	public int ability;
	public GameObject[] abilities;

	public string name;
	public string description;

	//What team are they on
	public string affiliation;

	//If they are magical
	public bool magical = false;

	public int costGold;
	public int costOre;
	public int costWood;
	public int costSapphire;
	public int costCrystal;
	public int costRuby;
	public GameObject projectile;

	private int lives;
	private bool canMove;
	private bool canAttack;
	private BattleActions actions;
	private int currentHP;
	private Animator animator;
	private SpriteRenderer spriteRenderer;
	private Color healthColor;
	private bool isPlayer;
	private bool is_gui = true;
	private GeneralAttributes attribs = null;
	private string effect = null;
	private SpriteOutline outline = null;

	void Awake()
	{
		init ();
	}

	public void init() {
		//Debug.Log ("init");
		currentHP = hp;
		lives = 1;
		animator = GetComponent<Animator>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		actions = new BattleActions (1 + extraAct(), 1 + extraAtk(),true);
		//Debug.Log ("Sprite");
		//Debug.Log (spriteRenderer.sprite);
		canMove = true;
		outline = GetComponent<SpriteOutline>();
		outline.enabled = false;
	}

	public void startTurn(){
		actions.startTurn ();
		//SpriteRenderer sprRend = gameObject.GetComponent<SpriteRenderer> ();
		outline.enabled = false;
		//sprRend.material.shader = Shader.Find ("Sprites/Default");
	}

	public void setGUI(bool set){
		is_gui = set;
	}

	public int extraAtk(){
		foreach (GameObject ability in abilities) {
			BattleAttributes att = ability.GetComponent<BattleAttributes> ();
			if (att.extra_atk > 0) {
				return att.extra_atk;
			}
		}
		return 0;
	}

	public int extraAct(){
		foreach (GameObject ability in abilities) {
			BattleAttributes att = ability.GetComponent<BattleAttributes> ();
			if (att.extra_act > 0) {
				return att.extra_act;
			}
		}
		return 0;
	}


	public bool atkAll(){
		foreach (GameObject ability in abilities) {
			BattleAttributes att = ability.GetComponent<BattleAttributes> ();
			if (att.atk_all) {
				return true;
			}
		}
		return false;
	}

	public bool sap(){
		foreach (GameObject ability in abilities) {
			BattleAttributes att = ability.GetComponent<BattleAttributes> ();
			if (att.sap_obs) {
				return true;
			}
		}
		return false;
	}

	public string sapSpawn(){
		foreach (GameObject ability in abilities) {
			BattleAttributes att = ability.GetComponent<BattleAttributes> ();
			if (!att.sap_spawn.Equals("")) {
				return att.sap_spawn;
			}
		}
		return "";
	}

	public int getMovement(){
		return movement;
	}

	public int getRange(){
		return range;
	}

	public int getLives(){
		return lives;
	}

	public void setLives(int lives){
		this.lives = lives;
	}

	public void addLives(int lives){
		this.lives += lives;
	}

	public bool getTurn(){
		return actions.checkTurn ();
	}

	public void setTurn(bool isTurn){
		actions.setTurn(isTurn);
		checkFatigue ();
	}

	public bool getMove(){
		return canMove;
	}

	public void setMove(bool canMove){
		this.canMove = canMove;
	}

	public int getMaxAttacks(){
		return actions.getAttacks ();
	}

	public int getMaxActions(){
		return actions.getActions ();
	}

	public int getAttacks(){
		return actions.getAttacks ();
	}

	public int getActions(){
		return actions.getActions ();
	}

	public bool getPlayer(){
		return isPlayer;
	}

	public void setGeneralAttributes(GeneralAttributes attribs){
		this.attribs = attribs;
		currentHP = getCharHp ();
	}

	public int getCharHp(){
		return hp;
	}

	public int getICharStrength(){
		if (attribs != null) {
			return attack + attribs.getAttack();
		} else {
			return attack;
		}
	}

	public int getCharStrength(){
		if (attribs != null) {
			return (attack + attribs.getAttack()) * getLives ();
		} else {
			return attack * getLives ();
		}
	}

	public void setPlayer(bool player){
		this.isPlayer = player;
		if (getPlayer()) {
			healthColor = Color.green;
		} else {
			healthColor = Color.red;
		}
	}

	//Called when the player has verified moving checks (getActions)
	public void isMoving(){
		actions.takeAction (1);
		checkFatigue ();
	}

	public void checkFatigue(){
		if(!getTurn() && getPlayer()){
			SpriteRenderer sprRend = gameObject.GetComponent<SpriteRenderer> ();
			applyShadeEnd (sprRend);
		}
	}

	public void applyShadeEnd(SpriteRenderer sprRend){
		//Debug.Log ("applyShadeEnd");
		outline.setColor (false);
		outline.enabled = true;
		//sprRend.material.shader = Shader.Find ("Custom/OverlayPencilOutline");
	}

	public bool checkAttacks(){
		return actions.getAttacks () > 0 && getTurn ();
	}

	private static Texture2D _staticRectTexture;
	private static GUIStyle _staticRectStyle;

	private static Texture2D _staticHealthTexture;
	private static GUIStyle _staticHealthStyle;

	private void OnGUI() {
		if (is_gui && getLives() > 0) {
			Vector3 guiPosition = Camera.main.WorldToScreenPoint (transform.position);
			guiPosition.y = Screen.height - guiPosition.y;

			//Black Box Base
			Rect bRect = new Rect (guiPosition.x - 18, guiPosition.y - 38, 
				            30 * (float)hp / (float)hp + 4, 16);

			if (_staticRectTexture == null) {
				_staticRectTexture = new Texture2D (1, 1);
			}
			if (_staticRectStyle == null) {
				_staticRectStyle = new GUIStyle ();
			}
 
			_staticRectTexture.SetPixel (0, 0, Color.black);
			_staticRectTexture.Apply ();
 
			_staticRectStyle.normal.background = _staticRectTexture;
 
			GUI.Box (bRect, GUIContent.none, _staticRectStyle);
        
			//Health Overlay
			Rect hRect = new Rect (guiPosition.x - 16, guiPosition.y - 35, 
				            30 * (float)currentHP / (float)hp, 10);

			if (_staticHealthTexture == null) {
				_staticHealthTexture = new Texture2D (1, 1);
			}
			if (_staticHealthStyle == null) {
				_staticHealthStyle = new GUIStyle ();
			}

			_staticHealthTexture.SetPixel (0, 0, healthColor);
			_staticHealthTexture.Apply ();

			_staticHealthStyle.normal.background = _staticHealthTexture;

			GUI.Box (hRect, GUIContent.none, _staticHealthStyle);
		}
	}

	public int getCurrentHP(){
		return currentHP;
	}

	public void playEffect(string effect) {
		try {
			EffectConfig exp = this.transform.Find(effect).gameObject.GetComponent<EffectConfig> ();
			exp.Play ();
		} catch (Exception e) {
			Debug.Log (e.Message);
		}
	}

	public bool isAttacked (BattleMeta attacker, GeneralAttributes own, GeneralAttributes theirs) {

		bool ranged = attacker.range > 1;
		bool magical = attacker.magical;

		int tAtk = attacker.attack + theirs.getAttack ();
		int tDef = defense + own.getDefense();
		float I = tAtk >= tDef ? (float) (0.05 * (tAtk - tDef)) : 0;
		float R = tAtk <= tDef ? (float) (0.025 * (tDef - tAtk)) : 0;
		int dmgB = UnityEngine.Random.Range (attacker.dMin, attacker.dMax) * attacker.getLives();
		int attack = (int) (dmgB * (1 + I) * (1 - R));

		//meta.getCharStrength(), meta.range > 1, meta.magical
//		DMGf = DMGb × (1 + I1 + I2 + I3 + I4 + I5) × (1 - R1) × (1 - R2 - R3) × (1 - R4) × (1 - R5) × (1 - R6) × (1 - R7) × (1 - R8)
//		Primary determinant for the final damage is the base damage (DMGb), which is affected by the number of attacking creatures and their damage range. 
//		All other variables are basically modifiers of the base damage. Variables are denoted as I if they (i)ncrease damage and as R if they (r)educe it. 
//			I1 and R1 are mutually exclusive, but all other variables may simultaneously affect the final damage (DMGf). 
//			A brief summary of the variables have been given in the table on the right. To summarize the above formula, the content in the first parentheses 
//			increase the base damage by multiplying it with a modifier varying from 1.00 to 8.00, and the content in the second parentheses reduces 
//			the damage with a modifier varying from ~0.01 to 1.00.

//		I1   = 0.05 × (Attack - Defense) (if A ≥ D)
//		R1 = 0.025 × (Defense - Attack) (if D ≥ A)


		StartCoroutine (atk_blur (transform));

		if (magical) {
			playEffect ("Magic");
		} else {
			if (ranged) {
				playEffect ("Explosion");
			} else {
				playEffect ("Blood");
			}
		}

		StartCoroutine (showEffects ("-" + attack.ToString()));
		currentHP -= attack;
		if (currentHP <= 0) {
			while (currentHP <= 0) {
				addLives (-1);
				currentHP += getCharHp();
			}
			if (getLives() < 1){
				setLives (0);
				currentHP = 0;
				StartCoroutine (slowDeath ());
				return false;
			}
		}
		//The unit is still active
		return true;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void takeAttacks(int attacks){
		actions.takeAttack (attacks);
	}

	public bool isAttacking(BattleMeta enemy, bool atkall){
		if (enemy != null && (atkall || actions.takeAttack(1))) {
			isAttackingUnrestricted (enemy);
			return true;
		}
		return false;
	}

	public void isAttackingUnrestricted(BattleMeta enemy){
		//Debug.Log ("Attacking!");
		if (projectile != null) {
			GameObject thisProjectile = Instantiate<GameObject> (projectile);
			thisProjectile.transform.position = transform.position;
			if (enemy.isActiveAndEnabled) {
				StartCoroutine (smooth_move (thisProjectile.transform, enemy.gameObject.transform, .5f));
			} else {
				thisProjectile.gameObject.SetActive (false);
			}
		}
		animator.SetTrigger ("UnitAttack");
		StartCoroutine (atk_blur (enemy.gameObject.transform));
		checkFatigue ();
	}

	IEnumerator atk_blur(Transform unit){
		SpriteRenderer sprRend = unit.gameObject.GetComponent<SpriteRenderer> ();
		//sprRend.material.shader = Shader.Find ("Custom/OverlayShaderHit");
		outline.setColor (true);
		outline.enabled = true;
		yield return new WaitForSeconds(.5f);
		if (actions.checkTurn () || !getPlayer()) {
			outline.enabled = false;
			if (!getPlayer()) {
				outline.setColor (false);
				outline.enabled = true;
			}
			//sprRend.material.shader = Shader.Find ("Sprites/Default");
		} else {
			checkFatigue();
		}
	}

	IEnumerator smooth_move(Transform origin, Transform end, float speed){
		float startime = Time.time;
		Vector3 start_pos = new Vector3(origin.position.x, origin.position.y, origin.position.z); //Starting position.
		Vector3 end_pos = end.position;
		while (origin.position != end_pos) { 
			float move = Mathf.Lerp (0,1, (Time.time - startime) * speed);
			Vector3 position = origin.position;
			position.x += (end_pos.x - start_pos.x) * move;
			position.y += (end_pos.y - start_pos.y) * move;
			if (start_pos.x > end_pos.x && origin.position.x < end_pos.x) {
				position.x = end_pos.x;
			}
			if (start_pos.x < end_pos.x && origin.position.x > end_pos.x) {
				position.x = end_pos.x;
			}
			if (start_pos.y > end_pos.y && origin.position.y < end_pos.y) {
				position.y = end_pos.y;
			}
			if (start_pos.y < end_pos.y && origin.position.y > end_pos.y) {
				position.y = end_pos.y;
			}

			origin.position = position;

			yield return null;
		}
		origin.gameObject.SetActive (false);
	}

	public string getEffect(){
		return effect;
	}

	IEnumerator showEffects(string effect){
		this.effect = effect;
		yield return new WaitForSeconds(1.5f);
		this.effect = null;
	}

	IEnumerator slowDeath(){
		yield return new WaitForSeconds(1.5f);
		//Make the unit inactive after waiting a bit...
		gameObject.SetActive(false);
	}
}

public class BattleMetaFactory : MonoBehaviour {
	
	void Start()
	{
		BattleMeta myScript = gameObject.AddComponent( typeof ( BattleMeta ) ) as BattleMeta;
	}
}
