using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VRVox
{
	public class VoxColorSelector : MonoBehaviour, IVoxUIElement
	{
		public VoxMenu mainMenu;
		public VoxMenuSubMenu subMenu;

		public Color myColor;

		public HSBColor hsbColor;

		public Material colorMaterial;

		public Renderer colorRenderer;
		public VoxMenuButton colorButton;
		//public VoxMenuButton colorAButton;

		public VoxUser VoxUserScript;

		public float Brightness;

		public VoxSlider HueSlider;
		public VoxSlider SaturationSlider;
		public VoxSlider ValueSlider;

		public VoxMenuButton eyeDropButton;

		protected bool EnableEyeDropper;

		protected List<VoxMenuButton> colorButtons;

		protected float uiAlpha;

		protected void Awake()
		{

			//uiAlpha = colorAButton.defaultMaterial.color.a;
			//colorAButton.defaultMaterial = new Material(colorMaterial);
			//colorAButton.defaultMaterial.color = new Color(myColor.r, myColor.g, myColor.b, uiAlpha);
			colorMaterial = new Material(colorMaterial);
			colorMaterial.color = myColor;

			colorButtons = new List<VoxMenuButton>();

			for (var i = 0; i < subMenu.transform.childCount; i++)
			{
				var cb = subMenu.transform.GetChild(i).GetComponent<VoxMenuButton>();

				if (!cb) continue;

				if (cb.buttonMode != VoxMenuButton.EButtonMode.color) continue;

				cb.SubscribeToButton(OnButtonSelect);
			}

			eyeDropButton.SubscribeToButton(OnEyedropper);

			//colorAButton.SubscribeToButton(OnColorMenu);
		}

		protected void Start()
		{
			subMenu.AddUIElement(this);

			SetColor(VoxUserScript.NewBlockColor);

			colorRenderer.material = colorMaterial;

			colorButton.defaultMaterial = colorMaterial;
			colorButton.highlightMaterial = colorMaterial;
			colorButton.selectedMaterial = colorMaterial;
		}

		protected void OnDisable()
		{
			EnableEyeDropper = false;
			eyeDropButton.gameObject.SetActive(true);
		}

		protected void OnEnable()
		{
			SetColor(VoxUserScript.NewBlockColor);
		}

		protected void Update()
		{
			if (!subMenu.IsVisible)
			{
				HueSlider.Value = hsbColor.h;
				SaturationSlider.Value = hsbColor.s;
				ValueSlider.Value = hsbColor.b;
				return;
			}
			//hsbColor = HSBColor.FromColor(myColor);

			hsbColor.h = HueSlider.Value;
			hsbColor.s = SaturationSlider.Value;
			hsbColor.b = ValueSlider.Value;

			myColor = HSBColor.ToColor(hsbColor);

			colorMaterial.color = myColor;
			VoxUserScript.NewBlockColor = myColor;

			//foreach (var button in colorButtons)
			//{
			//	button.UpdateColor(0.5f, Brightness);
			//}
		}

		protected void LateUpdate()
		{
			CanClose = true;
			PickColor();
		}

		protected void PickColor()
		{
			if (!EnableEyeDropper) return;

			CanClose = false;

			//if (!subMenu.CanClose) return; // this would be false if any ui elements are highlighted

			var click = false;
			//var clickDown = false;
			var clickUp = false;

			//if (Input.GetButtonDown(VoxInput.rightShoulder))
			//{
			//	clickDown = true;
			//}
			//if (VoxInput.GetSelectDown())
			//{
			//	clickDown = true;
			//}

			var clickDown = VoxInput.GetSelectDown() || Input.GetButtonDown(VoxInput.rightShoulder) || Input.GetButtonDown(VoxInput.ButtonA);

			if (Input.GetButton(VoxInput.rightShoulder)) click = true;
			if (VoxInput.GetSelect()) click = true;

			if (Input.GetButtonUp(VoxInput.rightShoulder)) clickUp = true;
			if (VoxInput.GetSelectUp()) clickUp = true;

			if (clickUp) click = false;

			RaycastHit hit;
			var ray = VoxManager.ViewRay;

			if (Physics.Raycast(ray, out hit, 50))
			{
				var chunk = hit.collider.GetComponent<Chunk>();
				if (chunk != null)
				{
					if (clickDown)
					{
						var block = chunk.World.GetBlock(VoxTerrain.GetBlockPos(hit));
						SetColor(block.BlockColor);
						//EnableEyeDropper = false;
						eyeDropButton.gameObject.SetActive(true);
					}
				}
			}
		}

		protected void SetColor(Color color)
		{
			myColor = color;
			myColor.a = 1f;
			hsbColor = HSBColor.FromColor(myColor);
			HueSlider.Value = hsbColor.h;
			SaturationSlider.Value = hsbColor.s;
			ValueSlider.Value = hsbColor.b;

			colorMaterial.color = myColor;
			VoxUserScript.NewBlockColor = myColor;
		}

		protected void OnButtonSelect(VoxMenuButton button, bool state)
		{
			if (!state) return;

			SetColor(button.GetColor());
		}

		protected void OnEyedropper(VoxMenuButton button, bool state)
		{
			if (!state) return;

			EnableEyeDropper = true;

			mainMenu.toolTipText.text = "Click a block to pick color";
		}

		protected void OnColorMenu(VoxMenuButton button, bool state)
		{
			if (!state) return;
		}

		public bool CanClose { get; private set; }
	}
}