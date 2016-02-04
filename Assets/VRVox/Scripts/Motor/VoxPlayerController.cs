using UnityEngine;
using System.Collections;

namespace VRVox
{
	public class VoxPlayerController : MonoBehaviour
	{
		public World voxWorld;

		public float height = 1.8f;

		protected float halfHeight;

		protected bool isGrounded = false;

		protected void Awake()
		{
			halfHeight = height*0.5f;

			//Reset();
		}

		public void Reset()
		{
			//transform.position = new Vector3(-5, 5, -5);
			lerpToNewPosition = false;
			isGrounded = false;
		}

		public float lerpSpeed = 0.05f;
		protected Vector3 newPosition;
		protected bool lerpToNewPosition;

		public void Teleport(Vector3 pos)
		{
			MoveToPosition(pos);
		}

		protected void LateUpdate()
		{
			//if (lerpToNewPosition)
			//{
			//	transform.position = Vector3.Lerp(transform.position, newPosition, lerpSpeed);
			//}
		}

		protected void MoveToPosition(Vector3 newPos)
		{
			var allEmpty = false;

			var origPosition = transform.position;

			transform.position = newPos;


			while (!allEmpty)
			{
				allEmpty = true;

				var playerPos = VoxTerrain.GetBlockPos(transform.position);

				for (var px = -3; px < 3; px++)
				{
					for (var py = -3; py < 5; py++)
					{
						for (var pz = -3; pz < 3; pz++)
						{
							var p = playerPos + new WorldPos(px, py, pz);

							var b = voxWorld.GetBlock(p);

							if (!b.IsEmpty())
							{
								allEmpty = false;
								break;
							}
						}
					}
				}


				//if (!voxWorld.GetBlock(playerPos + new WorldPos(0, -4, 0)).IsEmpty()) // we have ground under the feet
				//{
				//	for (var px = -3; px < 3; px++)
				//	{
				//		for (var py = -3; py < 5; py++)
				//		{
				//			for (var pz = -3; pz < 3; pz++)
				//			{
				//				var p = playerPos + new WorldPos(px, py, pz);

				//				var b = voxWorld.GetBlock(p);

				//				if (!b.IsEmpty())
				//				{
				//					allEmpty = false;
				//					break;
				//				}
				//			}
				//		}
				//	}
				//}
				//else
				//{
				//	allEmpty = false;
				//}

				transform.position = transform.position + Vector3.up; // keep moving player up

				if (transform.position.y > 100) break;
			}

			newPosition = transform.position;
			//transform.position = origPosition;
			//lerpToNewPosition = true;

			Debug.Log("Moving player upwards");
			isGrounded = true;
			
		}

		protected void Update()
		{
			if (!isGrounded && voxWorld.InitialBuildFinished)
			{
				var blockPos = VoxTerrain.GetBlockPos(transform.position);

				MoveToPosition(Vector3.zero);
			}
		}
	}
}