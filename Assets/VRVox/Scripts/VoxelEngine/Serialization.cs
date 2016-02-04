using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VRVox
{
	public static class Serialization
	{
		public static string SaveFolderName = "VoxSaves";

		public static string WorldInfoName = "WorldInfo";


		public static string SaveLocation(string name)
		{
			var path = Path.Combine(Application.persistentDataPath, SaveFolderName);
			path = Path.Combine(path, name);

			Debug.Log("SaveLocation = " + path);

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			return path;
		}

		public static string FileName(WorldPos chunkLocation)
		{
			return chunkLocation.x + "," + chunkLocation.y + "," + chunkLocation.z + ".bin";
		}

		public static void SaveChunk(Chunk chunk)
		{
			var save = new VoxSave(chunk);
			if (save.Blocks.Count == 0) return; 

			var path = Path.Combine(SaveLocation(chunk.World.WorldName), FileName(chunk.Pos));

			IFormatter formatter = new BinaryFormatter();
			var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
			formatter.Serialize(stream, save);
			stream.Close();
		}

		public static bool Load(Chunk chunk)
		{
			 var path = Path.Combine(SaveLocation(chunk.World.WorldName), FileName(chunk.Pos));

			if (!File.Exists(path))
				return false;

			IFormatter formatter = new BinaryFormatter();
			var stream = new FileStream(path, FileMode.Open);

			var save = (VoxSave)formatter.Deserialize(stream);

			foreach (var block in save.Blocks)
			{
				chunk.Blocks[block.Key.x, block.Key.y, block.Key.z] = block.Value;
			}

			stream.Close();
			return true;
		}

		public static bool DeleteSaves(World world)
		{
			//var path = Path.Combine(SaveLocation(world.WorldName), FileName(chunk.Pos));

			var path = SaveLocation(world.WorldName);

			if (!Directory.Exists(path))
				return false;

			var files = Directory.GetFiles(path);

			foreach (var file in files)
			{
				File.Delete(file);
			}

			return true;


			//if (!File.Exists(path))
			//	return false;

			//IFormatter formatter = new BinaryFormatter();
			//var stream = new FileStream(path, FileMode.Open);

			//var save = (VoxSave)formatter.Deserialize(stream);

			//foreach (var block in save.Blocks)
			//{
			//	chunk.Blocks[block.Key.x, block.Key.y, block.Key.z] = block.Value;
			//}

			//stream.Close();
			//return true;

			
		}


		public static void SaveWorldInfo(World world, SavedWorldInfo worldInfo)
		{
			var path = Path.Combine(SaveLocation(world.WorldName), WorldInfoName + ".bin");

			IFormatter formatter = new BinaryFormatter();
			var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
			formatter.Serialize(stream, worldInfo);
			stream.Close();
		}

		public static SavedWorldInfo LoadWorldInfo(World world)
		{
			var path = Path.Combine(SaveLocation(world.WorldName), WorldInfoName + ".bin");

			if (!File.Exists(path))
				return null;

			IFormatter formatter = new BinaryFormatter();
			var stream = new FileStream(path, FileMode.Open);

			var save = (SavedWorldInfo)formatter.Deserialize(stream);

			//foreach (var block in save.Blocks)
			//{
			//	chunk.Blocks[block.Key.x, block.Key.y, block.Key.z] = block.Value;
			//}

			stream.Close();
			return save;
		}
	}
}