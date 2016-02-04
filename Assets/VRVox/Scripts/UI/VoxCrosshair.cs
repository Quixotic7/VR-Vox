using UnityEngine;
using System.Collections;

namespace VRVox
{
	public class VoxCrosshair : MonoBehaviour
	{
		public enum CrosshairMode
		{
			Add,
			Subtract
		}

		public CrosshairMode mode;
		private CrosshairMode lastMode;

		public int objectLayer = 8;
		public float offsetFromObjects = 0.1f;
		public float fixedDepth = 3.0f;
		//public OVRCameraRig cameraController = null;

		public Texture AddTexture;
		public Texture SubtractTexture;

		public float LerpSpeed = 0.6f;

		public bool lockToPlane;
		public Plane ConstructionPlane;

		private Material crosshairMaterial = null;
		private Renderer myRenderer;

		public Vector3 newPosition;

		private float distanceFromCamera;

		public bool CanDoubleClickToOpenMenu;

		/// <summary>
		/// Initialize the crosshair
		/// </summary>
		private void Awake()
		{
			//if (cameraController == null)
			//{
			//	Debug.LogError("ERROR: missing camera controller object on " + name);
			//	enabled = false;
			//	return;
			//}
			// clone the crosshair material
			myRenderer = GetComponent<Renderer>();
			crosshairMaterial = myRenderer.material;

			ConstructionPlane = new Plane(Vector3.forward, 10);

			ChangeTexture();
		}

		public void SetColor(Color color)
		{
			crosshairMaterial.color = color;
		}

		public void SetCrosshairMode(CrosshairMode interactionMode)
		{
			mode = interactionMode;
		}

		public void SetCrosshairMode(bool addMode)
		{
			mode = addMode ? CrosshairMode.Add : CrosshairMode.Subtract;
		}

		public void SetVisible(bool state)
		{
			myRenderer.enabled = state;
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

		private void ChangeTexture()
		{
			switch (mode)
			{
				case CrosshairMode.Add:
					crosshairMaterial.mainTexture = AddTexture;
					break;
				case CrosshairMode.Subtract:
					crosshairMaterial.mainTexture = SubtractTexture;
					break;
			}
		}

		//protected void FixedUpdate()
		//{
		//	//transform.position = Vector3.Lerp(transform.position, newPosition, LerpSpeed);

		//}

		/// <summary>
		/// Updates the position of the crosshair.
		/// </summary>
		private void LateUpdate()
		{
			if (mode != lastMode)
			{
				ChangeTexture();
			}

			lastMode = mode;

			//Ray ray;
			//RaycastHit hit;

			transform.position = Vector3.Lerp(transform.position, newPosition, LerpSpeed);


			//transform.position = newPosition;


			// get the camera forward vector and position
			//var cameraPosition = VoxManager.ViewPosition;
			//var cameraForward = VoxManager.ViewForward;

			return;

			//GetComponent<Renderer>().enabled = true;


			//var plane = VoxUser.GetXYZPlaneFacingDirection(cameraForward, Vector3.zero);

			//transform.forward = plane.normal;


			//transform.position = cameraPosition + cameraForward*10f;

			//ConstructionPlane = VoxUser.GetXYZPlaneFacingDirection(cameraForward, cameraPosition + cameraForward*5f);
			//transform.forward = ConstructionPlane.normal;










			//if (!lockToPlane)
			//{

			//	ray = new Ray(cameraPosition, cameraForward);
			//	if (Physics.Raycast(ray, out hit))
			//	{
			//		CanDoubleClickToOpenMenu = false;

			//		//myRenderer.enabled = true;
			//		//newPosition = cameraPosition + cameraForward * hit.distance;

			//		transform.forward = hit.normal;

			//		newPosition = VoxTerrain.GetUnityPosition(VoxTerrain.GetBlockPos(hit.point - hit.normal * World.UnitScale * 0.5f)) + hit.normal * World.UnitScale * 0.5f;

			//		if (Input.GetButtonDown(OVRGamepadController.ButtonNames[(int)OVRGamepadController.Button.A]))
			//		{
			//			hit.transform.gameObject.BroadcastMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			//		}
			//	}
			//	else
			//	{
			//		CanDoubleClickToOpenMenu = true;

			//		transform.forward = ConstructionPlane.normal;

			//		float enter = 0f;
			//		if (ConstructionPlane.Raycast(ray, out enter))
			//		{
			//			var pointOnPlane = ray.origin + ray.direction*enter;
			//			newPosition = pointOnPlane;
			//		}
			//	}
			//}










			//else
			//{
			//	//newPosition = cameraPosition + cameraForward*distanceFromCamera;
			//	//newPosition = cameraPosition - cameraForward * 10f;
			//	//transform.position = newPosition;
			//	//float enter;
			//	//if (ConstructionPlane.Raycast(ray, out enter))
			//	//{
			//	//	//Debug.Log("Enter = " + enter);
			//	//	var pointOnPlane = ray.origin + ray.direction * enter;

			//	//	//newPosition = pointOnPlane;

			//	//	//transform.position = pointOnPlane;

			//	//	//transform.forward = ConstructionPlane.normal;
			//	//}

			//}


			

			//if (OVRManager.display.isPresent)
			//{
				
			//}
			//else
			//{
			//	GetComponent<Renderer>().enabled = false;
			//}
		}
	}
}