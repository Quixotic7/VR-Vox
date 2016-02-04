using UnityEngine;
using System.Collections;
using VRVox;

/// <summary>
/// Used to change the quality of the game
/// </summary>
public class VoxQuality : MonoBehaviour
{

	public string defaultLevelName = "RiftAndroid";
	public string highResLevelName = "RiftAndroidHigh";

	public float textureScaleLow = 0.6f;
	public float textureScaleHigh = 1f;

	protected int defaultLevel;
	protected int highResLevel;

	protected bool highRes = false;

	public VoxMenuButton toggleQualityButton;
	public VoxMenuButton toggleTextureScaleButton;
	public VoxMenuButton toggleAAButton;

	// public OVRManager ovrManager;

	protected bool AAEnabled = true;

	protected void Start()
	{
		var names = QualitySettings.names;

		for (var i = 0; i < names.Length; i++)
		{
			if (names[i] == defaultLevelName) defaultLevel = i;
			if (names[i] == highResLevelName) highResLevel = i;
		}

		QualitySettings.SetQualityLevel(defaultLevel);


		toggleQualityButton.SubscribeToButton(OnToggleQuality);
		toggleTextureScaleButton.SubscribeToButton(OnToggleTextureScale);
		toggleAAButton.SubscribeToButton(OnToggleAA);
	}

	protected void OnToggleQuality(VoxMenuButton button, bool state)
	{
		if (!state) return;

		highRes = button.isSelected;

		var level = highRes ? highResLevel : defaultLevel;

		QualitySettings.SetQualityLevel(level);

		//OVRManager.instance.nativeTextureScale = highRes ? 1 : textureScaleHigh;

	}

	protected void OnToggleTextureScale(VoxMenuButton button, bool state)
	{
		if (!state) return;

		//var prevScale = ovrManager.nativeTextureScale;

		if (button.isSelected)
		{
			//OVRManager.instance.nativeTextureScale = textureScaleLow;
			//OVRManager.NativeTextureScaleModified(prevScale, textureScaleLow);
		}
		else
		{
			//OVRManager.instance.nativeTextureScale = textureScaleHigh;
		}

		//OVRManager.ForceNativeTextureScaleModified();

		//ovrManager.gameObject.SetActive(false);
		//ovrManager.gameObject.SetActive(true);

		//ovrManager.gameObject.SetActive(false);
		//ovrManager.gameObject.SetActive(true);
	}

	protected void OnToggleAA(VoxMenuButton button, bool state)
	{
		if (!state) return;

		AAEnabled = !button.isSelected;

		if (AAEnabled)
		{
			// OVRManager.instance.eyeTextureAntiAliasing = OVRManager.RenderTextureAntiAliasing._2;
		}
		else
		{
			// OVRManager.instance.eyeTextureAntiAliasing = OVRManager.RenderTextureAntiAliasing._1;
		}

		//ovrManager.gameObject.SetActive(false);
		//ovrManager.gameObject.SetActive(true);

	}

	//protected void Update()
	//{
	//	if (Input.GetKeyDown(KeyCode.Z))
	//	{
	//		highRes = !highRes;

	//		var level = highRes ? highResLevel : defaultLevel;

	//		QualitySettings.SetQualityLevel(level);
	//	}
	//}
}
