using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class TerrainTiles {

	public static int terrainSizeSmall = 5;
	public static int terrainSizeMedium = 10;
	public static int terrainSizeLarge = 20;

	//Left == south
	public static int[,] straightRoad = { 
		{ 0, 0, 0, 0, 0 }, 
		{ 0, 0, 0, 0, 0 }, 
		{ 2, 2, 2, 2, 2 }, 
		{ 0, 0, 0, 0, 0 }, 
		{ 0, 0, 0, 0, 0 }
	};

	public static int[,] curvyRoad = { 
		{ 0, 0, 2, 0, 0 }, 
		{ 0, 0, 2, 0, 0 }, 
		{ 2, 2, 2, 0, 0 }, 
		{ 0, 0, 0, 0, 0 }, 
		{ 0, 0, 0, 0, 0 }
	};

	public static int[,] curvyRoad2 = { 
		{ 0, 0, 2, 0, 0 }, 
		{ 0, 0, 2, 0, 0 }, 
		{ 0, 0, 2, 2, 2 }, 
		{ 0, 0, 0, 0, 0 }, 
		{ 0, 0, 0, 0, 0 }
	};

	public static int[,] curvyRoad3 = { 
		{ 0, 0, 0, 0, 0 }, 
		{ 0, 0, 0, 0, 0 }, 
		{ 0, 0, 2, 2, 2 }, 
		{ 0, 0, 2, 0, 0 }, 
		{ 0, 0, 2, 0, 0 }
	};

	public static int[,] curvyRoad4 = { 
		{ 0, 0, 0, 0, 0 }, 
		{ 0, 0, 0, 0, 0 }, 
		{ 2, 2, 2, 0, 0 }, 
		{ 0, 0, 2, 0, 0 }, 
		{ 0, 0, 2, 0, 0 }
	};

	public static int[,] tRoad = { 
		{ 0, 0, 2, 0, 0 }, 
		{ 0, 0, 2, 0, 0 }, 
		{ 2, 2, 2, 2, 2 }, 
		{ 0, 0, 0, 0, 0 }, 
		{ 0, 0, 0, 0, 0 }
	};

	public static int[,] intersectionRoad = { 
		{ 0, 0, 2, 0, 0 }, 
		{ 0, 0, 2, 0, 0 }, 
		{ 2, 2, 2, 2, 2 }, 
		{ 0, 0, 2, 0, 0 }, 
		{ 0, 0, 2, 0, 0 }
	};

	/*public static int[,] riverStraight = { 
		{ 0, 0, 3, 0, 0 }, 
		{ 0, 0, 3, 0, 0 }, 
		{ 0, 0, 3, 0, 0 }, 
		{ 0, 0, 3, 0, 0 }, 
		{ 0, 0, 3, 0, 0 } 
	};

	public static int[,] riverBridge = { 
		{ 0, 0, 3, 0, 0 }, 
		{ 0, 0, 3, 0, 0 }, 
		{ 2, 2, 2, 2, 2 }, 
		{ 0, 0, 3, 0, 0 }, 
		{ 0, 0, 3, 0, 0 } 
	};*/

	public static int[,] cliffRoad = { 
		{ 20, 20, 20, 20, 20 }, 
		{ 20, 20, 20, 20, 20 }, 
		{  2,  2,  2,  2,  2 }, 
		{ 20, 20, 20, 20, 20 }, 
		{ 20, 20, 20, 20, 20 } 
	};

	public static int[,] cliffTee = { 
		{ 20, 20,  2, 20, 20 }, 
		{ 20, 20,  2, 20, 20 }, 
		{  2,  2,  2,  2,  2 }, 
		{ 20, 20, 20, 20, 20 }, 
		{ 20, 20, 20, 20, 20 } 
	};

	public static int[,] cliffPlus = { 
		{ 20, 20,  2, 20, 20 }, 
		{ 20, 20,  2, 20, 20 }, 
		{  2,  2,  2,  2,  2 }, 
		{ 20, 20,  2, 20, 20 }, 
		{ 20, 20,  2, 20, 20 } 
	};

	public static int[,] smallCityRoad = { 
		{ 0, 2, 2, 2, 2 }, 
		{ 0, 2,10,11, 2 }, 
		{ 0, 2,16,16, 2 }, 
		{ 0, 2, 2, 2, 2 }, 
		{ 0, 0, 2, 0, 0 } 
	};

	public static int[,] smallCityRoadAlt = { 
		{ 2, 2, 2, 2, 0 }, 
		{ 2, 10,11,2, 0 }, 
		{ 2, 16,16,2, 0 }, 
		{ 2, 2, 2, 2, 0 }, 
		{ 0, 0, 2, 0, 0 } 
	};

	public static int[,] smallCityRoad2 = { 
		{ 0, 2, 2, 2, 2 }, 
		{ 0, 2,10,12, 2 }, 
		{ 0, 2,17,17, 2 }, 
		{ 0, 2, 2, 2, 2 }, 
		{ 0, 0, 2, 0, 0 } 
	};

	public static int[,] smallCityRoadAlt2 = { 
		{ 2, 2, 2, 2, 0 }, 
		{ 2, 10,12,2, 0 }, 
		{ 2, 17,17,2, 0 }, 
		{ 2, 2, 2, 2, 0 }, 
		{ 0, 0, 2, 0, 0 } 
	};

	public static int[,] smallCityRoad3 = { 
		{ 0, 2, 2, 2, 2 }, 
		{ 0, 2,10,13, 2 }, 
		{ 0, 2,18,18, 2 }, 
		{ 0, 2, 2, 2, 2 }, 
		{ 0, 0, 2, 0, 0 } 
	};

	public static int[,] smallCityRoadAlt3 = { 
		{ 2, 2, 2, 2, 0 }, 
		{ 2, 10,13,2, 0 }, 
		{ 2, 18,18,2, 0 }, 
		{ 2, 2, 2, 2, 0 }, 
		{ 0, 0, 2, 0, 0 } 
	};

	/*public static int[,] cityRoad = { 
		{ 0, 0, 2, 0, 0,  0,  0, 2, 0, 0 }, 
		{ 0, 2, 2, 2, 2,  2,  2, 2, 2, 0 }, 
		{ 2, 2, 0, 0, 0,  0,  0, 0, 2, 2 }, 
		{ 0, 2, 0, 0, 0,  0,  0, 0, 2, 0 }, 
		{ 0, 2, 0, 0, 12, 11, 0, 0, 2, 0 }, 
		{ 0, 2, 0, 0, 12, 12, 0, 0, 2, 0 }, 
		{ 0, 2, 0, 0, 2,  2,  0, 0, 2, 0 }, 
		{ 2, 2, 0, 0, 2,  2,  0, 0, 2, 2 }, 
		{ 0, 2, 2, 2, 2,  2,  2, 2, 2, 0 }, 
		{ 0, 0, 2, 0, 0,  0,  0, 2, 0, 0 }, 
	};*/

	/*public static int[,] cityRoadAlt = { 
		{ 0, 0, 2, 0, 0,  0,  0, 2, 0, 0 }, 
		{ 0, 2, 2, 2, 2,  2,  2, 2, 2, 0 }, 
		{ 2, 2, 0, 0, 0,  0,  0, 0, 2, 2 }, 
		{ 0, 2, 0, 0, 0,  0,  0, 0, 2, 0 }, 
		{ 0, 2, 2, 2, 12, 12, 0, 0, 2, 0 }, 
		{ 0, 2, 2, 2, 12, 11, 0, 0, 2, 0 }, 
		{ 0, 2, 0, 0, 0,  0,  0, 0, 2, 0 }, 
		{ 2, 2, 0, 0, 0,  0,  0, 0, 2, 2 }, 
		{ 0, 2, 2, 2, 2,  2,  2, 2, 2, 0 }, 
		{ 0, 0, 2, 0, 0,  0,  0, 2, 0, 0 }, 
	};*/

	public static List<int[,]> returnTiles(){
		List<int[,]> tiles = new List<int[,]> ();
		tiles.Add (TerrainTiles.straightRoad);
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.straightRoad));
		tiles.Add (TerrainTiles.curvyRoad);
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.curvyRoad));
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.curvyRoad)));
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.curvyRoad))));
		tiles.Add (TerrainTiles.cliffRoad);
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.cliffRoad));
		tiles.Add (TerrainTiles.cliffPlus);
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.cliffPlus));
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.cliffPlus)));
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.cliffPlus))));
		tiles.Add (TerrainTiles.cliffPlus);
		tiles.Add (TerrainTiles.tRoad);
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.tRoad));
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.tRoad)));
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.tRoad))));
		tiles.Add (TerrainTiles.intersectionRoad);
		tiles.Add (TerrainTiles.smallCityRoad);
		tiles.Add (TerrainTiles.smallCityRoadAlt);
		tiles.Add (TerrainTiles.smallCityRoad2);
		tiles.Add (TerrainTiles.smallCityRoadAlt2);
		tiles.Add (TerrainTiles.smallCityRoad3);
		tiles.Add (TerrainTiles.smallCityRoadAlt3);

		//tiles.Add (TerrainTiles.riverStraight);
		//tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.riverStraight));

		//tiles.Add (TerrainTiles.riverBridge);
		//tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.riverBridge));
		//tiles.Add (TerrainTiles.smallCityRoadAlt);
		//tiles.Add (TerrainTiles.cityRoad);
		//tiles.Add (TerrainTiles.cityRoadAlt);
		return tiles;
	}

}
