// Core engine code taken from tutorials by AlexStv http://alexstv.com/ please review his license at http://alexstv.com/index.php/posts/unity-voxel-tutorial-licencing 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VRVox
{
	public class LoadChunks : MonoBehaviour
	{

		public World world;

		protected List<WorldPos> updateList = new List<WorldPos>();
		protected List<WorldPos> buildList = new List<WorldPos>();

		protected int Timer = 0;

		public float MaxChunkDistance = 64;

		static WorldPos[] chunkPositions = {   new WorldPos( 0, 0,  0), new WorldPos(-1, 0,  0), new WorldPos( 0, 0, -1), new WorldPos( 0, 0,  1), new WorldPos( 1, 0,  0),
							 new WorldPos(-1, 0, -1), new WorldPos(-1, 0,  1), new WorldPos( 1, 0, -1), new WorldPos( 1, 0,  1), new WorldPos(-2, 0,  0),
							 new WorldPos( 0, 0, -2), new WorldPos( 0, 0,  2), new WorldPos( 2, 0,  0), new WorldPos(-2, 0, -1), new WorldPos(-2, 0,  1),
							 new WorldPos(-1, 0, -2), new WorldPos(-1, 0,  2), new WorldPos( 1, 0, -2), new WorldPos( 1, 0,  2), new WorldPos( 2, 0, -1),
							 new WorldPos( 2, 0,  1), new WorldPos(-2, 0, -2), new WorldPos(-2, 0,  2), new WorldPos( 2, 0, -2), new WorldPos( 2, 0,  2),
							 new WorldPos(-3, 0,  0), new WorldPos( 0, 0, -3), new WorldPos( 0, 0,  3), new WorldPos( 3, 0,  0), new WorldPos(-3, 0, -1),
							 new WorldPos(-3, 0,  1), new WorldPos(-1, 0, -3), new WorldPos(-1, 0,  3), new WorldPos( 1, 0, -3), new WorldPos( 1, 0,  3),
							 new WorldPos( 3, 0, -1), new WorldPos( 3, 0,  1), new WorldPos(-3, 0, -2), new WorldPos(-3, 0,  2), new WorldPos(-2, 0, -3),
							 new WorldPos(-2, 0,  3), new WorldPos( 2, 0, -3), new WorldPos( 2, 0,  3), new WorldPos( 3, 0, -2), new WorldPos( 3, 0,  2),
							 new WorldPos(-4, 0,  0), new WorldPos( 0, 0, -4), new WorldPos( 0, 0,  4), new WorldPos( 4, 0,  0), new WorldPos(-4, 0, -1),
							 new WorldPos(-4, 0,  1), new WorldPos(-1, 0, -4), new WorldPos(-1, 0,  4), new WorldPos( 1, 0, -4), new WorldPos( 1, 0,  4),
							 new WorldPos( 4, 0, -1), new WorldPos( 4, 0,  1), new WorldPos(-3, 0, -3), new WorldPos(-3, 0,  3), new WorldPos( 3, 0, -3),
							 new WorldPos( 3, 0,  3), new WorldPos(-4, 0, -2), new WorldPos(-4, 0,  2), new WorldPos(-2, 0, -4), new WorldPos(-2, 0,  4),
							 new WorldPos( 2, 0, -4), new WorldPos( 2, 0,  4), new WorldPos( 4, 0, -2), new WorldPos( 4, 0,  2), new WorldPos(-5, 0,  0),
							 new WorldPos(-4, 0, -3), new WorldPos(-4, 0,  3), new WorldPos(-3, 0, -4), new WorldPos(-3, 0,  4), new WorldPos( 0, 0, -5),
							 new WorldPos( 0, 0,  5), new WorldPos( 3, 0, -4), new WorldPos( 3, 0,  4), new WorldPos( 4, 0, -3), new WorldPos( 4, 0,  3),
							 new WorldPos( 5, 0,  0), new WorldPos(-5, 0, -1), new WorldPos(-5, 0,  1), new WorldPos(-1, 0, -5), new WorldPos(-1, 0,  5),
							 new WorldPos( 1, 0, -5), new WorldPos( 1, 0,  5), new WorldPos( 5, 0, -1), new WorldPos( 5, 0,  1), new WorldPos(-5, 0, -2),
							 new WorldPos(-5, 0,  2), new WorldPos(-2, 0, -5), new WorldPos(-2, 0,  5), new WorldPos( 2, 0, -5), new WorldPos( 2, 0,  5),
							 new WorldPos( 5, 0, -2), new WorldPos( 5, 0,  2), new WorldPos(-4, 0, -4), new WorldPos(-4, 0,  4), new WorldPos( 4, 0, -4),
							 new WorldPos( 4, 0,  4), new WorldPos(-5, 0, -3), new WorldPos(-5, 0,  3), new WorldPos(-3, 0, -5), new WorldPos(-3, 0,  5),
							 new WorldPos( 3, 0, -5), new WorldPos( 3, 0,  5), new WorldPos( 5, 0, -3), new WorldPos( 5, 0,  3), new WorldPos(-6, 0,  0),
							 new WorldPos( 0, 0, -6), new WorldPos( 0, 0,  6), new WorldPos( 6, 0,  0), new WorldPos(-6, 0, -1), new WorldPos(-6, 0,  1),
							 new WorldPos(-1, 0, -6), new WorldPos(-1, 0,  6), new WorldPos( 1, 0, -6), new WorldPos( 1, 0,  6), new WorldPos( 6, 0, -1),
							 new WorldPos( 6, 0,  1), new WorldPos(-6, 0, -2), new WorldPos(-6, 0,  2), new WorldPos(-2, 0, -6), new WorldPos(-2, 0,  6),
							 new WorldPos( 2, 0, -6), new WorldPos( 2, 0,  6), new WorldPos( 6, 0, -2), new WorldPos( 6, 0,  2), new WorldPos(-5, 0, -4),
							 new WorldPos(-5, 0,  4), new WorldPos(-4, 0, -5), new WorldPos(-4, 0,  5), new WorldPos( 4, 0, -5), new WorldPos( 4, 0,  5),
							 new WorldPos( 5, 0, -4), new WorldPos( 5, 0,  4), new WorldPos(-6, 0, -3), new WorldPos(-6, 0,  3), new WorldPos(-3, 0, -6),
							 new WorldPos(-3, 0,  6), new WorldPos( 3, 0, -6), new WorldPos( 3, 0,  6), new WorldPos( 6, 0, -3), new WorldPos( 6, 0,  3),
							 new WorldPos(-7, 0,  0), new WorldPos( 0, 0, -7), new WorldPos( 0, 0,  7), new WorldPos( 7, 0,  0), new WorldPos(-7, 0, -1),
							 new WorldPos(-7, 0,  1), new WorldPos(-5, 0, -5), new WorldPos(-5, 0,  5), new WorldPos(-1, 0, -7), new WorldPos(-1, 0,  7),
							 new WorldPos( 1, 0, -7), new WorldPos( 1, 0,  7), new WorldPos( 5, 0, -5), new WorldPos( 5, 0,  5), new WorldPos( 7, 0, -1),
							 new WorldPos( 7, 0,  1), new WorldPos(-6, 0, -4), new WorldPos(-6, 0,  4), new WorldPos(-4, 0, -6), new WorldPos(-4, 0,  6),
							 new WorldPos( 4, 0, -6), new WorldPos( 4, 0,  6), new WorldPos( 6, 0, -4), new WorldPos( 6, 0,  4), new WorldPos(-7, 0, -2),
							 new WorldPos(-7, 0,  2), new WorldPos(-2, 0, -7), new WorldPos(-2, 0,  7), new WorldPos( 2, 0, -7), new WorldPos( 2, 0,  7),
							 new WorldPos( 7, 0, -2), new WorldPos( 7, 0,  2), new WorldPos(-7, 0, -3), new WorldPos(-7, 0,  3), new WorldPos(-3, 0, -7),
							 new WorldPos(-3, 0,  7), new WorldPos( 3, 0, -7), new WorldPos( 3, 0,  7), new WorldPos( 7, 0, -3), new WorldPos( 7, 0,  3),
							 new WorldPos(-6, 0, -5), new WorldPos(-6, 0,  5), new WorldPos(-5, 0, -6), new WorldPos(-5, 0,  6), new WorldPos( 5, 0, -6),
							 new WorldPos( 5, 0,  6), new WorldPos( 6, 0, -5), new WorldPos( 6, 0,  5) };

		// Update is called once per frame
		void Update()
		{
			if (DeleteChunks()) //Check to see if a delete happened
				return;                 //and if so return early

			FindChunksToLoad();
			LoadAndRenderChunks();
		}

		protected WorldPos LastPlayerPos;
		protected bool positionsChecked;

		protected int positionIndex;

		void FindChunksToLoad()
		{
			var player = VoxManager.ViewPosition;
			//Get the position of this gameobject to generate around
			WorldPos playerPos = new WorldPos(
				Mathf.FloorToInt(player.x * World.WorldToVoxScaler / World.ChunkSize) * World.ChunkSize,
				Mathf.FloorToInt(player.y * World.WorldToVoxScaler / World.ChunkSize) * World.ChunkSize,
				Mathf.FloorToInt(player.z * World.WorldToVoxScaler / World.ChunkSize) * World.ChunkSize
				);

			//If there aren't already chunks to generate
			if (updateList.Count == 0)
			{
				//Cycle through the array of positions
				for (int i = positionIndex; i < chunkPositions.Length; i++)
				{
					positionIndex = i;
					//translate the player position and array position into chunk position
					WorldPos newChunkPos = new WorldPos(
						chunkPositions[i].x * World.ChunkSize + playerPos.x,
						0,
						chunkPositions[i].z * World.ChunkSize + playerPos.z
						);

					var distance = Vector3.Distance(
						VoxTerrain.GetUnityPosition(newChunkPos),
						new Vector3(player.x, 0, player.z));

					if (distance >= MaxChunkDistance) continue;

					//Get the chunk in the defined position
					Chunk newChunk = world.GetChunk(
						newChunkPos.x, newChunkPos.y, newChunkPos.z);

					//If the chunk already exists and it's already
					//rendered or in queue to be rendered continue
					if (newChunk != null
						&& (newChunk.Rendered || updateList.Contains(newChunkPos)))
						continue;

					//load a column of chunks in this position
					for (int y = -world.worldYNegative; y < world.worldY; y++)
					{
						for (int x = newChunkPos.x - World.ChunkSize; x <= newChunkPos.x + World.ChunkSize; x += World.ChunkSize)
						{
							for (int z = newChunkPos.z - World.ChunkSize; z <= newChunkPos.z + World.ChunkSize; z += World.ChunkSize)
							{
								buildList.Add(new WorldPos(
									x, y * World.ChunkSize, z));
							}
						}
						updateList.Add(new WorldPos(
							newChunkPos.x, y * World.ChunkSize, newChunkPos.z));
					}

					//if (updateList.Count > 4) return;
					break;
				}

				positionsChecked = true;
			}

			if (!playerPos.Equals(LastPlayerPos)) positionIndex = 0;
			LastPlayerPos = playerPos;
		}

		void BuildChunk(WorldPos pos)
		{
			if (world.GetChunk(pos.x, pos.y, pos.z) == null)
				world.CreateTerrainChunk(pos.x, pos.y, pos.z);
		}

		void LoadAndRenderChunks()
		{
			if (buildList.Count != 0)
			{
				for (int i = 0; i < buildList.Count && i < 1; i++)
				{
					BuildChunk(buildList[0]);
					buildList.RemoveAt(0);
				}

				//If chunks were built return early
				return;
			}

			if (updateList.Count != 0)
			{
				Chunk chunk = world.GetChunk(updateList[0].x, updateList[0].y, updateList[0].z);
				if (chunk != null)
					chunk.RebuildNeeded = true;
				updateList.RemoveAt(0);
			}
		}

		protected float deleteIndex;

		protected bool DeleteChunks()
		{
			var player = VoxManager.ViewPosition;

			if (Timer == 20)
			 {
				 var chunksToDelete = new List<WorldPos>();

				 foreach (var chunk in world.Chunks)
				 {
					 float distance = Vector3.Distance(
						 VoxTerrain.GetUnityPosition(chunk.Value.Pos),
							new Vector3(player.x, 0, player.z));

					 if (distance > MaxChunkDistance)
					 {
						 chunksToDelete.Add(chunk.Key);
						 break;
					 }
				 }
 
				 foreach (var chunk in chunksToDelete)
					 world.DestroyChunk(chunk.x, chunk.y, chunk.z);

				 Timer = 0;
				 return true;    //Add this line
			 }

			Timer++;
			 return false;    //Add this line
		}
	}
}