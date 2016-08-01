﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/*
 * TODO:
 * Move board with drag
 * Zoom in and out with keys
 * Add a better check in each instance for enemy unit and player unit
 * 
 */
using AssemblyCSharp; 

public class AdventureGameManager : MonoBehaviour {

	public static AdventureGameManager instance = null;

	public int columns = 10;
	public int rows = 10;
	public GameObject general;

	private AdventureBoardManager boardSetup;
	private int level;

	void Awake()
	{
		if (instance == null){
			instance = this;
		} else if (instance != this) { 
			Destroy(gameObject);   
		} 
		DontDestroyOnLoad(gameObject);

		boardSetup = GetComponent<AdventureBoardManager>();

		level = 1;
	}

	void Start(){
		boardSetup.setupScene (instance, general);
	}

	public int getColumns() {
		return columns + level;
	}

	public int getRows() {
		return rows + level;
	}

	void Update()
	{
		if ( Input.GetMouseButtonDown (0)){ 
			Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
			RaycastHit2D [] hit = Physics2D.RaycastAll(ray,Vector2.zero,Mathf.Infinity,Physics2D.DefaultRaycastLayers);
			if (hit.Length > 0) {
				boardSetup.clicked (hit [0].transform.position);
			}
		}
	}

	public void returnToMenu()
	{
		Destroy (gameObject);
		Application.LoadLevel ("MainMenu");
	}
}