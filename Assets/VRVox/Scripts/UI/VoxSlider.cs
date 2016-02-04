using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using VRVox;

namespace VRVox
{
	public interface IVoxUIElement
	{
		bool CanClose { get; }	
	}

	public class VoxSlider : MonoBehaviour, IVoxUIElement
	{
		public VoxMenuSubMenu subMenu;

		public Transform control;

		public float Value
		{
			get { return sliderValue; }
			set
			{
				var clamped = Mathf.Clamp01(value);

				var unityY = (clamped*(unityRange.y - unityRange.x)) + unityRange.x;

				control.localPosition = new Vector3(0, unityY, zOffset);
					
				sliderValue = value;
			}
		}

		protected float sliderValue;

		public Vector2 unityRange;

		public Vector2 range;

		public float yValue;

		public float zOffset = -0.02f;

		public Material defaultMaterial;
		public Material highlightMaterial;

		public float animateDistance = 0.02f;

		public float animateTime = 0.3f;

		protected bool isHighlighted;
		protected bool lastIsHighlighted;

		protected Renderer myRenderer;
		protected Collider myCollider;

		protected bool sliderGrabbed;

		protected Plane dragPlane;

		protected Vector3 initPosition;

		protected bool animating;

		public bool CanClose { get; private set; }

		protected void Awake()
		{
			myCollider = GetComponent<Collider>();
			myRenderer = GetComponent<Renderer>();

			defaultMaterial = new Material(defaultMaterial);
			highlightMaterial = new Material(highlightMaterial);

			myRenderer.material = defaultMaterial;

			Value = sliderValue;

			initPosition = control.localPosition;

			var controlRenderer = control.GetComponent<MeshRenderer>();

			if (controlRenderer != null)
			{
				controlRenderer.material = new Material(controlRenderer.material);
			}
		}

		protected void Start()
		{
			subMenu.AddUIElement(this);
		}

		protected void OnEnable()
		{
			var unityY = (sliderValue * (unityRange.y - unityRange.x)) + unityRange.x;

			control.localPosition = new Vector3(0, unityY, zOffset);
		}

		protected void SetSlider(Vector3 position)
		{
			var pointLocal = transform.InverseTransformPoint(position);

			var clampedY = Mathf.Clamp(pointLocal.y, unityRange.x, unityRange.y);

			control.localPosition = new Vector3(0, clampedY, zOffset);

			yValue = pointLocal.y;

			var normalize = (clampedY + Mathf.Abs(unityRange.x)) / Mathf.Max((Mathf.Abs(unityRange.x) + Mathf.Abs(unityRange.y)), float.Epsilon);

			sliderValue = normalize;
		}

		public void Highlight(bool state)
		{
			var startPos = control.localPosition;
			startPos.z = initPosition.z;

			isHighlighted = state;
			if (isHighlighted)
			{
				myRenderer.material = highlightMaterial;

				if (!lastIsHighlighted)
				{
					StopAllCoroutines();
					
					StartCoroutine(DoAnimation(startPos - Vector3.forward * animateDistance, animateTime));
				}
			}
			else
			{
				myRenderer.material = defaultMaterial;

				if (lastIsHighlighted)
				{
					StopAllCoroutines();
					StartCoroutine(DoAnimation(startPos, animateTime));
				}
			}

		}

		IEnumerator DoAnimation(Vector3 targetPos, float duration)
		{
			animating = true;

			var pos = 0f;

			var startPos = control.localPosition;

			while (pos < 1f)
			{
				var start = control.localPosition;
				start.z = startPos.z;

				control.localPosition = Vector3.Slerp(start, targetPos, pos);

				pos += Time.deltaTime / duration;

				yield return new WaitForEndOfFrame();
			}

			control.localPosition = targetPos;
			
			animating = false;
		}

		protected void Update()
		{
			if (isHighlighted != lastIsHighlighted) Highlight(isHighlighted);

			lastIsHighlighted = isHighlighted;
		}

		protected void LateUpdate()
		{
			var rayHit = false;
			var ray = VoxManager.ViewRay;
			RaycastHit hit;

			var click = false;
			var clickDown = false;
			var clickUp = false;

			if (Input.GetButtonDown(VoxInput.rightShoulder))
			{
				clickDown = true;
			}
			if (VoxInput.GetSelectDown())
			{
				clickDown = true;
			}
			if (Input.GetButtonDown(VoxInput.ButtonA))
			{
				clickDown = true;
			}

			if (Input.GetButton(VoxInput.rightShoulder)) click = true;
			if (Input.GetButton(VoxInput.ButtonA)) click = true;
			if (VoxInput.GetSelect()) click = true;

			if (Input.GetButtonUp(VoxInput.rightShoulder)) clickUp = true;
			if (Input.GetButtonUp(VoxInput.ButtonA)) clickUp = true;
			if (VoxInput.GetSelectUp()) clickUp = true;

			if (clickUp) click = false;

			if (!sliderGrabbed)
			{
				if (myCollider.Raycast(ray, out hit, 25))
				{
					rayHit = true;

					if (clickDown)
					{
						SetSlider(hit.point);

						sliderGrabbed = true;

						dragPlane = new Plane(transform.forward, transform.position);
					}

					if(!isHighlighted) Highlight(true);
				}
				else if(isHighlighted) Highlight(false);
			}
			else
			{
				if (click)
				{
					float enter;

					if (dragPlane.Raycast(ray, out enter))
					{
						var posOnPlane = ray.origin + ray.direction*enter;

						SetSlider(posOnPlane);
					}
					CanClose = false;
				}
				else
				{
					sliderGrabbed = false;
				}
			}

			CanClose = !rayHit;
		}
	}
}