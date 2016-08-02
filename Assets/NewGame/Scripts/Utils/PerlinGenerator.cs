using UnityEngine;
using System.Collections;

public class PerlinGenerator {
	
	public static float scale = 1.0F;

	/*public static float[,] calcNoise (Vector2 dimensions){
		float[,] map = GenerateWhiteNoise(dimensions);
		for (int y = 0; y < dimensions.y; y++) {
			for (int x = 0; x < dimensions.x; x++) {
				float xCoord = map[x,y] + x / dimensions.x * scale;
				float yCoord = map[x,y] + y / dimensions.y * scale;
				//map[x,y] = Mathf.PerlinNoise(xCoord, yCoord);
				map[x,y] = Mathf.PerlinNoise((int) xCoord, (int) yCoord);
			}
		}
		return map;
	}

	private static float[,] GenerateWhiteNoise(Vector2 dimensions)
	{
		float[,] noise = new float[(int)dimensions.x,(int)dimensions.y];

		for (int x = 0; x < dimensions.x; x++)
		{
			for (int y = 0; y < dimensions.y; y++)
			{
				noise[x,y] = UnityEngine.Random.Range (0, 1);
			}
		}

		return noise;
	}*/

	public static float[,] calcNoise(Vector2 dimensions)
	{
		float[,] map = new float[(int)dimensions.x, (int)dimensions.y];

		for (int i = 0; i < dimensions.x; i++)
		{
			for (int k = 0; k < dimensions.y; k++)
			{
				map[i, k] = Mathf.PerlinNoise(((float)i / (float)dimensions.x) * scale, ((float)k / (float)dimensions.y) * scale);
			}
		}

		return map;
	}

}