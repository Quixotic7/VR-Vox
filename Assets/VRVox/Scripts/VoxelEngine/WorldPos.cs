// Core engine code taken from tutorials by AlexStv http://alexstv.com/ please review his license at http://alexstv.com/index.php/posts/unity-voxel-tutorial-licencing 

using System;
using UnityEngine;
using System.Collections;

namespace VRVox
{
	[Serializable]
	public struct WorldPos
	{
		public int x, y, z;

		public WorldPos(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public WorldPos(Vector3 v)
		{
			x = (int)v.x;
			y = (int)v.y;
			z = (int)v.z;
		}

		public Vector3 vector { get { return new Vector3(x,y,z);} }

		public override bool Equals(object obj)
		{
			return GetHashCode() == obj.GetHashCode();
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 47;

				hash = hash * 227 + x.GetHashCode();
				hash = hash * 227 + y.GetHashCode();
				hash = hash * 227 + z.GetHashCode();

				return hash;
			}
		}

		public static WorldPos operator +(WorldPos a, WorldPos b)
		{
			return new WorldPos(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static WorldPos operator +(WorldPos a, Vector3 b)
		{
			return new WorldPos(a.x + (int)b.x, a.y + (int)b.y, a.z + (int)b.z);
		}

		public static WorldPos operator -(WorldPos a, WorldPos b)
		{
			return new WorldPos(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static WorldPos operator -(WorldPos a, Vector3 b)
		{
			return new WorldPos(a.x - (int)b.x, a.y - (int)b.y, a.z - (int)b.z);
		}
	}
}
