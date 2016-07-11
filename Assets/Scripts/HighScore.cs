using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HighScore : MonoBehaviour {

	Text highScoreText;
	private int currentHigh;

	// Use this for initialization
	void Start () {
		currentHigh = PlayerPrefs.GetInt ("High Score");
		highScoreText = gameObject.GetComponent<Text>(); 
		highScoreText.text = "High Score: " + currentHigh;
	}

	// Update is called once per frame
	void Update () {
		currentHigh = PlayerPrefs.GetInt ("High Score");
		highScoreText.text = "High Score: " + currentHigh;
	}
}
