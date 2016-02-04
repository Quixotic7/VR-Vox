using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VRVox
{
	public class VoxMenuSubMenu : MonoBehaviour, IVoxMenu
	{
		protected IVoxMenu parentMenu;

		protected VoxMenuButton parentButton;

		protected List<VoxMenuButton> menuButtons;

		protected List<IVoxUIElement> uiElements;


		protected bool showMenu = false;

		public bool IsVisible { get { return showMenu; } }

		public bool CanClose { get; private set; }

		protected void Awake()
		{
			uiElements = new List<IVoxUIElement>();
			menuButtons = VoxMenu.GetButtons(transform);

			foreach (var button in menuButtons)
			{
				button.SetMenu(this);
			}

			ShowMenu(showMenu);
		}

		public void PlayClip(EVoxMenuSound soundType)
		{
			parentMenu.PlayClip(soundType);
		}

		public void SetMenu(IVoxMenu menu)
		{
			parentMenu = menu;
		}

		public void AddUIElement(IVoxUIElement element)
		{
			uiElements.Add(element);
		}

		public void SetParentButton(VoxMenuButton button)
		{
			parentButton = button;
		}

		public void CloseAllMenus()
		{
			parentMenu.CloseAllMenus();
		}

		public void ShowMenu(bool state)
		{
			//parentMenu.ShowMenu(state);
			showMenu = state;

			if (showMenu)
			{
				gameObject.SetActive(true);
			}
			else
			{
				foreach (var menuButton in menuButtons)
				{
					menuButton.Reset();
				}
				if(parentButton) parentButton.SubmenuClose();
				gameObject.SetActive(false);
			}
		}

		protected void HighlightButtons(bool state)
		{
			foreach (var button in menuButtons)
			{
				button.Highlight(state);
			}
		}

		protected void Update()
		{
			//if (Input.GetKeyDown(KeyCode.Escape))
			//{
			//	ShowMenu(!showMenu);

			//}
		}

		public float doubleClickDelay = 0.25f;

		protected float doubleClickTimer = 0f;

		protected void LateUpdate()
		{
			if (showMenu)
			{
				var select = VoxInput.GetSelectDown() || Input.GetButtonDown(VoxInput.rightShoulder) || Input.GetButtonDown(VoxInput.ButtonA);

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

				//var submenuOpen = IsSubmenuOpen();

				foreach (var button in menuButtons)
				{
					if (button.RayCast(ray, out hit, 25f))
					{
						rayHit = true;

						button.Highlight(true);

						if (select) button.Select(true);
						if(doubleClick) button.DoubleClick();
					}
					else
					{
						button.Highlight(false);
					}
				}

				CanClose = !rayHit;

				foreach (var element in uiElements)
				{
					if (!element.CanClose)
					{
						CanClose = false;
						break;
					}
				}

				if (CanClose && select)
				{
					ShowMenu(false);
					parentMenu.PlayClip(EVoxMenuSound.MenuClose);
				}
			}
		}
	}
}