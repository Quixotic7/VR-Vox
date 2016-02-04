using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace VRVox
{
	public interface IVoxMenu
	{
		bool IsVisible { get; }
		void ShowMenu(bool state);
		void CloseAllMenus();
		void PlayClip(EVoxMenuSound soundType);
	}

	public enum EVoxMenuSound
	{
		Click,
		Hover,
		MenuOpen,
		MenuClose,
		Enabled,
		Disabled,
		Afirmative
	}

	public class VoxMenu : MonoBehaviour, IVoxMenu
	{
		public VoxUser Player;
		public VoxCrosshair Crosshair;
		//public GameObject Crosshair;

		public Text toolTipText;

		public Text fpsText;

		public VoxFPS voxFPS;

		public FixedCrosshair fixedCrosshair;

		public float fixedCameraOffset = 1.5f;

		public GameObject menuRoot;

		public AudioSource audioSource;

		public AudioClip sndOnClick;
		public AudioClip sndOnHover;
		public AudioClip sndOnMenuOpen;
		public AudioClip sndOnMenuClose;

		public AudioClip sndOnEnable;
		public AudioClip sndOnDisable;
		public AudioClip sndOnAffirm;


		protected bool showMenu = false;

		protected Vector3 initialForward;

		protected List<VoxMenuButton> menuButtons;
		protected List<VoxMenuSubMenu> subMenus;

		public bool IsVisible { get { return showMenu; } }

		protected void Awake()
		{
			menuButtons = GetButtons(menuRoot.transform);

			foreach (var button in menuButtons)
			{
				button.SetMenu(this);
			}

			subMenus = GetSubmenus(menuRoot.transform);

			foreach (var subMenu in subMenus)
			{
				subMenu.SetMenu(this);
			}

			toolTipText.text = "";
		}

		public void PlayClip(EVoxMenuSound soundType)
		{
			switch (soundType)
			{
				case EVoxMenuSound.Click:
					PlayClip(sndOnClick);
					break;
				case EVoxMenuSound.Hover:
					PlayClip(sndOnHover);
					break;
				case EVoxMenuSound.MenuClose:
					PlayClip(sndOnMenuClose);
					break;
				case EVoxMenuSound.MenuOpen:
					PlayClip(sndOnMenuOpen);
					break;
				case EVoxMenuSound.Enabled:
					PlayClip(sndOnEnable);
					break;
				case EVoxMenuSound.Disabled:
					PlayClip(sndOnDisable);
					break;
				case EVoxMenuSound.Afirmative:
					PlayClip(sndOnAffirm);
					break;
				default:
					PlayClip(sndOnClick);
					break;
			}
		}

		public void PlayClip(AudioClip clip)
		{
			audioSource.PlayOneShot(clip);
		}

		public void PlayClip(AudioClip clip, float volume)
		{
			audioSource.PlayOneShot(clip, volume);
		}

		protected void Start()
		{
			ShowMenu(showMenu);
		}

		public static List<VoxMenuButton> GetButtons(Transform t)
		{
			var buttons = new List<VoxMenuButton>();

			for (var i = 0; i < t.childCount; i++)
			{
				var child = t.GetChild(i);

				var submenu = child.GetComponent<VoxMenuSubMenu>();
				if (submenu) continue;

				buttons.AddRange(GetButtons(child));
			}

			var button = t.gameObject.GetComponent<VoxMenuButton>();

			if (button != null) buttons.Add(button);

			return buttons;
		}

		public static List<VoxMenuSubMenu> GetSubmenus(Transform t)
		{
			var submenus = new List<VoxMenuSubMenu>();

			for (var i = 0; i < t.childCount; i++)
			{
				var submenu = t.GetChild(i).GetComponent<VoxMenuSubMenu>();

				if(submenu) submenus.Add(submenu);
			}

			return submenus;
		}

		public void CloseAllMenus()
		{
			ShowMenu(false);
			doubleClickTimer = 2f;
		}

		public void ShowMenu(bool state)
		{
			showMenu = state;

			if (showMenu)
			{
				menuRoot.SetActive(true);

				transform.position = VoxManager.ViewPosition;
				transform.forward = VoxManager.ViewForward;
				menuRoot.transform.position = VoxManager.GetPointInView(fixedCameraOffset);

				initialForward = VoxManager.ViewForward;
				//Crosshair.gameObject.SetActive(false);
				//FixedCrosshair.

				fixedCrosshair.fixCrosshair = true;

				PlayClip(sndOnMenuOpen);
			}
			else
			{
				foreach (var subMenu in subMenus)
				{
					subMenu.ShowMenu(false);
				}

				menuRoot.SetActive(false);
				toolTipText.text = "";
				//Crosshair.gameObject.SetActive(true);
				//FixedCrosshair.SetActive(false);

				fixedCrosshair.fixCrosshair = false;
				highlightedButton = null;
			}
		}

		protected void HighlightButtons(bool state)
		{
			foreach (var button in menuButtons)
			{
				button.Highlight(state);
			}
		}

		protected void TryClose()
		{
			var canClose = true;
			foreach (var subMenu in subMenus)
			{
				if (subMenu.IsVisible)
				{
					canClose = false;
					subMenu.ShowMenu(false);

					foreach (var button in menuButtons)
					{
						button.Reset();
					}
				}
			}
			HighlightButtons(false);
			if (canClose)
			{
				foreach (var button in menuButtons)
				{
					button.Reset();
				}
				ShowMenu(false);
				//PlayClip(sndOnMenuClose);
			}
			
		}

		protected void Update()
		{
			if (!VoxManager.InitialBuildFinished) return;

			//if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown(VoxInput.ButtonA) || Input.GetButtonDown(VoxInput.Start))
			//{
			//	if (showMenu) TryClose();
			//	else ShowMenu(true);

			//	ShowMenu(true);
			//}

			if (!showMenu)
			{
				if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown(VoxInput.ButtonA))
				{
					//if (showMenu) TryClose();
					//else ShowMenu(true);

					ShowMenu(true);
					canSelectTimer = 0.15f;
				}

			}
			else
			{
				if (Input.GetButtonDown(VoxInput.leftShoulder) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown(VoxInput.ButtonB))
				{
					PlayClip(EVoxMenuSound.MenuClose);
					TryClose();
				}
			}
			

			

			fpsText.text = "FPS: " + voxFPS.FramesPerSecond;
		}

		protected bool IsSubmenuOpen()
		{
			foreach (var subMenu in subMenus)
			{
				if (subMenu.IsVisible) return true;
			}
			return false;
		}

		public float doubleClickDelay = 0.25f;

		protected float doubleClickTimer = 0f;

		protected float canSelectTimer;

		protected VoxMenuButton highlightedButton;

		protected void LateUpdate()
		{
			if (showMenu)
			{
				canSelectTimer -= Time.deltaTime;
				
				var select = false;
				if (canSelectTimer < 0) select = VoxInput.GetSelectDown() || Input.GetButtonDown(VoxInput.rightShoulder) || Input.GetButtonDown(VoxInput.ButtonA);

				var doubleClick = false;

				if (doubleClickTimer <= doubleClickDelay)
				{
					if (select) doubleClick = true;
				}
				else
				{
					if (select)
					{
						doubleClickTimer = 0f;
					}
				}

				doubleClickTimer += Time.deltaTime;


				var rayHit = false;
				var ray = VoxManager.ViewRay;
				RaycastHit hit;

				var submenuOpen = IsSubmenuOpen();

				foreach (var button in menuButtons)
				{
					if (!submenuOpen)
					{
						if (button.RayCast(ray, out hit, 25f))
						{
							rayHit = true;
							//HighlightButtons(false);

							if (highlightedButton == null)
							{
								button.Highlight(true);
								if (!submenuOpen) toolTipText.text = button.toolTip;
								highlightedButton = button;
							}
							else if (highlightedButton != button)
							{
								button.Highlight(true);
								if(!submenuOpen) toolTipText.text = button.toolTip;
								highlightedButton = button;
							}

							if (select) button.Select(true);
							if (doubleClick) button.DoubleClick();
						}
						else
						{
							if (highlightedButton != null)
							{
								if (button == highlightedButton) highlightedButton = null;
							}
							button.Highlight(false);
						}
					}
				}

				if (!submenuOpen && !rayHit)
				{
					toolTipText.text = "";
				}

				if (!submenuOpen && !rayHit && select)
				{
					PlayClip(EVoxMenuSound.MenuClose);
					TryClose();
				}
			}
			else
			{
				//if (doubleClick && Crosshair.CanDoubleClickToOpenMenu) ShowMenu(true);
			}
		}
	}
}