// Core engine code taken from tutorials by AlexStv http://alexstv.com/ please review his license at http://alexstv.com/index.php/posts/unity-voxel-tutorial-licencing 

using UnityEngine;
using System.Collections;


namespace VRVox
{
	public enum VoxBlockType
	{
		Default,
		Empty
	}

	[RequireComponent(typeof (MeshFilter))]
	[RequireComponent(typeof (MeshRenderer))]
	[RequireComponent(typeof (MeshCollider))]
	public class Chunk : MonoBehaviour
	{
		public bool initEmpty = false;
		public bool isDefault = false;
		public World World;
		public WorldPos Pos;

		public static float BlockScale = 0.5f;

		public bool RebuildNeeded
		{
			get { return _rebuildNeeded; }
			set
			{
				_rebuildNeeded = value;
				if (_rebuildNeeded && !doingRebuild) StartCoroutine(DoRebuild());
			}
		}

		private bool _rebuildNeeded;

		public bool Rendered;
		public bool UseMeshColliders = true;

		public Block[,,] Blocks = new Block[World.ChunkSize, World.ChunkSize, World.ChunkSize];
		protected MeshFilter Filter;
		protected MeshCollider Coll;

		// Use this for initialization
		protected void Awake()
		{
			Filter = gameObject.GetComponent<MeshFilter>();
			Coll = gameObject.GetComponent<MeshCollider>();

			//past here is just to set up an example chunk
			//Blocks = new VoxBlock[ChunkSize, ChunkSize, ChunkSize];

			for (var x = 0; x < World.ChunkSize; x++)
			{
				for (var y = 0; y < World.ChunkSize; y++)
				{
					for (var z = 0; z < World.ChunkSize; z++)
					{
						Blocks[x, y, z] = new Block(Color.white) { BlockType = VoxBlockType.Empty };
					}
				}
			}

			//Blocks[1, 14, 1] = new VoxBlock();

			//Blocks[3, 5, 2] = new VoxBlock();
			//Blocks[4, 5, 2] = new VoxBlock();


			//UpdateChunk();

		}

		// Update is called once per frame
		//private void Update()
		//{
		//	if (RebuildNeeded)
		//	{
		//		RebuildNeeded = false;
		//		Reconstruct();
		//	}

		//}

		protected bool doingRebuild;

		protected IEnumerator DoRebuild()
		{
			doingRebuild = true;
			yield return new WaitForEndOfFrame();

			if (_rebuildNeeded)
			{
				_rebuildNeeded = false;
				Reconstruct();
			}

			doingRebuild = false;
		}

		public Block GetBlock(int x, int y, int z)
		{
			//return Blocks[x, y, z];

			if (InRange(x) && InRange(y) && InRange(z))
				return Blocks[x, y, z];
			return World.GetBlock(Pos.x + x, Pos.y + y, Pos.z + z);
		}

		//public void SetBlock(int x, int y, int z, VoxBlockType blockType)
		//{
		//	SetBlock(x,y,z,blockType,Color.white);
		//}

		public void SetBlock(int x, int y, int z, VoxBlockType blockType, Color blockColor)
		{
			if (InRange(x) && InRange(y) && InRange(z))
			{
				if (Blocks[x, y, z] != null)
				{
					Blocks[x, y, z].JustCreated = Blocks[x, y, z].IsEmpty();

					Blocks[x, y, z].BlockType = blockType;
					Blocks[x, y, z].BlockColor = blockColor;
					Blocks[x, y, z].Modified = true;
				}
				else
				{
					//Blocks[x, y, z] = new Block(blockColor) {BlockType = blockType};
					Debug.LogError("You're doing it wrong, block in range should not be null");
				}
				//Blocks[x, y, z] = block;
			}
			else
			{
				World.SetBlock(Pos.x + x, Pos.y + y, Pos.z + z, blockType, blockColor);
			}

		}

		//public void SetBlock(int x, int y, int z, Block block)
		//{
		//	if (InRange(x) && InRange(y) && InRange(z))
		//	{
		//		if (Blocks[x, y, z] != null)
		//		{
		//			if (!Blocks[x, y, z].IsEmpty()) block.JustCreated = false;
		//		}
		//		Blocks[x, y, z] = block;
		//	}
		//	else
		//	{
		//		World.SetBlock(Pos.x + x, Pos.y + y, Pos.z + z, block);
		//	}
		//}

		public void SetBlocksUnmodified()
		{
			foreach (var block in Blocks)
			{
				block.Modified = false;
			}
		}

		public static bool InRange(int index)
		{
			if (index < 0 || index >= World.ChunkSize)
				return false;

			return true;
		}

		//Updates the chunk based on its contents
		protected void Reconstruct()
		{
			Rendered = true;
			var meshData = new MeshData();

			for (var x = 0; x < World.ChunkSize; x++)
			{
				for (var y = 0; y < World.ChunkSize; y++)
				{
					for (var z = 0; z < World.ChunkSize; z++)
					{
						Blocks[x, y, z].JustCreated = false;
						meshData = Blocks[x, y, z].Blockdata(this, x, y, z, BlockScale * World.UnitScale, World.UnitScale, meshData);
						//if (Blocks[x, y, z] != null)
						//{
							
						//}
						//else
						//{
						//	Blocks[x, y, z] = new Block(Color.white) {BlockType = VoxBlockType.Empty};
						//}
					}
				}
			}
			RenderMesh(meshData);
		}

		//Sends the calculated mesh information
		//to the mesh and collision components
		protected void RenderMesh(MeshData meshData)
		{
			Filter.mesh.Clear();
			Filter.mesh.vertices = meshData.vertices.ToArray();
			Filter.mesh.triangles = meshData.triangles.ToArray();
			Filter.mesh.colors = meshData.colors.ToArray();

			Filter.mesh.uv = meshData.uv.ToArray();
			Filter.mesh.RecalculateNormals();

			Coll.sharedMesh = null;
			var mesh = new Mesh
			{
				vertices = meshData.colVertices.ToArray(),
				triangles = meshData.colTriangles.ToArray()
			};
			mesh.RecalculateNormals();

			Coll.sharedMesh = mesh;
		}
	}
}
