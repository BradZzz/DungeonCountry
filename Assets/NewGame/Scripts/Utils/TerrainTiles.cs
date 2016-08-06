using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class TerrainTiles {

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

	public static int[,] cityRoad = { 
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
	};

	public static int[,] cityRoadAlt = { 
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
	};

	public static List<int[,]> returnTiles(){
		List<int[,]> tiles = new List<int[,]> ();
		tiles.Add (TerrainTiles.straightRoad);
		tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.straightRoad));
		tiles.Add (TerrainTiles.curvyRoad);
		tiles.Add (TerrainTiles.curvyRoad2);
		//tiles.Add (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.curvyRoad4)));
		tiles.Add (TerrainTiles.curvyRoad3);
		tiles.Add (TerrainTiles.curvyRoad4);
		//tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.curvyRoad));
		//tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.curvyRoad));
		//tiles.Add (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.curvyRoad))));
		//tiles.Add (TerrainTiles.tRoad);
		//tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.tRoad));
		//tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.tRoad));
		//tiles.Add (Coroutines.RotateMatrixCounterClockwise (TerrainTiles.tRoad));
		//tiles.Add (TerrainTiles.intersectionRoad);
		//tiles.Add (TerrainTiles.cityRoad);
		//tiles.Add (TerrainTiles.cityRoadAlt);
		return tiles;
	}

}
