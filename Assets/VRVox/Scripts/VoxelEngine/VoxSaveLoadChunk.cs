// Core engine code taken from tutorials by AlexStv http://alexstv.com/ please review his license at http://alexstv.com/index.php/posts/unity-voxel-tutorial-licencing 

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VRVox
{
	[Serializable]
	public class SavedWorldInfo
	{
		public List<SavedChunkInfo> chunkInfos;

		public VoxColor colorA;
		public VoxColor colorB;
		public int terrainSeed;


		public SavedWorldInfo()
		{
			chunkInfos = new List<SavedChunkInfo>();
		}

		public SavedWorldInfo(World world, TerrainGen terrain)
		{
			colorA = new VoxColor(terrain.colorA);
			colorB = new VoxColor(terrain.colorB);
			terrainSeed = terrain.randomSeed;

			chunkInfos = new List<SavedChunkInfo>();
			
			foreach (var chunk in world.Chunks)
			{
				var chunkInfo = new SavedChunkInfo(chunk.Value);

				if(chunkInfo.hasBlocks) chunkInfos.Add(chunkInfo);
			}
		}
	}

	[Serializable]
	public class SavedChunkInfo
	{
		public Vector3 unityPosition
		{
			get { return new Vector3(unityPosX, unityPosY, unityPosZ);}
			set
			{
				unityPosX = value.x;
				unityPosY = value.y;
				unityPosZ = value.z;
			}
		}

		public WorldPos worldPos;
		public bool initEmpty;
		public bool hasBlocks;

		private float unityPosX;
		private float unityPosY;
		private float unityPosZ;

		public Dictionary<WorldPos, Block> Blocks = new Dictionary<WorldPos, Block>();

		public SavedChunkInfo(Chunk chunk)
		{
			unityPosition = chunk.transform.position;
			worldPos = chunk.Pos;
			initEmpty = chunk.initEmpty;
			hasBlocks = false;

			 for (var x = 0; x < World.ChunkSize; x++)
			 {
				 for (var y = 0; y < World.ChunkSize; y++)
				 {
					 for (var z = 0; z < World.ChunkSize; z++)
					 {
						 if (!chunk.isDefault && chunk.Blocks[x, y, z].BlockType == VoxBlockType.Empty) continue;

						 if (!chunk.Blocks[x, y, z].Modified)
							 continue;
  
						 var pos = new WorldPos(x,y,z);
						 Blocks.Add(pos, chunk.Blocks[x, y, z]);
						 hasBlocks = true;
					 }
				 }
			 }
		 }
	}


	public class VoxSaveLoadChunk : MonoBehaviour
	{

		public void Update()
		{


			
		}


		public static void SaveChunk(Chunk chunk)
		{
			//PlayerPrefs.SetString();

			//var save = new VoxSave(chunk);
			//if (save.Blocks.Count == 0) return;

			//var path = Path.Combine(SaveLocation(chunk.World.WorldName), FileName(chunk.Pos));

			//IFormatter formatter = new BinaryFormatter();
			//var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
			//formatter.Serialize(stream, save);
			//stream.Close();
		}
	}
}