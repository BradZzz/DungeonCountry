using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ButtonClick : MonoBehaviour {
	public void click() {
		Application.LoadLevel ("PuzzleScene");
	}
}
