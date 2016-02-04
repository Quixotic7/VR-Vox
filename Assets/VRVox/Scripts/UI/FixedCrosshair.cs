using UnityEngine;
using System.Collections;

namespace VRVox
{
	public class FixedCrosshair : MonoBehaviour{
		public int objectLayer = 8;

		public float objectOffset = 0.5f;

		public float fixedDepth = 3.0f;

		public float lerpSpeed = 0.7f;

		public bool fixCrosshair = false;

		private Material crosshairMaterial = null;
		private Renderer myRenderer;

		private Vector3 newPosition;

		/// <summary>
		/// Initialize the crosshair
		/// </summary>
		private void Awake()
		{
			// clone the crosshair material
			myRenderer = GetComponent<Renderer>();
			crosshairMaterial = myRenderer.material;
		}

		/// <summary>
		/// Cleans up the cloned material
		/// </summary>
		private void OnDestroy()
		{
			if (crosshairMaterial != null)
			{
				Destroy(crosshairMaterial);
			}
		}

		private void LateUpdate()
		{
			if (fixCrosshair)
			{
				transform.localPosition = new Vector3(0, 0, fixedDepth);
			}
			else
			{
				RaycastHit hit;
				if (Physics.Raycast(VoxManager.ViewRay, out hit, 100))
				{
					transform.localPosition = new Vector3(0, 0, hit.distance - objectOffset);
				}
			}
			

			//newPosition = VoxManager.GetPointInView(fixedDepth);

			//transform.position = Vector3.Lerp(transform.position, newPosition, lerpSpeed);
			////transform.forward = -VoxManager.ViewForward;

			//transform.LookAt(VoxManager.ViewPosition);
		}
	}
}