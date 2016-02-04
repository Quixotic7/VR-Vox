// Core engine code taken from tutorials by AlexStv http://alexstv.com/ please review his license at http://alexstv.com/index.php/posts/unity-voxel-tutorial-licencing 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VRVox
{
	public class World : MonoBehaviour
	{
		public float VisualScale = 0.5f;

		public static int ChunkSize = 8;

		public static float UnitScale = 0.5f;

		public static float HalfUnitScale = 0.25f;

		public static float WorldToVoxScaler = 1f;

		public string WorldName = "MyVoxWorld";

		public GameObject ChunkPrefab;

		public Dictionary<WorldPos, Chunk> Chunks = new Dictionary<WorldPos, Chunk>();

		public Color colorA = Color.blue;
		public Color colorB = Color.white;

		public int NewChunkX;
		public int NewChunkY;
		public int NewChunkZ;

		public bool GenChunk;

		//public VoxMenuButton SaveButton;
		public VoxMenuButton NewButton;
		//public VoxMenuButton LoadButton;

		public VoxMenuButton[] LoadButtons;
		public VoxMenuButton[] SaveButtons;

		public TerrainGen terrainGenerator;

		public VoxVignette vignette;

		public int worldX = 8;
		public int worldY = 4;
		public int worldYNegative = 2;
		public int worldZ = 8;

		public bool genWorld = false;

		public bool InitialBuildFinished { get; private set; }

		protected void Awake()
		{
			UnitScale = VisualScale;

			HalfUnitScale = UnitScale*0.5f;

			WorldToVoxScaler = 1/Mathf.Max(UnitScale, float.Epsilon);


			//SaveButton.SubscribeToButton(OnSaveButton);
			//LoadButton.SubscribeToButton(OnLoadButton);

			foreach (var savebutton in SaveButtons)
			{
				savebutton.SubscribeToButton(OnSaveButton);
			}

			foreach (var loadbutton in LoadButtons)
			{
				loadbutton.SubscribeToButton(OnLoadButton);
			}

			NewButton.SubscribeToButton(OnNewButton);
		}

		protected void OnSaveButton(VoxMenuButton button, bool state)
		{
			Debug.Log("Saving World");
			WorldName = "VoxWorld" + button.toolTip;

			SaveAll();
		}

		protected void OnLoadButton(VoxMenuButton button, bool state)
		{
			Debug.Log("Loading World");

			WorldName = "VoxWorld" + button.toolTip;

			LoadAll();
		}

		protected void OnNewButton(VoxMenuButton button, bool state)
		{
			WorldName = "VoxWorld";

			NewWorld();
		}

		protected Block EmptyBlock = new Block(Color.white) {BlockType = VoxBlockType.Empty};

		//// Use this for initialization
		private void Start()
		{
			//Cursor.visible = false;

			//Cursor.lockState = CursorLockMode.Locked;

			//CreateChunk(16,16,16);

			//CreateDefaultChunks();

			//CreateProceduralTerrain();


			NewWorld();
		}

		private void CreateProceduralTerrain()
		{
			//DeleteAllChunks();

			terrainGenerator.colorA = GetRandomColor();
			terrainGenerator.colorB = GetRandomColor(Color.white, terrainGenerator.colorA);
			terrainGenerator.randomSeed = (int)(Random.value*1000);


			StartCoroutine(DoProceduralTerrain());

		}

		protected bool buildingTerrain;

		IEnumerator DoProceduralTerrain()
		{
			buildingTerrain = true;

			//VoxManager.ShowMessage("World Loading...");
			//for (var x = -worldX; x < worldX; x++)
			//{
			//	for (var y = -worldY; y < worldY; y++)
			//	{
			//		for (var z = -worldZ; z < worldZ; z++)
			//		{
			//			var chunk = CreateTerrainChunk(x * ChunkSize, y * ChunkSize, z * ChunkSize);
			//			chunk.isDefault = true;
			//			yield return new WaitForEndOfFrame();
			//		}
			//	}
			//}

			//for (var x = 0; x < worldX; x++)
			//{
			//	for (var y = -worldY; y < worldY; y++)
			//	{
			//		for (var z = 0; z < worldZ; z++)
			//		{
			//			var chunk = CreateTerrainChunk(x * ChunkSize, y * ChunkSize, z * ChunkSize);
			//			if (chunk != null) chunk.isDefault = true;

			//			chunk = CreateTerrainChunk(-x * ChunkSize, y * ChunkSize, -z * ChunkSize);
			//			if (chunk != null) chunk.isDefault = true;

			//			yield return new WaitForEndOfFrame();
			//		}
			//	}
			//}

			const int chunksPerFrame = 16;
			const float waitTime = 0.15f;


			var chunkCount = 0;

			int z, dx;
			var x = z = dx = 0;
			var dz = -1;
			var t = Mathf.Max(worldX, worldZ);
			var maxI = t * t;
			for (var i = 0; i < maxI; i++)
			{
				if ((-worldX / 2 <= x) && (x <= worldX / 2) && (-worldZ / 2 <= z) && (z <= worldZ / 2))
				{
					// DO STUFF...

					for (var y = -worldYNegative; y < worldY; y++)
					{
						var xChunk = x * ChunkSize;
						var yChunk = y * ChunkSize;
						var zChunk = z * ChunkSize;

						var chunk = CreateTerrainChunk(xChunk, yChunk, zChunk);
						if (chunk != null) chunk.isDefault = true;
						var s = ChunkSize;

						//yChunk -= s;

						//for (var j = 0; j < 3; j++)
						//{
						//	var neighbors = new[]
						//	{
						//		new WorldPos(xChunk + s, yChunk, zChunk),
						//		new WorldPos(xChunk + s, yChunk, zChunk - s),
						//		new WorldPos(xChunk, yChunk, zChunk - s),
						//		new WorldPos(xChunk - s, yChunk, zChunk - s),
						//		new WorldPos(xChunk - s, yChunk, zChunk),
						//		new WorldPos(xChunk - s, yChunk, zChunk + s),
						//		new WorldPos(xChunk, yChunk, zChunk + s),
						//		new WorldPos(xChunk + s, yChunk, zChunk + s)
						//	};

						//	foreach (var pos in neighbors)
						//	{
						//		if (Chunks.ContainsKey(pos))
						//		{
						//			Chunks[pos].RebuildNeeded = true;
						//		}
						//	}

						//	yChunk += s;
						//}
						chunkCount++;
						if (chunkCount > chunksPerFrame)
						{
							//VoxManager.ShowMessage("World Loading...");
							yield return new WaitForSeconds(waitTime);
							chunkCount = 0;
						}


						//yield return new WaitForEndOfFrame();
					}
				}
				if ((x == z) || ((x < 0) && (x == -z)) || ((x > 0) && (x == 1 - z)))
				{
					t = dx;
					dx = -dz;
					dz = t;
				}
				x += dx;
				z += dz;

			}

			buildingTerrain = false;


			foreach (var chunk in Chunks)
			{
				chunk.Value.RebuildNeeded = true;

				//yield return new WaitForSeconds(0.05f);
			}

			yield return null;
		}

		private void CreateDefaultChunks()
		{
			for (var x = -4; x < 4; x++)
			{
				for (var y = -1; y < 1; y++)
				{
					for (var z = -4; z < 4; z++)
					{
						var chunk = CreateChunk(x * ChunkSize, y * ChunkSize, z * ChunkSize);
						chunk.isDefault = true;
					}
				}
			}
		}

		// Update is called once per frame
		private void Update()
		{
			if (GenChunk)
			{
				GenChunk = false;
				var chunkPos = new WorldPos(NewChunkX, NewChunkY, NewChunkZ);
				Chunk chunk = null;

				if (Chunks.TryGetValue(chunkPos, out chunk))
				{
					DestroyChunk(chunkPos.x, chunkPos.y, chunkPos.z);
				}
				else
				{
					CreateChunk(chunkPos.x, chunkPos.y, chunkPos.z);
				}
			}

			if (genWorld)
			{
				genWorld = false;
				CreateProceduralTerrain();
			}

			//if(Input.GetKeyDown(KeyCode.J)) SaveAll();

			//if (Input.GetKeyDown(KeyCode.L)) LoadAll();

			//if(Input.GetKeyDown(KeyCode.G)) NewWorld();

			//if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
		}

		public void SaveAll()
		{
			var worldSaveInfo = new SavedWorldInfo(this, terrainGenerator);

			Serialization.SaveWorldInfo(this, worldSaveInfo);

			VoxManager.ShowMessage("World Saved");

			//DeleteAllChunks();
		}

		public void NewWorld()
		{
			InitialBuildFinished = false;

			vignette.ToggleVignette(true);

			VoxManager.ResetPlayer();
			DeleteAllChunks();
			//Serialization.DeleteSaves(this);
			//DeleteAllChunks();
			//CreateDefaultChunks();
			CreateProceduralTerrain();
			StartCoroutine(ProceduralLoad(new SavedWorldInfo(this, terrainGenerator)));
		}

		public void DeleteAllChunks()
		{
			StopAllCoroutines();
			foreach (var chunk in Chunks)
			{
				//Serialization.SaveChunk(chunk.Value);
				Destroy(chunk.Value.gameObject);
			}

			Chunks.Clear();
		}

		IEnumerator ProceduralLoad(SavedWorldInfo worldInfo)
		{
			while (buildingTerrain)
			{
				yield return new WaitForEndOfFrame();
			}

			foreach (var chunkInfo in worldInfo.chunkInfos)
			{
				var chunk = Chunks.ContainsKey(chunkInfo.worldPos) ?
					Chunks[chunkInfo.worldPos]
					: CreateChunk(chunkInfo.worldPos.x, chunkInfo.worldPos.y, chunkInfo.worldPos.z, chunkInfo.initEmpty);

				foreach (var block in chunkInfo.Blocks)
				{
					block.Value.JustCreated = false;
					chunk.Blocks[block.Key.x, block.Key.y, block.Key.z] = block.Value;
				}

				chunk.RebuildNeeded = true;
			}

			vignette.ToggleVignette(false);

			VoxManager.ShowMessage("");

			//var allEmpty = false;

			//while (!allEmpty)
			//{
			//	allEmpty = true;

			//	var playerPos = VoxTerrain.GetBlockPos(VoxManager.Player.position);

			//	for (var px = -3; px < 3; px++)
			//	{
			//		for (var py = -3; py < 5; py++)
			//		{
			//			for (var pz = -3; pz < 3; pz++)
			//			{
			//				var p = playerPos + new WorldPos(px, py, pz);

			//				var b = GetBlock(p);

			//				if (!b.IsEmpty())
			//				{
			//					allEmpty = false;
			//					break;
			//				}
			//			}
			//		}
			//	}

			//	VoxManager.Player.position = VoxManager.Player.position + Vector3.up*2;
			//}

			//Debug.Log("Initial Building Finished");
			InitialBuildFinished = true;

		}

		public void LoadAll()
		{
			var worldInfo = Serialization.LoadWorldInfo(this);

			if (worldInfo == null)
			{
				Debug.LogError("Can't Load, World is null");
				//CreateProceduralTerrain();
				return;
			}
			DeleteAllChunks();

			//VoxManager.ResetPlayer();

			vignette.ToggleVignette(true);

			terrainGenerator.colorA = worldInfo.colorA.color;
			terrainGenerator.colorB = worldInfo.colorB.color;
			terrainGenerator.randomSeed = worldInfo.terrainSeed;

			StartCoroutine(DoProceduralTerrain());
			StartCoroutine(ProceduralLoad(worldInfo));
		}

		void UpdateIfEqual(int value1, int value2, WorldPos pos)
		{
			if (value1 == value2)
			{
				var chunk = GetChunk(pos.x, pos.y, pos.z);
				if (chunk != null)
					chunk.RebuildNeeded = true;
			}
		}

		public Chunk CreateChunk(int x, int y, int z, bool empty = false)
		{
			var worldPos = new WorldPos(x, y, z);

			//Instantiate the chunk at the coordinates using the chunk prefab
			var newChunkObject = Instantiate(ChunkPrefab, new Vector3(x * UnitScale, y * UnitScale, z * UnitScale), Quaternion.identity) as GameObject;

			var newChunk = newChunkObject.GetComponent<Chunk>();

			newChunk.initEmpty = empty;
			newChunk.Pos = worldPos;
			newChunk.World = this;
			newChunk.RebuildNeeded = true;

			//Add it to the chunks dictionary with the position as the key
			Chunks.Add(worldPos, newChunk);

			var counter = 0;

			//for (var xi = 0; xi < ChunkSize; xi++)
			//{
			//	for (var yi = 0; yi < ChunkSize; yi++)
			//	{
			//		for (var zi = 0; zi < ChunkSize; zi++)
			//		{
			//			if (!empty)
			//			{
			//				SetBlock(x + xi, y + yi, z + zi, new Block(GetRandomColor(colorA, colorB)));
			//			}
			//			else
			//			{
			//				SetBlock(x + xi, y + yi, z + zi, new BlockEmpty());
			//			}
			//		}
			//	}
			//}

			newChunk.SetBlocksUnmodified();

			return newChunk;
		}

		public Chunk CreateTerrainChunk(int x, int y, int z)
		{
			var worldPos = new WorldPos(x, y, z);

			if (Chunks.ContainsKey(worldPos)) return null;

			//Instantiate the chunk at the coordinates using the chunk prefab
			var newChunkObject = Instantiate(ChunkPrefab, new Vector3(x * UnitScale, y * UnitScale, z * UnitScale), Quaternion.identity) as GameObject;

            newChunkObject.hideFlags = HideFlags.HideInHierarchy; // Keeps the hierachy clean. 

			var newChunk = newChunkObject.GetComponent<Chunk>();

			newChunk.initEmpty = false;
			newChunk.Pos = worldPos;
			newChunk.World = this;
			newChunk.RebuildNeeded = true;

			//Add it to the chunks dictionary with the position as the key
			Chunks.Add(worldPos, newChunk);

			newChunk = terrainGenerator.ChunkGen(newChunk);

			newChunk.SetBlocksUnmodified();

			//bool loaded = Serialization.Load(newChunk);

			return newChunk;
		}



		public void CreateChunk2(int x, int y, int z)
		{
			WorldPos worldPos = new WorldPos(x, y, z);

			//Instantiate the chunk at the coordinates using the chunk prefab
			GameObject newChunkObject = Instantiate(
							ChunkPrefab, new Vector3(x, y, z),
							Quaternion.Euler(Vector3.zero)
						) as GameObject;

			var newChunk = newChunkObject.GetComponent<Chunk>();

			newChunk.Pos = worldPos;
			newChunk.World = this;

			//Add it to the chunks dictionary with the position as the key
			Chunks.Add(worldPos, newChunk);

			//Add these lines:
			//var terrainGen = new TerrainGen();
			newChunk = terrainGenerator.ChunkGen(newChunk);

			newChunk.SetBlocksUnmodified();

			//bool loaded = Serialization.Load(newChunk);

		}

		public static Color GetRandomColor()
		{
			return new Color(Random.value, Random.value, Random.value);
		}

		public static Color GetRandomColor(Color colorA, Color colorB)
		{
			return Color.Lerp(colorA, colorB, Random.value);
		}

		public void DestroyChunk(int x, int y, int z)
		{
			Chunk chunk = null;
			if (Chunks.TryGetValue(new WorldPos(x, y, z), out chunk))
			{
				Serialization.SaveChunk(chunk);
				Destroy(chunk.gameObject);
				Chunks.Remove(new WorldPos(x, y, z));
			}
		}

		public Chunk GetChunk(int x, int y, int z, bool createChunkIfNull = false)
		{
			var pos = new WorldPos();
			float multiple = ChunkSize;
			pos.x = Mathf.FloorToInt(x/multiple)*ChunkSize;
			pos.y = Mathf.FloorToInt(y/multiple)*ChunkSize;
			pos.z = Mathf.FloorToInt(z/multiple)*ChunkSize;

			Chunk containerChunk = null;

			Chunks.TryGetValue(pos, out containerChunk);

			if (createChunkIfNull && containerChunk == null)
			{
				containerChunk = CreateChunk(pos.x, pos.y, pos.z, true);
			}

			return containerChunk;
		}

		public Block GetBlock(WorldPos pos)
		{
			return GetBlock(pos.x, pos.y, pos.z);
		}

		public Block GetBlock(int x, int y, int z)
		{
			var containerChunk = GetChunk(x, y, z);

			if (containerChunk != null)
			{
				var block = containerChunk.GetBlock(
					x - containerChunk.Pos.x,
					y - containerChunk.Pos.y,
					z - containerChunk.Pos.z);

				return block;
			}
			else
			{
				return EmptyBlock;
			}

		}

		public void SetBlock(WorldPos pos, VoxBlockType blockType, Color blockColor, bool createChunkIfNull = false)
		{
			SetBlock(pos.x, pos.y, pos.z, blockType, blockColor, createChunkIfNull);
		}

		public void SetBlock(int x, int y, int z, VoxBlockType blockType, Color blockColor, bool createChunkIfNull = false)
		{
			var chunk = GetChunk(x, y, z, createChunkIfNull);

			if (chunk != null)
			{
				chunk.SetBlock(x - chunk.Pos.x, y - chunk.Pos.y, z - chunk.Pos.z, blockType, blockColor);
				chunk.RebuildNeeded = true;

				UpdateIfEqual(x - chunk.Pos.x, 0, new WorldPos(x - 1, y, z));
				UpdateIfEqual(x - chunk.Pos.x, ChunkSize - 1, new WorldPos(x + 1, y, z));
				UpdateIfEqual(y - chunk.Pos.y, 0, new WorldPos(x, y - 1, z));
				UpdateIfEqual(y - chunk.Pos.y, ChunkSize - 1, new WorldPos(x, y + 1, z));
				UpdateIfEqual(z - chunk.Pos.z, 0, new WorldPos(x, y, z - 1));
				UpdateIfEqual(z - chunk.Pos.z, ChunkSize - 1, new WorldPos(x, y, z + 1));
			}
		}

		//public void SetBlock(WorldPos pos, Block block, bool createChunkIfNull = false)
		//{
		//	SetBlock(pos.x, pos.y, pos.z, block, createChunkIfNull);
		//}

		//public void SetBlock(int x, int y, int z, Block block, bool createChunkIfNull = false)
		//{
		//	var chunk = GetChunk(x, y, z, createChunkIfNull);

		//	if (chunk != null)
		//	{
		//		chunk.SetBlock(x - chunk.Pos.x, y - chunk.Pos.y, z - chunk.Pos.z, block);
		//		chunk.RebuildNeeded = true;

		//		UpdateIfEqual(x - chunk.Pos.x, 0, new WorldPos(x - 1, y, z));
		//		UpdateIfEqual(x - chunk.Pos.x, ChunkSize - 1, new WorldPos(x + 1, y, z));
		//		UpdateIfEqual(y - chunk.Pos.y, 0, new WorldPos(x, y - 1, z));
		//		UpdateIfEqual(y - chunk.Pos.y, ChunkSize - 1, new WorldPos(x, y + 1, z));
		//		UpdateIfEqual(z - chunk.Pos.z, 0, new WorldPos(x, y, z - 1));
		//		UpdateIfEqual(z - chunk.Pos.z, ChunkSize - 1, new WorldPos(x, y, z + 1));
		//	}
		//}
	}
}
