// Core engine code taken from tutorials by AlexStv http://alexstv.com/ please review his license at http://alexstv.com/index.php/posts/unity-voxel-tutorial-licencing 

using UnityEngine;
using SimplexNoise;

namespace VRVox
{
	public class TerrainGen : MonoBehaviour
	{
		public Color colorA = Color.blue;
		public Color colorB = Color.blue;

		public float StoneBaseHeight = -24;
		public float StoneBaseNoise = 0.05f;
		public float StoneBaseNoiseHeight = 4;
		 
		public float StoneMountainHeight = 48;
		public float StoneMountainFrequency = 0.008f;
		public float StoneMinHeight = -12;
		 
		public float DirtBaseHeight = 1;
		public float DirtNoise = 0.04f;
		public float DirtNoiseHeight = 3;

		public int randomSeed = 10;

		public Chunk ChunkGen(Chunk chunk)
		{
			for (var x = chunk.Pos.x; x < chunk.Pos.x + World.ChunkSize; x++)
			{
				for (var z = chunk.Pos.z; z < chunk.Pos.z + World.ChunkSize; z++)
				{
					chunk = ChunkColumnGen(chunk, x, z);
				}
			}
			return chunk;
		}

		public Chunk ChunkColumnGen(Chunk chunk, int x, int z)
		{
			var stoneHeight = Mathf.FloorToInt(StoneBaseHeight);
			//stoneHeight += GetNoise(x, 0, z, StoneMountainFrequency, Mathf.FloorToInt(StoneMountainHeight));

			stoneHeight += GetNoisePerlin(x + randomSeed, 0, z + randomSeed, StoneMountainFrequency, Mathf.FloorToInt(StoneMountainHeight));

			//stoneHeight += Mathf.PerlinNoise(x, z);

			if (stoneHeight < StoneMinHeight)
				stoneHeight = Mathf.FloorToInt(StoneMinHeight);

			stoneHeight += GetNoise(x + randomSeed, 0, z + randomSeed, StoneBaseNoise, Mathf.FloorToInt(StoneBaseNoiseHeight));

			var dirtHeight = stoneHeight + Mathf.FloorToInt(DirtBaseHeight);
			dirtHeight += GetNoise(x, 100, z, DirtNoise, Mathf.FloorToInt(DirtNoiseHeight));

			for (var y = chunk.Pos.y; y < chunk.Pos.y + World.ChunkSize; y++)
			{
				if (y <= stoneHeight)
				{
					//chunk.SetBlock(x - chunk.Pos.x, y - chunk.Pos.y, z - chunk.Pos.z, new Block(colorB));

					chunk.SetBlock(x - chunk.Pos.x, y - chunk.Pos.y, z - chunk.Pos.z, VoxBlockType.Default, colorB);
				}
				else if (y <= dirtHeight)
				{
					//chunk.SetBlock(x - chunk.Pos.x, y - chunk.Pos.y, z - chunk.Pos.z, new Block(colorA));
					chunk.SetBlock(x - chunk.Pos.x, y - chunk.Pos.y, z - chunk.Pos.z, VoxBlockType.Default, colorA);
				}
				else
				{
					//chunk.SetBlock(x - chunk.Pos.x, y - chunk.Pos.y, z - chunk.Pos.z, new BlockEmpty());
					chunk.SetBlock(x - chunk.Pos.x, y - chunk.Pos.y, z - chunk.Pos.z, VoxBlockType.Empty, Color.white);
				}

			}

			return chunk;
		}

		public static int GetNoise(int x, int y, int z, float scale, int max)
		{
			return Mathf.FloorToInt((Noise.Generate(x*scale, y*scale, z*scale) + 1f)*(max*0.5f));
		}

		public static int GetNoisePerlin(int x, int y, int z, float scale, int max)
		{
			return Mathf.FloorToInt(((Mathf.PerlinNoise(x * scale, y * scale) + 1f) * (max * 0.5f)));
		}
	}
}