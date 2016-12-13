using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStatics {

	public static readonly Dictionary<int, string> factionDict = new Dictionary<int, string>
	{
		{ 1, "Human" },
		{ 2, "Gnome" },
		{ 3, "Death" },
		{ 4, "Human" }
	};

	//Base stats
	// <================>
	// Attack
	// Hp
	// <================>
	//The level-up tree
	// <================>
	// Magic => Human -> Death -> Fae -> Arcane
	// Tactics => Placement / Speed / Resource Generation / Base Capture
	// Leadership => Battle Attack / HP / Range / Abilities
	// Journeyman => Adventure Movement / Resource Pickup Gain / Treasure Pickup Gain / Other Player Spying / Other Player Sabotage
	// <================>
}
