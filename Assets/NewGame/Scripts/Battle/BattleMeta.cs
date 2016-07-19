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

	//What team are they on
	public string affiliation;

	private bool canMove;
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
		currentHP = hp;
		animator = GetComponent<Animator>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		Debug.Log ("Sprite");
		Debug.Log (spriteRenderer.sprite);
		canMove = true;
	}

	public bool getMove(){
		return canMove;
	}

	public void setMove(bool canMove){
		this.canMove = canMove;
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

	public void isAttacking(BattleMeta enemy){
		if (enemy != null) {
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
		}
	}

	IEnumerator atk_blur(Transform unit){
		SpriteRenderer sprRend = unit.gameObject.GetComponent<SpriteRenderer> ();
		sprRend.material.shader = Shader.Find ("Custom/OverlayShaderRed");
		yield return new WaitForSeconds(.5f);
		sprRend.material.shader = Shader.Find ("Sprites/Default");
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
