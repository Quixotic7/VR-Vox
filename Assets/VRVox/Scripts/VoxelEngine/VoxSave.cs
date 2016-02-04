// Core engine code taken from tutorials by AlexStv http://alexstv.com/ please review his license at http://alexstv.com/index.php/posts/unity-voxel-tutorial-licencing 

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VRVox
{
	[Serializable]
	public class VoxSave
	{
		public Dictionary<WorldPos, Block> Blocks = new Dictionary<WorldPos, Block>();

		public VoxSave(Chunk chunk)
		 {
			 for (var x = 0; x < World.ChunkSize; x++)
			 {
				 for (var y = 0; y < World.ChunkSize; y++)
				 {
					 for (var z = 0; z < World.ChunkSize; z++)
					 {
						 if (!chunk.Blocks[x, y, z].Modified)
							 continue;
  
						 var pos = new WorldPos(x,y,z);
						 Blocks.Add(pos, chunk.Blocks[x, y, z]);
					 }
				 }
			 }
		 }
	}
}