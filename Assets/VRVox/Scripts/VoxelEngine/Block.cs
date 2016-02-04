// Core engine code taken from tutorials by AlexStv http://alexstv.com/ please review his license at http://alexstv.com/index.php/posts/unity-voxel-tutorial-licencing 

using System;
using UnityEngine;
using System.Collections;

namespace VRVox
{
	[Serializable]
	public class VoxColor
	{
		public float r;
		public float g;
		public float b;
		public float a;

		public VoxColor(Color color)
		{
			setColor(color);
		}

		public void setColor(Color color)
		{
			r = color.r;
			g = color.g;
			b = color.b;
			a = color.a;
		}

		public Color color
		{
			get { return new Color(r, g, b, a); }
		}
	}

	[Serializable]
	public class Block
	{
		public bool JustCreated = true;

		public bool Modified = true;

		public VoxBlockType BlockType = VoxBlockType.Default;

		public enum Direction
		{
			North,
			East,
			South,
			West,
			Up,
			Down
		};

		private const float TileSize = 0.25f;

		public Color BlockColor
		{
			get { return VoxBlockColor.color; }
			set { VoxBlockColor.setColor(value); }
		}

		public VoxColor VoxBlockColor;

		public Block(Color color)
		{
			VoxBlockColor = new VoxColor(color);
		}

		public virtual MeshData Blockdata(Chunk chunk, int x, int y, int z, float scale, float worldScale, MeshData meshData)
		{
			if (BlockType == VoxBlockType.Empty) return meshData;

			meshData.UseRenderDataForCol = true;

			if (!chunk.GetBlock(x, y + 1, z).IsSolid(Direction.Down))
			{
				meshData = FaceDataUp(chunk, x, y, z, scale, worldScale, meshData);
			}
			if (!chunk.GetBlock(x, y - 1, z).IsSolid(Direction.Up))
			{
				meshData = FaceDataDown(chunk, x, y, z, scale, worldScale, meshData);
			}

			if (!chunk.GetBlock(x, y, z + 1).IsSolid(Direction.South))
			{
				meshData = FaceDataNorth(chunk, x, y, z, scale, worldScale, meshData);
			}

			if (!chunk.GetBlock(x, y, z - 1).IsSolid(Direction.North))
			{
				meshData = FaceDataSouth(chunk, x, y, z, scale, worldScale, meshData);
			}

			if (!chunk.GetBlock(x + 1, y, z).IsSolid(Direction.West))
			{
				meshData = FaceDataEast(chunk, x, y, z, scale, worldScale, meshData);
			}

			if (!chunk.GetBlock(x - 1, y, z).IsSolid(Direction.East))
			{
				meshData = FaceDataWest(chunk, x, y, z, scale, worldScale, meshData);
			}

			return meshData;
		}

		protected virtual MeshData FaceDataUp(Chunk chunk, int x, int y, int z, float scale, float worldScale, MeshData meshData)
		{
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale + scale, z * worldScale + scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale + scale, z * worldScale + scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale + scale, z * worldScale - scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale + scale, z * worldScale - scale), BlockColor);

			meshData.AddQuadTriangles();
			meshData.uv.AddRange(FaceUVs(Direction.Up));
			return meshData;
		}

		protected virtual MeshData FaceDataDown(Chunk chunk, int x, int y, int z, float scale, float worldScale, MeshData meshData)
		{
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale - scale, z * worldScale - scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale - scale, z * worldScale - scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale - scale, z * worldScale + scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale - scale, z * worldScale + scale), BlockColor);

			meshData.AddQuadTriangles();
			meshData.uv.AddRange(FaceUVs(Direction.Down));

			return meshData;
		}

		protected virtual MeshData FaceDataNorth(Chunk chunk, int x, int y, int z, float scale, float worldScale, MeshData meshData)
		{
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale - scale, z * worldScale + scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale + scale, z * worldScale + scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale + scale, z * worldScale + scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale - scale, z * worldScale + scale), BlockColor);

			meshData.AddQuadTriangles();
			meshData.uv.AddRange(FaceUVs(Direction.North));

			return meshData;
		}

		protected virtual MeshData FaceDataEast(Chunk chunk, int x, int y, int z, float scale, float worldScale, MeshData meshData)
		{
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale - scale, z * worldScale - scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale + scale, z * worldScale - scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale + scale, z * worldScale + scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale - scale, z * worldScale + scale), BlockColor);

			meshData.AddQuadTriangles();
			meshData.uv.AddRange(FaceUVs(Direction.East));

			return meshData;
		}

		protected virtual MeshData FaceDataSouth(Chunk chunk, int x, int y, int z, float scale, float worldScale, MeshData meshData)
		{
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale - scale, z * worldScale - scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale + scale, z * worldScale - scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale + scale, z * worldScale - scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale + scale, y * worldScale - scale, z * worldScale - scale), BlockColor);

			meshData.AddQuadTriangles();
			meshData.uv.AddRange(FaceUVs(Direction.South));

			return meshData;
		}

		protected virtual MeshData FaceDataWest(Chunk chunk, int x, int y, int z, float scale, float worldScale, MeshData meshData)
		{
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale - scale, z * worldScale + scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale + scale, z * worldScale + scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale + scale, z * worldScale - scale), BlockColor);
			meshData.AddVertex(new Vector3(x * worldScale - scale, y * worldScale - scale, z * worldScale - scale), BlockColor);

			meshData.AddQuadTriangles();
			meshData.uv.AddRange(FaceUVs(Direction.West));

			return meshData;
		}

		public struct Tile
		{
			public int x;
			public int y;
		}

		public virtual Tile GetTexturePosition(Direction direction)
		{
			var tile = new Tile
			{
				x = 0,
				y = 0
			};

			return tile;
		}

		public virtual Vector2[] FaceUVs(Direction direction)
		{
			var uvs = new Vector2[4];
			var tilePos = GetTexturePosition(direction);

			//uvs[0] = new Vector2(0,0);
			//uvs[1] = new Vector2(0.5f,0);
			//uvs[2] = new Vector2(0.5f,1);
			//uvs[3] = new Vector2(0,0.5f);

			//uvs[0] = new Vector2(TileSize * tilePos.x,
			//	TileSize * tilePos.y);
			//uvs[1] = new Vector2(TileSize * tilePos.x + TileSize,
			//	TileSize * tilePos.y);
			//uvs[2] = new Vector2(TileSize * tilePos.x + TileSize,
			//	TileSize * tilePos.y + TileSize);
			//uvs[3] = new Vector2(TileSize * tilePos.x,
			//	TileSize * tilePos.y + TileSize);


			uvs[0] = new Vector2(TileSize * tilePos.x + TileSize,
				TileSize * tilePos.y);
			uvs[1] = new Vector2(TileSize * tilePos.x + TileSize,
				TileSize * tilePos.y + TileSize);
			uvs[2] = new Vector2(TileSize * tilePos.x,
				TileSize * tilePos.y + TileSize);
			uvs[3] = new Vector2(TileSize * tilePos.x,
				TileSize * tilePos.y);

			return uvs;
		}

		public virtual bool IsSolid(Direction direction)
		{
			if (BlockType == VoxBlockType.Empty) return false; 

			switch (direction)
			{
				case Direction.North:
					return true;
				case Direction.East:
					return true;
				case Direction.South:
					return true;
				case Direction.West:
					return true;
				case Direction.Up:
					return true;
				case Direction.Down:
					return true;
			}

			return false;
		}

		public virtual bool IsEmpty()
		{
			return BlockType == VoxBlockType.Empty;
		}
	}
}
