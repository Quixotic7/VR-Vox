using UnityEngine;
using System.Collections;

namespace VRVox
{
	public class VoxQuitGame : MonoBehaviour
	{
		public VoxMenuButton quitButton;

		private bool platformUIStarted = false;

		private float doubleTapDelay = 0.5f;
		private float longPressDelay = 1f;
		private float homeButtonDownTime = 0.0f;

		protected void Start()
		{
			quitButton.SubscribeToButton(OnQuitButton);
		}

		protected void OnQuitButton(VoxMenuButton button, bool state)
		{
			if (!state) return;

			ShowConfirmQuitMenu();
		}

		/// <summary>
		/// Reset when resuming
		/// </summary>
		void OnApplicationFocus()
		{
			platformUIStarted = false;
			Input.ResetInputAxes();
			homeButtonDownTime = 0.0f;
		}

		/// <summary>
		/// Show the confirm quit menu
		/// </summary>
		private void ShowConfirmQuitMenu()
		{
			
#if UNITY_ANDROID && !UNITY_EDITOR

        // TODO : Switched to Unity Built-in VR, Application.Quit should work, but it might not. 
//		Debug.Log("[PlatformUI-ConfirmQuit] Showing @ " + Time.time);
//		OVRManager.PlatformUIConfirmQuit();
//		platformUIStarted = true;
#endif

			Application.Quit();
		}

		/// <summary>
		/// Show the platform UI global menu
		/// </summary>
		private void ShowGlobalMenu()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
        // TODO : Switched to Unity Built-in VR, not sure how to bring up the global menu here

//		Debug.Log("[PlatformUI-Global] Showing @ " + Time.time);
//		OVRManager.PlatformUIGlobalMenu();
//		platformUIStarted = true;
#endif
        }

        protected bool CanHoldBack = false;

		void Update()
		{
			if (!platformUIStarted)
			{
				// process input for the home button
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					homeButtonDownTime = 0.0f;
				}
				else if (Input.GetKeyUp(KeyCode.Escape))
				{
					homeButtonDownTime = 0.0f;
					CanHoldBack = true;
				}
				else if (Input.GetKey(KeyCode.Escape))
				{
					if (CanHoldBack) homeButtonDownTime += Time.deltaTime;

					// Check for long press
					if (homeButtonDownTime >= longPressDelay)
					{
						// reset so something else doesn't trigger afterwards
						Input.ResetInputAxes();
						homeButtonDownTime = 0.0f;

						Invoke("ShowGlobalMenu",0.1f);
						CanHoldBack = false;
					}
				}
			}
		}
	}
}
