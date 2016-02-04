using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VoxVignette : MonoBehaviour
{
	public Image uiImage;

	public GameObject canvasGO;

	public float fadeInTime = 0.1f;

	public float fadeOutTime = 1f;

	public bool vignetteVisible = true;

	protected bool fading;

	public void ToggleVignette(bool state)
	{
		vignetteVisible = state;
		StopAllCoroutines();
		StartCoroutine(FadeVignette(state));
	}

	IEnumerator FadeVignette(bool enable)
	{
		fading = true;
		var c = uiImage.color;
		var startAlpha = c.a;

		var targetAlpha = enable ? 1 : 0;

		var position = 0f;

		canvasGO.SetActive(true);

		var fadeTime = enable ? fadeInTime : fadeOutTime;

		while (position < 1)
		{
			c.a = Mathf.Lerp(startAlpha, targetAlpha, position);

			uiImage.color = c;

			position += Time.deltaTime/fadeTime;
			yield return new WaitForEndOfFrame();
		}

		c.a = targetAlpha;
		uiImage.color = c;

		if(!enable)
		{
			canvasGO.SetActive(false);
		}
		fading = false;
	}

	//protected void Update()
	//{
	//	if (Input.GetKeyDown(KeyCode.F))
	//	{
	//		ToggleVignette(!vignetteVisible);
	//	}
	//}
}
