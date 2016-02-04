// Core engine code taken from tutorials by AlexStv http://alexstv.com/ please review his license at http://alexstv.com/index.php/posts/unity-voxel-tutorial-licencing 

using UnityEngine;
using System.Collections;

namespace VRVox
{
	public static class VoxTerrain
	{
		public static WorldPos GetBlockPos(Vector3 pos)
		{
			return new WorldPos(
				Mathf.RoundToInt(pos.x * World.WorldToVoxScaler),
				Mathf.RoundToInt(pos.y * World.WorldToVoxScaler),
				Mathf.RoundToInt(pos.z * World.WorldToVoxScaler)
				);
		}

		public static WorldPos GetBlockPos(RaycastHit hit, bool adjacent = false)
		{
			var pos = new Vector3(
				MoveWithinBlock(hit.point.x, hit.normal.x, adjacent),
				MoveWithinBlock(hit.point.y, hit.normal.y, adjacent),
				MoveWithinBlock(hit.point.z, hit.normal.z, adjacent)
				);

			return GetBlockPos(pos);
		}

		public static Vector3 GetUnityPosition(WorldPos pos)
		{
			return new Vector3(pos.x * World.UnitScale, pos.y * World.UnitScale, pos.z * World.UnitScale);
		}

		static float MoveWithinBlock(float pos, float norm, bool adjacent = false)
		{
			//if (pos - (int)pos == 0.5f || pos - (int)pos == -0.5f)
			//{
				
			//}

			if (adjacent)
			{
				pos += (norm / 2 * World.UnitScale);
			}
			else
			{
				pos -= (norm / 2 * World.UnitScale);
			}

			return pos;
		}

		//public static bool SetBlock(RaycastHit hit, Block block, bool adjacent = false)
		//{
		//	var chunk = hit.collider.GetComponent<Chunk>();
		//	if (chunk == null)
		//		return false;

		//	var pos = GetBlockPos(hit, adjacent);

		//	chunk.World.SetBlock(pos, block, true);

		//	return true;
		//}

		//public static bool SetBlock(RaycastHit hit, WorldPos pos, Block block)
		//{
		//	var chunk = hit.collider.GetComponent<Chunk>();
		//	if (chunk == null)
		//		return false;

		//	chunk.World.SetBlock(pos, block, true);

		//	return true;
		//}


		public static bool SetBlock(RaycastHit hit, VoxBlockType block, Color blockColor, bool adjacent = false)
		{
			var chunk = hit.collider.GetComponent<Chunk>();
			if (chunk == null)
				return false;

			var pos = GetBlockPos(hit, adjacent);

			chunk.World.SetBlock(pos, block, blockColor, true);

			return true;
		}

		public static bool SetBlock(RaycastHit hit, WorldPos pos, VoxBlockType block, Color blockColor)
		{
			var chunk = hit.collider.GetComponent<Chunk>();
			if (chunk == null)
				return false;

			chunk.World.SetBlock(pos, block, blockColor, true);

			return true;
		}

		public static Block GetBlock(RaycastHit hit, bool adjacent = false)
		{
			var chunk = hit.collider.GetComponent<Chunk>();
			if (chunk == null)
				return null;

			var pos = GetBlockPos(hit, adjacent);

			var block = chunk.World.GetBlock(pos);

			return block;
		}
	}
}