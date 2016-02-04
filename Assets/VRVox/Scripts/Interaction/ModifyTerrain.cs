using UnityEngine;
using System.Collections;
using VRVox;

public class ModifyTerrain : MonoBehaviour {

	protected Vector2 Rot;

	public float RotSpeed = 3f;

	public float MoveSpeed = 3f;

	protected void Update()
	{
		if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Z))
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
			{
				//VoxTerrain.SetBlock(hit, new BlockEmpty());

				VoxTerrain.SetBlock(hit, VoxBlockType.Default, Color.blue);

			}
		}
		if (Input.GetMouseButton(1) || Input.GetKeyDown(KeyCode.X))
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
			{
				VoxTerrain.SetBlock(hit, VoxBlockType.Default, Color.green, true);
			}
		}

		Rot = new Vector2(
			Rot.x + Input.GetAxis("Mouse X")*RotSpeed*Time.deltaTime,
			Rot.y + Input.GetAxis("Mouse Y")*RotSpeed*Time.deltaTime);

		transform.localRotation = Quaternion.AngleAxis(Rot.x, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(Rot.y, Vector3.left);

		transform.position += transform.forward * MoveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
		transform.position += transform.right * MoveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
	}
}
