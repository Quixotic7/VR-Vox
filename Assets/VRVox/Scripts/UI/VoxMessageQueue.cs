using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VoxMessageQueue : MonoBehaviour
{
	public Text uiText;

	public float showMessageTime = 3f;

	public void DisplayMessge(string message)
	{
		gameObject.SetActive(true);
		uiText.text = message;

		StopAllCoroutines();
		StartCoroutine(DisplayMessages());
	}

	public void HideMessages()
	{
		StopAllCoroutines();
		uiText.text = "";
		gameObject.SetActive(false);
	}

	IEnumerator DisplayMessages()
	{
		yield return new WaitForSeconds(showMessageTime);
		HideMessages();
	}
}
