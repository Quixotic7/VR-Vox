using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VRVox
{
	public delegate void VoxMenuButtonAction(VoxMenuButton button, bool state);

	public class VoxMenuButton : MonoBehaviour
	{
		public enum EButtonMode
		{
			normal,
			color,
			openSubMenu,
			toggle
		}

		public EButtonMode buttonMode = EButtonMode.normal;

		public Material defaultMaterial;
		public Material highlightMaterial;
		public Material selectedMaterial;

		public Transform renderTransform; // Move the visual separately to avoid Raycast glitchiness

		public Color buttonColor;

		public VoxMenuSubMenu subMenu;

		public bool hideMenuWhenSelected = true;

		public bool closeAllMenusWhenSelected = false;

		public bool closeAllMenusOnDoubleClick = false;

		public bool isHighlighted;

		public bool isSelected;

		public float animateDistance = 0.02f;

		public float animateTime = 0.3f;

		public string toolTip = "";

		public Text descriptionText;
		public string description;

		protected Color currentColor;

		protected Renderer myRenderer;

		protected Collider myCollider;

		protected VoxMenuButtonAction OnButtonAction = (button, state) => { };

		protected IVoxMenu voxMenu;

		protected Vector3 initPosition;

		protected bool animating;

		protected Button uiButton;
		protected Image uiImage;


		public void SubscribeToButton(VoxMenuButtonAction onButton)
		{
			OnButtonAction += onButton;
		}

		public void SetMenu(IVoxMenu menu)
		{
			voxMenu = menu;
		}

		protected virtual void Awake()
		{
			uiButton = GetComponent<Button>();

			if (renderTransform != null)
			{
				myRenderer = renderTransform.GetComponent<Renderer>();
				initPosition = renderTransform.localPosition;
			}
			else
			{
				myRenderer = GetComponent<Renderer>();

				//Debug.LogWarning("VoxButton: Consider using a render transform for animation or raycasting will be glitchy");
				initPosition = transform.localPosition;
			}
			myCollider = GetComponent<Collider>();


			if (uiButton == null)
			{
				defaultMaterial = new Material(defaultMaterial);
				highlightMaterial = new Material(highlightMaterial);
				selectedMaterial = new Material(selectedMaterial);
			}
			else
			{
				uiImage = GetComponent<Image>();

				if (uiImage != null)
				{
					uiImage.color = uiButton.colors.normalColor;
					uiImage.material = new Material(uiImage.material);
				}
			}

			if (buttonMode == EButtonMode.color)
			{
				buttonColor.a = 1;
				currentColor = buttonColor;
				//defaultMaterial = new Material(defaultMaterial);
				defaultMaterial.color = buttonColor;
			}

			if (buttonMode == EButtonMode.openSubMenu)
			{
				subMenu.SetParentButton(this);
			}

			if(myRenderer != null) myRenderer.material = defaultMaterial;
		}

		protected void OnEnable()
		{
			//isSelected = false;

			isHighlighted = false;

			Reset();

			if (renderTransform != null)
			{
				renderTransform.localPosition = initPosition;
			}
			else
			{
				transform.localPosition = initPosition;
			}

			Highlight(false);
		}

		public void UpdateColor(Color newColor)
		{
			currentColor = newColor;

			defaultMaterial.color = currentColor;
		}

		public void UpdateColor(float saturation, float brightness)
		{
			var hsb = HSBColor.FromColor(buttonColor);
			hsb.b = brightness;
			hsb.s = saturation;

			currentColor = HSBColor.ToColor(hsb);

			defaultMaterial.color = currentColor;
		}

		public Color GetColor()
		{
			return currentColor;
		}

		public void Reset()
		{
			isHighlighted = false;
			//isSelected = false;

			if (uiButton == null)
			{
				if (myRenderer != null) myRenderer.material = isSelected ? selectedMaterial : defaultMaterial;
			}
			else
			{
				if (uiImage != null) uiImage.color = isSelected ? uiButton.colors.pressedColor : uiButton.colors.normalColor;
			}
		}

		public void Highlight(bool state)
		{
			if (!isActiveAndEnabled) return;
			//if(buttonMode == EButtonMode.openSubMenu && subMenu.IsVisible) return;
			//if (isSelected) return;

			isHighlighted = state;
			if (isHighlighted)
			{
				if (uiButton == null)
				{
					if (myRenderer != null) myRenderer.material = isSelected ? selectedMaterial : highlightMaterial;
				}
				else
				{
					if (uiImage != null) uiImage.color = isSelected ? uiButton.colors.pressedColor : uiButton.colors.highlightedColor;
				}
				//transform.localPosition = initPosition - transform.forward*animateDistance;

				if (!lastIsHighlighted)
				{
					StopAllCoroutines();
					StartCoroutine(DoAnimation(initPosition - Vector3.forward * animateDistance, animateTime));
					voxMenu.PlayClip(EVoxMenuSound.Hover);
				}

				if(descriptionText != null) descriptionText.text = description;
			}
			else
			{
				if (uiButton == null)
				{
					if (myRenderer != null) myRenderer.material = isSelected ? selectedMaterial : defaultMaterial;
				}
				else
				{
					if (uiImage != null) uiImage.color = isSelected ? uiButton.colors.pressedColor : uiButton.colors.normalColor;
				}

				//transform.localPosition = initPosition;

				if (lastIsHighlighted)
				{
					StopAllCoroutines();
					StartCoroutine(DoAnimation(initPosition, animateTime));
				}
			}

		}

		public void SubmenuClose()
		{
			if(buttonMode != EButtonMode.openSubMenu) return;
			
			OnButtonAction(this, false);

			//isSelected = false;

			if (myRenderer != null) myRenderer.material = defaultMaterial;
		}

		protected void UpdateMaterial()
		{
			if (uiButton == null)
			{
				if (myRenderer != null)
				{
					if (isHighlighted)
					{
						myRenderer.material = isSelected ? selectedMaterial : highlightMaterial;
					}
					else
					{
						myRenderer.material = isSelected ? selectedMaterial : defaultMaterial;
					}
				}
			}
			else
			{
				
				if (uiImage != null)
				{
					if (isHighlighted)
					{
						uiImage.color = isSelected ? uiButton.colors.pressedColor : uiButton.colors.highlightedColor;
					}
					else
					{
						uiImage.color = isSelected ? uiButton.colors.pressedColor : uiButton.colors.disabledColor;
					}
					
				}
			}
			
		}

		public void Select(bool state)
		{
			switch (buttonMode)
			{
				case EButtonMode.toggle:
					isSelected = !isSelected;
					Highlight(false);
					break;
				case EButtonMode.openSubMenu:

					if (!subMenu.IsVisible)
					{
						//isSelected = true;
						subMenu.ShowMenu(true);

						voxMenu.PlayClip(EVoxMenuSound.MenuOpen);
					}
					else
					{
						if (subMenu.CanClose) // This is to prevent the button from being clicked if submenu physically overlaps
						{
							//isSelected = false;
							subMenu.ShowMenu(false);
							voxMenu.PlayClip(EVoxMenuSound.MenuClose);
						}
					}

					break;
				default:
					isSelected = false;
					Highlight(false);
					voxMenu.PlayClip(EVoxMenuSound.Afirmative);
					break;
			}

			OnButtonAction(this, state);

			if (buttonMode == EButtonMode.toggle)
			{
				voxMenu.PlayClip(isSelected ? EVoxMenuSound.Enabled : EVoxMenuSound.Disabled);
			}


			UpdateMaterial();

			if (hideMenuWhenSelected) voxMenu.ShowMenu(false);
			if (closeAllMenusWhenSelected) voxMenu.CloseAllMenus();
		}

		public void DoubleClick()
		{
			if (closeAllMenusOnDoubleClick)
			{
				//isSelected = false;
				Highlight(false);
				voxMenu.CloseAllMenus();
			}
		}

		protected bool lastIsHighlighted;

		protected void Update()
		{
			//if (isHighlighted != lastIsHighlighted) Highlight(isHighlighted);

			//lastIsHighlighted = isHighlighted;
			lastIsHighlighted = isHighlighted;

		}

		public bool RayCast(Ray ray, out RaycastHit hitInfo, float distance)
		{
			if (myCollider != null) return myCollider.Raycast(ray, out hitInfo, distance);
			hitInfo = new RaycastHit();
			return false;
		}


		IEnumerator DoAnimation(Vector3 targetPos, float duration)
		{
			animating = true;

			var pos = 0f;

			var startPos = renderTransform == null ? transform.localPosition : renderTransform.localPosition;

			while (pos < 1f)
			{
				if (renderTransform != null)
				{
					renderTransform.localPosition = Vector3.Slerp(startPos, targetPos, pos);
				}
				else
				{
					transform.localPosition = Vector3.Slerp(startPos, targetPos, pos);
				}

				pos += Time.deltaTime/duration;

				yield return new WaitForEndOfFrame();
			}


			if (renderTransform != null)
			{
				renderTransform.localPosition = targetPos;
			}
			else
			{
				transform.localPosition = targetPos;
			}
			animating = false;
		}

	}
}