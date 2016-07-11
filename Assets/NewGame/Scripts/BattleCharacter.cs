using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BattleCharacter : MovingObject {

	/*public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
	public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
	public int pointsPerSoda = 20;              //Number of points to add to player food points when picking up a soda object.
	public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
	public Text foodText;                       //UI Text to display current player food total.
	public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
	public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
	public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
	public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
	public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
	public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
	public AudioClip gameOverSound;             //Audio clip to play when player dies.*/
	public int movementDistance;

	private Animator animator;                  //Used to store a reference to the Player's animator component.
	//private int food;                           //Used to store player food points total during level.
	private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.

	private SpriteRenderer spriteRenderer;      //Used to hold a reference to the player sprite for upgrades
	//private Shader shaderSpritesPotion;
	//private Shader shaderSpritesDefault;

	private MaterialPropertyBlock block = null;
	private AssemblyCSharp.PaletteManager paletteManager = null;

	Camera mainCamera;
	private Vector3 camMovingPos;
	float smoothTime = 0.2F;
	Vector3 velocity = Vector3.zero;

	void Awake ()
	{
		//Get a component reference to the SpriteRenderer.
		Debug.Log("Here!");
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		Debug.Log (mainCamera.transform.position.ToString());
	}

	//Start overrides the Start function of MovingObject
	protected override void Start ()
	{
		paletteManager = new AssemblyCSharp.PaletteManager ();
		//shaderSpritesPotion = Shader.Find("GUI/Text Shader");
		//shaderSpritesDefault = Shader.Find("Sprites/Default");

		camMovingPos = new Vector3 (transform.position.x, transform.position.y, mainCamera.transform.position.z);

		//Get a component reference to the Player's animator component
		animator = GetComponent<Animator>();

		//Get the current food point total stored in GameManager.instance between levels.
		//food = BattleGameManager.instance.playerFoodPoints;

		//Set the foodText to reflect the current player food total.
		//foodText.text = "Food: " + food;

		//Call the Start function of the MovingObject base class.
		base.Start ();
	}


	//This function is called when the behaviour becomes disabled or inactive.
	private void OnDisable ()
	{
		//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
		//BattleGameManager.instance.playerFoodPoints = food;
	}


	private void Update ()
	{

		/*if ( Input.GetMouseButtonDown (0)){ 
			Debug.Log("Clicked on button");
			//RaycastHit hit; 
			Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
			RaycastHit2D hit = (Physics2D.Raycast (ray,Vector2.zero,Mathf.Infinity));

			if (hit) {
				Debug.Log ("Hit!");
				BattleGameManager.instance.boardClicked (hit.transform, movementDistance);
			} else {
				Debug.Log ("Miss...");
			}
		}*/

		/*if ( Input.GetMouseButtonDown (0)){ 
			Debug.Log("Clicked on button");
			RaycastHit hit; 
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
			if ( Physics.Raycast (ray,out hit,100.0f)) {
				Debug.Log("Hit object!");
				Debug.Log (hit.transform.name);
				//StartCoroutine(ScaleMe(hit.transform));
				//Debug.Log("You selected the " + hit.transform.name); // ensure you picked right object
			}
		}*/

		//If it's not the player's turn, exit the function.
		//if(!BattleGameManager.instance.playersTurn) return;

		int horizontal = 0;     //Used to store the horizontal move direction.
		int vertical = 0;       //Used to store the vertical move direction.

		//Check if we are running either in the Unity editor or in a standalone build.
		/*#if UNITY_STANDALONE || UNITY_WEBPLAYER

		//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));

		//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
		vertical = (int) (Input.GetAxisRaw ("Vertical"));

		Debug.Log("hor: " + horizontal + " vert: " + vertical);

		//Check if moving horizontally, if so set vertical to zero.
		if(horizontal != 0)
		{
			vertical = 0;
		}
		//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		//Check if Input has registered more than zero touches
		if (Input.touchCount > 0)
		{
		//Store the first touch detected.
		Touch myTouch = Input.touches[0];

		//Check if the phase of that touch equals Began
		if (myTouch.phase == TouchPhase.Began)
		{
		//If so, set touchOrigin to the position of that touch
		touchOrigin = myTouch.position;
		}

		//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
		else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
		{
		//Set touchEnd to equal the position of this touch
		Vector2 touchEnd = myTouch.position;

		//Calculate the difference between the beginning and end of the touch on the x axis.
		float x = touchEnd.x - touchOrigin.x;

		//Calculate the difference between the beginning and end of the touch on the y axis.
		float y = touchEnd.y - touchOrigin.y;

		//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
		touchOrigin.x = -1;

		//Check if the difference along the x axis is greater than the difference along the y axis.
		if (Mathf.Abs(x) > Mathf.Abs(y))
		//If x is greater than zero, set horizontal to 1, otherwise set it to -1
		horizontal = x > 0 ? 1 : -1;
		else
		//If y is greater than zero, set horizontal to 1, otherwise set it to -1
		vertical = y > 0 ? 1 : -1;
		}
		}*/

		//#endif //End of mobile platform dependendent compilation section started above with #elif
		//Check if we have a non-zero value for horizontal or vertical
		if(horizontal != 0 || vertical != 0)
		{
			//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
			//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
			AttemptMove<Wall> (horizontal, vertical);
		}

		checkDamp ();
	}

	//AttemptMove overrides the AttemptMove function in the base class MovingObject
	//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
	protected override bool AttemptMove <T> (int xDir, int yDir)
	{
		//Every time player moves, subtract from food points total.
		//food--;

		//Update food text display to reflect current score.
		//foodText.text = "Food: " + food;

		//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
		bool moved = base.AttemptMove <T> (xDir, yDir);

		//Hit allows us to reference the result of the Linecast done in Move.
		RaycastHit2D hit;

		//If Move returns true, meaning Player was able to move into an empty space.
		if (Move (xDir, yDir, out hit)) 
		{
			//SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);

			//Debug.Log ("DiffX: " + diff(transform.position.x, mainCamera.transform.position.x));
			//Debug.Log ("DiffY: " + diff(transform.position.y, mainCamera.transform.position.y));

			if (diff(transform.position.x, mainCamera.transform.position.x) > 2 || diff(transform.position.y, mainCamera.transform.position.y) > 1.5) {
				camMovingPos = new Vector3 (transform.position.x, transform.position.y, mainCamera.transform.position.z);
			}
		}

		//Since the player has moved and lost food points, check if the game has ended.
		CheckIfGameOver ();

		//Set the playersTurn boolean of GameManager to false now that players turn is over.
		//BattleGameManager.instance.playersTurn = false;

		return moved;
	}

	private int diff (float x1, float x2) {
		return (int)(Mathf.Abs (x1 - x2));
	}

	private void checkDamp(){
		if(camMovingPos.x != mainCamera.transform.position.x || camMovingPos.y != mainCamera.transform.position.y) {
			mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, camMovingPos, ref velocity, smoothTime);
		}
	}

	//OnCantMove overrides the abstract function OnCantMove in MovingObject.
	//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
	protected override void OnCantMove <T> (T component)
	{
		//Set hitWall to equal the component passed in as a parameter.
		//Wall hitWall = component as Wall;

		//Call the DamageWall function of the Wall we are hitting.
		//hitWall.DamageWall (wallDamage);

		//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
		animator.SetTrigger ("PlayerChop");
	}


	//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
	private void OnTriggerEnter2D (Collider2D other)
	{
		//Check if the tag of the trigger collided with is Exit.
		/*if(other.tag == "Exit")
		{

			//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
			Invoke ("Restart", restartLevelDelay);

			//Disable the player object since level is over.
			enabled = false;
		}

		//Check if the tag of the trigger collided with is Food.
		else if(other.tag == "Food")
		{
			//Add pointsPerFood to the players current food total.
			food += pointsPerFood;

			//Update foodText to represent current total and notify player that they gained points
			foodText.text = "+" + pointsPerFood + " Food: " + food;

			//Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
			SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);

			//Disable the food object the player collided with.
			other.gameObject.SetActive (false);
		}

		//Check if the tag of the trigger collided with is Soda.
		else if(other.tag == "Soda")
		{
			//Add pointsPerSoda to players food points total
			food += pointsPerSoda;

			//Update foodText to represent current total and notify player that they gained points
			foodText.text = "+" + pointsPerSoda + " Food: " + food;

			//Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);

			//Disable the soda object the player collided with.
			other.gameObject.SetActive (false);
		}
		else if(other.tag == "Potion")
		{
			if (wallDamage != 4) {
				//Super wall damage!!!
				wallDamage = 4;

				Color lightGreen = new Color (104 / 255.0F, 143 / 255.0F, 104 / 255.0F, 1);
				Color darkGreen = new Color (86 / 255.0F, 101 / 255.0F, 86 / 255.0F, 1);

				Color lightRed = new Color (176 / 255.0F, 60 / 255.0F, 60 / 255.0F, 1);
				Color darkRed = new Color (122 / 255.0F, 72 / 255.0F, 72 / 255.0F, 1);

				Color lightPurple = new Color (151 / 255.0F, 0, 151 / 255.0F, 1);
				Color darkPurple = new Color (121 / 255.0F, 0, 121 / 255.0F, 1);

				paletteManager.CreatePalette ("playerRed", spriteRenderer);
				paletteManager.SwitchColor ("playerRed", lightGreen, lightPurple);
				paletteManager.SwitchColor ("playerRed", darkGreen, darkPurple);
				block = new MaterialPropertyBlock ();
				//block.AddTexture ("_MainTex", paletteManager.generateSprite ("playerRed"));
				block.SetTexture ("_MainTex", paletteManager.generateSprite ("playerRed"));

				//Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);

				//Disable the soda object the player collided with.
				other.gameObject.SetActive (false);

				BattleGameManager.instance.DestroyPotions ();
			}
		}*/
	}

	//Restart reloads the scene when called.
	private void Restart ()
	{
		//init()
		//wallDamage = 1;

		//BattleGameManager.instance.reloadLevel = true;

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}


	//LoseFood is called when an enemy attacks the player.
	//It takes a parameter loss which specifies how many points to lose.
	public void LoseFood (int loss)
	{
		//Set the trigger for the player animator to transition to the playerHit animation.
		animator.SetTrigger ("PlayerHit");

		//Subtract lost food points from the players total.
		//food -= loss;

		//Update the food display with the new total.
		//foodText.text = "-"+ loss + " Food: " + food;

		//Check to see if game has ended.
		CheckIfGameOver ();
	}


	//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
	private void CheckIfGameOver ()
	{
		//Check if food point total is less than or equal to zero.
		/*if (food <= 0) 
		{
			//Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
			SoundManager.instance.PlaySingle (gameOverSound);

			//Stop the background music.
			SoundManager.instance.musicSource.Stop();

			//Call the GameOver function of GameManager.
			BattleGameManager.instance.GameOver ();
		}*/
	}

	void LateUpdate(){
		if (block != null) {
			spriteRenderer.SetPropertyBlock (block);
		}
	}
}
