using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/***
*	TODO: Make sure that units can only be placed on the orange parts of the board on setup
***/
public class BattleMeta : MonoBehaviour {

	//Game Meta
	public int movement;
	public int range;
	public int attack;
	public int hp;

	public string name;
	public string description;

	//What team are they on
	public string affiliation;

	private bool canMove;
	private bool canAttack;
	private BattleActions actions;

	private int currentHP;

	//Game Sprite Modifiers
	//Character's health
	public Texture2D tex;
	//Character's ranged attack sprite
	public GameObject projectile;

	private Animator animator;
	private SpriteRenderer spriteRenderer;
	//private GameObject thisProjectile;

	void Awake()
	{
		Debug.Log ("OnAwake");
		currentHP = hp;
		animator = GetComponent<Animator>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		actions = new BattleActions (1,1,true);
		Debug.Log ("Sprite");
		Debug.Log (spriteRenderer.sprite);
		canMove = true;
	}

	public void startTurn(){
		actions.startTurn ();
		SpriteRenderer sprRend = gameObject.GetComponent<SpriteRenderer> ();
		sprRend.material.shader = Shader.Find ("Sprites/Default");
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

	public int getAttacks(){
		return actions.getAtacks ();
	}

	public int getActions(){
		return actions.getActions ();
	}

	//Called when the player has verified moving checks (getActions)
	public void isMoving(){
		actions.takeAction (1);
		checkFatigue ();
	}

	public void checkFatigue(){
		if(!getTurn() && affiliation.Equals("Human")){
			SpriteRenderer sprRend = gameObject.GetComponent<SpriteRenderer> ();
			applyShadeEnd (sprRend);
		}
	}

	public void applyShadeEnd(SpriteRenderer sprRend){
		sprRend.material.shader = Shader.Find ("Custom/OverlayShaderRed");
	}

	public bool checkAttacks(){
		return actions.getAtacks () > 0 && getTurn ();
	}

	private void OnGUI() {
		Vector3 guiPosition = Camera.main.WorldToScreenPoint(transform.position);
		guiPosition.y = Screen.height - guiPosition.y;
		Rect rect = new Rect(guiPosition.x - tex.width/2, guiPosition.y - 3f * tex.height, tex.width * (float) currentHP/ (float) hp, tex.height);
		GUI.DrawTexture(rect, tex);
	}

	public bool isAttacked (int attack) {
		currentHP -= attack;
		if (currentHP <= 0) {
			gameObject.SetActive(false);
			//The unit isnt active anymore
			return false;
		}
		//The unit is still active
		return true;
	}

	public void isAttacking(BattleMeta enemy){
		if (enemy != null && checkAttacks()) {
			actions.takeAttack(1);
			if (projectile != null) {
				GameObject thisProjectile = Instantiate<GameObject> (projectile);
				thisProjectile.transform.position = transform.position;
				if (enemy.isActiveAndEnabled) {
					StartCoroutine (smooth_move (thisProjectile.transform, enemy.gameObject.transform, 1f));
				} else {
					thisProjectile.gameObject.SetActive (false);
				}
			}
			animator.SetTrigger ("UnitAttack");
			StartCoroutine (atk_blur (enemy.gameObject.transform));
			checkFatigue ();
		}
	}

	IEnumerator atk_blur(Transform unit){
		SpriteRenderer sprRend = unit.gameObject.GetComponent<SpriteRenderer> ();
		sprRend.material.shader = Shader.Find ("Custom/OverlayShaderHit");
		yield return new WaitForSeconds(.5f);
		if (actions.checkTurn () || !affiliation.Equals("Human")) {
			sprRend.material.shader = Shader.Find ("Sprites/Default");
		} else {
			checkFatigue();
		}
	}

	IEnumerator smooth_move(Transform origin, Transform end, float speed){
		float startime = Time.time;
		Vector3 start_pos = new Vector3(origin.position.x, origin.position.y, origin.position.z); //Starting position.
		Vector3 end_pos = end.position; //Ending position.
		// Debug.Log ("Start: " + start_pos.ToString ());
		// Debug.Log ("End: " + end_pos.ToString ());
		while (origin.position != end_pos/* && ((Time.time - startime)*speed) < 1f*/) { 
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
}

public class BattleMetaFactory : MonoBehaviour {
	
	void Start()
	{
		BattleMeta myScript = gameObject.AddComponent( typeof ( BattleMeta ) ) as BattleMeta;
	}
}
