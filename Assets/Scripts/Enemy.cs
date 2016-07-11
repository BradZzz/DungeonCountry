using UnityEngine;
using System.Collections;

//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MovingObject
{
	public int playerDamage;                            //The amount of food points to subtract from the player when attacking.
	public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
	public AudioClip attackSound2;						//Second of two audio clips to play when attacking the player.
	private SpriteRenderer spriteRenderer; 				//The texture for the enemy

	private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
	private Transform target;                           //Transform to attempt to move toward each turn.
	//private bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.
	private int skipMoves;
	private int skipMax;
	private AssemblyCSharp.PaletteManager paletteManager = null;
	private MaterialPropertyBlock block = null;
	private bool changed = false;

	void Awake ()
	{
		//Get a component reference to the SpriteRenderer.
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}

	private void Update ()
	{
		Debug.Log ("Enemy Sprite: " + spriteRenderer.sprite);
		if (!changed && spriteRenderer.sprite != null) {
			Debug.Log ("Changing outfits...");
			changed = true;
			paletteManager = new AssemblyCSharp.PaletteManager ();
			if (this.gameObject.name.Equals("Enemy3(Clone)") || this.gameObject.name.Equals("Enemy4(Clone)")) {
				Color lightRed = new Color (124 / 255.0F, 73 / 255.0F, 73 / 255.0F, 1);
				Color darkRed = new Color (100 / 255.0F, 47 / 255.0F, 47 / 255.0F, 1);

				Color lightBlue = new Color (62 / 255.0F, 66 / 255.0F, 112 / 255.0F, 1);
				Color darkBlue = new Color (47 / 255.0F, 52 / 255.0F, 97 / 255.0F, 1);

				paletteManager.CreatePalette ("enemyBlue", spriteRenderer);
				paletteManager.SwitchColor ("enemyBlue", lightRed, lightBlue);
				paletteManager.SwitchColor ("enemyBlue", darkRed, darkBlue);

				block = new MaterialPropertyBlock ();
				block.SetTexture ("_MainTex", paletteManager.generateSprite ("enemyBlue"));

				paletteManager.printPalette ("enemyBlue");
			}
		}
	}

	//Start overrides the virtual Start function of the base class.
	protected override void Start ()
	{
		//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
		//This allows the GameManager to issue movement commands.
		GameManager.instance.AddEnemyToList (this);

		//Debug.Log ("Enemy name: " + this.gameObject.name);
		//Debug.Log ("Enemy tag: " + this.gameObject.tag);
		//Debug.Log ("Enemy equal: " + (this.gameObject.name.Equals("Enemy1(Clone)") || this.gameObject.name.Equals("Enemy2(Clone)")));

		//Debug.Log (spriteRenderer);
		//Debug.Log (spriteRenderer.sprite);

		/*if (this.gameObject.name.Equals("Enemy(Clone)")) {
			Color lightRed = new Color (122 / 255.0F, 72 / 255.0F, 72 / 255.0F, 1);
			Color darkRed = new Color (99 / 255.0F, 47 / 255.0F, 47 / 255.0F, 1);

			Color lightBlue = new Color (62 / 255.0F, 66 / 255.0F, 112 / 255.0F, 1);
			Color darkBlue = new Color (47 / 255.0F, 52 / 255.0F, 97 / 255.0F, 1);

			paletteManager.CreatePalette ("enemyBlue", spriteRenderer);
			paletteManager.SwitchColor ("enemyBlue", lightRed, lightBlue);
			paletteManager.SwitchColor ("enemyBlue", darkRed, darkBlue);

			block = new MaterialPropertyBlock ();
			block.SetTexture ("_MainTex", paletteManager.generateSprite ("enemyBlue"));
		}*/

		//Get and store a reference to the attached Animator component.
		animator = GetComponent<Animator> ();

		//Find the Player GameObject using it's tag and store a reference to its transform component.
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		skipMax = Random.Range (2, 5);
		skipMoves = 0;

		//Call the start function of our base class MovingObject.
		base.Start ();
	}


	//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
	//See comments in MovingObject for more on how base AttemptMove function works.
	protected override bool AttemptMove <T> (int xDir, int yDir)
	{
		//Check if skipMove is true, if so set it to false and skip this turn.
		if(skipMoves < skipMax)
		{
			skipMoves++;
			return false;

		}

		//Call the AttemptMove function from MovingObject.
		bool moved = base.AttemptMove <T> (xDir, yDir);

		if (moved){
			skipMoves = 0;
		}

		return moved;
	}


	//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
	public void MoveEnemy ()
	{
		//Declare variables for X and Y axis move directions, these range from -1 to 1.
		//These values allow us to choose between the cardinal directions: up, down, left and right.
		int xDir = 0;
		int yDir = 0;
		bool moved = false;

		//If the difference in positions is approximately zero (Epsilon) do the following:
		if (Mathf.Abs (target.position.y - transform.position.y) > float.Epsilon) { AttemptMove <Player> (0, target.position.y > transform.position.y ? 1 : -1); } 

		if (Mathf.Abs (target.position.x - transform.position.x) > float.Epsilon) { AttemptMove <Player> (target.position.x > transform.position.x ? 1 : -1, 0); }

		Debug.Log ("Enemy X: " + xDir + " Enemy Y: " + yDir);

		//Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
		//AttemptMove <Player> (xDir, yDir);

		/*if (!AttemptMove <Player> (xDir, yDir) && !moved) {
			skipMove = true;
			AttemptMove <Player> (target.position.x > transform.position.x ? 1 : -1, 0);
		} else if (!moved) {
			skipMove = true;
			if (Random.Range (0, 1) == 0) {
				AttemptMove <Player> (Random.Range (-1, 1), 0);
			} else {
				AttemptMove <Player> (0, Random.Range (-1, 1));
			}
		}*/
	}


	//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
	//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
	protected override void OnCantMove <T> (T component)
	{
		//Declare hitPlayer and set it to equal the encountered component.
		Player hitPlayer = component as Player;

		//Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
		hitPlayer.LoseFood (playerDamage);

		//Set the attack trigger of animator to trigger Enemy attack animation.
		animator.SetTrigger ("EnemyAttack");

		SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
	}

	void LateUpdate(){
		if (block != null) {
			spriteRenderer.SetPropertyBlock (block);
		}
	}
}
