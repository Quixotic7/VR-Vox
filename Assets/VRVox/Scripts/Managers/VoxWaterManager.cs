using UnityEngine;
using System.Collections;
using VRVox;

public class VoxWaterManager : MonoBehaviour
{

	public GameObject normalWater;
	public GameObject hiResWater;

	public bool hiResWaterEnabled;
	public bool enableWater;

	public VoxMenuButton toggleWaterButton;
	public VoxMenuButton toggleHiResButton;

	protected void Start () {
		toggleWaterButton.SubscribeToButton(OnToggleWater);
		toggleHiResButton.SubscribeToButton(OnToggleHiResWater);

		toggleWaterButton.isSelected = enableWater;
		toggleHiResButton.isSelected = hiResWaterEnabled;

		ToggleWater(enableWater);
	}

	protected void OnToggleHiResWater(VoxMenuButton button, bool state)
	{
		if (!state) return;

		ToggleHighResWater(button.isSelected);
	}

	protected void OnToggleWater(VoxMenuButton button, bool state)
	{
		if (!state) return;

		ToggleWater(button.isSelected);
	}

	public void ToggleWater(bool enabled)
	{
		enableWater = enabled;

		if (enableWater)
		{
			ToggleHighResWater(hiResWaterEnabled);
		}
		else
		{
			normalWater.SetActive(false);
			hiResWater.SetActive(false);
		}
	}

	public void ToggleHighResWater(bool hiRes)
	{
		hiResWaterEnabled = hiRes;

		if (!enableWater) return;

		if (hiResWaterEnabled)
		{
			hiResWater.SetActive(true);
			normalWater.SetActive(false);
		}
		else
		{
			hiResWater.SetActive(false);
			normalWater.SetActive(true);
		}
	}
}
