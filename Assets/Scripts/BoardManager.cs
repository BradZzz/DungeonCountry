using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.


public class BoardManager : MonoBehaviour {

	// Using Serializable allows us to embed a class with sub properties in the inspector.
	[Serializable]
	public class Count
	{
		public int minimum;             //Minimum value for our Count class.
		public int maximum;             //Maximum value for our Count class.


		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}


	public int columns = 8;                                         //Number of columns in our game board.
	public int rows = 8;                                            //Number of rows in our game board.
	public Count wallCount = new Count (5, 9);                      //Lower and upper limit for our random number of walls per level.
	public Count foodCount = new Count (1, 5);                      //Lower and upper limit for our random number of food items per level.
	public Count potionCount = new Count (0, 1);                      //Lower and upper limit for our random number of food items per level.
	public GameObject exit;                                         //Prefab to spawn for exit.
	public GameObject[] floorTiles;                                 //Array of floor prefabs.
	public GameObject[] wallTiles;                                  //Array of wall prefabs.
	public GameObject[] foodTiles;                                  //Array of food prefabs.
	public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
	public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.
	public GameObject[] potionTiles;                                //Array of potion tile prefabs.

	private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
	private List <Vector3> gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.


	//Clears our list gridPositions and prepares it to generate a new board.
	void InitialiseList (int level)
	{
		//Clear our list gridPositions.
		gridPositions.Clear ();

		AssemblyCSharp.MazeGenerator maze = new AssemblyCSharp.MazeGenerator (new Vector2(0, 0), new Vector2(getColumns(level) - 1, getRows(level) - 1), new Vector2(getColumns(level), getRows(level)));
		bool[,] map = maze.getMap ();

		for(int x = 0; x < getColumns(level); x++)
		{
			for(int y = 0; y < getRows(level); y++)
			{
				if (map [x, y]) {
					Instantiate (wallTiles [Random.Range (0, wallTiles.Length)], new Vector3 (x, y, 0), Quaternion.identity);
				} else if ((x != 0 && y != 0) && (x != getColumns(level) - 1 && y != getRows(level) - 1)) {
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}
	}

	private int getColumns(int level) {
		return columns + level;
	}

	private int getRows(int level) {
		return rows + level;
	}

	//Sets up the outer walls and floor (background) of the game board.
	void BoardSetup (int level)
	{
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;

		//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
		for(int x = -1; x < getColumns(level) + 1; x++)
		{
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for(int y = -1; y < getRows(level) + 1; y++)
			{
				//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
				GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];

				//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
				if (x == -1 || x == getColumns(level) || y == -1 || y == getRows(level)) {
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
				}

				//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
				GameObject instance =
					Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

				//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
				instance.transform.SetParent (boardHolder);
			}
		}
	}


	//RandomPosition returns a random position from our list gridPositions.
	Vector3 RandomPosition ()
	{
		//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
		int randomIndex = Random.Range (0, gridPositions.Count);

		//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
		Vector3 randomPosition = gridPositions[randomIndex];

		//Remove the entry at randomIndex from the list so that it can't be re-used.
		gridPositions.RemoveAt (randomIndex);

		//Return the randomly selected Vector3 position.
		return randomPosition;
	}

	//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		//Choose a random number of objects to instantiate within the minimum and maximum limits
		int objectCount = Random.Range (minimum, maximum+1);

		//Instantiate objects until the randomly chosen limit objectCount is reached
		for(int i = 0; i < objectCount; i++)
		{
			//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
			Vector3 randomPosition = RandomPosition();

			//Choose a random tile from tileArray and assign it to tileChoice
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];

			//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}

	public void replacePotionsFood () {

		Debug.Log ("Replacing Potions");
		foreach (object o in GameObject.FindObjectsOfType(typeof (GameObject)))
		{
			if (((GameObject) o).tag == "Potion") {
				Debug.Log ("Potion!");

				Vector3 position = ((GameObject) o).transform.position;
				GameObject tileChoice = foodTiles[Random.Range (0, foodTiles.Length)];
				Destroy (((GameObject) o).gameObject);
				Instantiate(tileChoice, position, Quaternion.identity);
			}
		}
	}

	public GameObject[] scaleDifficulty (int level, GameObject[] allObjects, int baseAmount, int scaleAmount){
		int scale = baseAmount + level / scaleAmount >= allObjects.Length ? allObjects.Length : baseAmount + level / scaleAmount;
		GameObject[] levelObjects = new GameObject[scale];
		for (int i = 0; i < levelObjects.Length; i++) {
			levelObjects [i] = allObjects [i];
		}
		return levelObjects;
	}

	//SetupScene initializes our level and calls the previous functions to lay out the game board
	public void SetupScene (int level)
	{
		//Creates the outer walls and floor.
		BoardSetup (level);

		//Reset our list of gridpositions.
		InitialiseList (level);

		//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
		//LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);

		//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (foodTiles, (int) (foodCount.minimum + (level * .6)), (int) (foodCount.maximum + (level * .6)));

		//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (potionTiles, (int) (potionCount.minimum + (level * .35)), (int) (potionCount.maximum + (level * .35)));

		//Determine number of enemies based on current level number, based on a logarithmic progression
		int enemyCount = (int)Mathf.Log(level, 2f);

		//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (scaleDifficulty(level, enemyTiles, 2, 4), enemyCount, enemyCount);

		//Instantiate the exit tile in the upper right hand corner of our game board
		Instantiate (exit, new Vector3 (getColumns(level) - 1, getRows(level) - 1, 0f), Quaternion.identity);
	}
}
