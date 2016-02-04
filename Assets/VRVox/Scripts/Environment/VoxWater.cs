using UnityEngine;
using System.Collections;

public class VoxWater : MonoBehaviour
{

	protected Vector3 initPosition;

	// Use this for initialization
	void Start ()
	{

		initPosition = transform.position;

	}
	
	// Update is called once per frame
	void LateUpdate()
	{
		var newPos = VoxManager.ViewPosition;

		newPos.y = initPosition.y;

		transform.position = newPos;
	}
}
