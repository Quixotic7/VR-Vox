using UnityEngine;
using System.Collections;
using VRVox;


namespace VRVox
{
	[ExecuteInEditMode]
	public class VoxButtonMakeRadial : MonoBehaviour
	{
		public Transform[] buttons;

		public float radius = 1f;

		public float myAngle;

		public float angleStep = 10;

	
		private void Start()
		{

		}

		// Update is called once per frame
		private void Update()
		{
			var buttonCount = buttons.Length;

			//var step = Mathf.PI*2/buttonCount;

			var step = -Mathf.Deg2Rad*angleStep;

			for (var i = 0; i < buttons.Length; i++)
			{
				var angle = step*i+Mathf.PI*0.5f;

				var x = radius*Mathf.Cos(angle);
				var y = radius*Mathf.Sin(angle);

				buttons[i].transform.localPosition = new Vector3(x, y, 0);
			}
		}
	}
}