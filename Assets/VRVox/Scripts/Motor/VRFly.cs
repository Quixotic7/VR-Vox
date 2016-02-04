using UnityEngine;
using System.Collections;
using VRVox;

public class VRFly : MonoBehaviour
{
	public VoxMenu voxMenu;

	protected Vector2 Rot;

	public float RotSpeed = 60f;

	public float MoveSpeed = 30f;

	public float rotSnapAngle = 30;

	public bool enableRotation = false;

	// public OVRCameraRig cameraController = null;

	private float SimulationRate = 60f;

	private Vector2 MoveThrottle;

	private float Yaw;

	protected float lastRightAxisX;
	protected float lastRightAxisY;


	protected float snapThreshold = 0.8f;

	protected void Awake()
	{
		Rot.x = transform.rotation.eulerAngles.y;
		Yaw = transform.rotation.eulerAngles.y;
	}

	protected void LateUpdate()
	{
		if (voxMenu.IsVisible) return;

		var cameraPosition = VoxManager.ViewPosition;
		var cameraForward = VoxManager.ViewForward;
		var cameraRight = VoxManager.ViewRight;
		var cameraUp = VoxManager.ViewUp;

		if (enableRotation)
		{
			Rot = new Vector2(
				Rot.x + Input.GetAxis("Mouse X")*RotSpeed*Time.deltaTime,
				Rot.y + Input.GetAxis("Mouse Y")*RotSpeed*Time.deltaTime);

			transform.localRotation = Quaternion.AngleAxis(Rot.x, Vector3.up);
			transform.localRotation *= Quaternion.AngleAxis(Rot.y, Vector3.left);
		}
		else
		{
			//Yaw += Input.GetAxis(VoxInput.RightXAxis) * RotSpeed * Time.deltaTime;
			//transform.localRotation = Quaternion.AngleAxis(Yaw, Vector3.up);


			//Yaw = transform.localRotation.eulerAngles.y;

			var turnAxis = 0f;

			var do180 = false;

			var rightAxisX = Input.GetAxis(VoxInput.RightXAxis);
			var rightAxisY = Input.GetAxis(VoxInput.RightYAxis);

			if (Input.GetKeyDown(KeyCode.E)) rightAxisX = 1;
			if (Input.GetKeyDown(KeyCode.Q)) rightAxisX = -1;


			if (rightAxisX >= snapThreshold && lastRightAxisX < snapThreshold) turnAxis = 1;
			else if (rightAxisX <= -snapThreshold && lastRightAxisX > -snapThreshold) turnAxis = -1;

			if (Mathf.Abs(rightAxisY) >= 0.95f && Mathf.Abs(lastRightAxisY) < 0.95f) do180 = true;

			if (Input.GetKeyDown(KeyCode.PageDown)) turnAxis = 1;
			if (Input.GetKeyDown(KeyCode.PageUp)) turnAxis = -1;

			lastRightAxisX = rightAxisX;
			lastRightAxisY = rightAxisY;

			if (turnAxis != 0 || do180)
			{
				var rot = transform.localEulerAngles;

				//Yaw += turnAxis * rotSnapAngle;

				if (do180) rot.y += 180;
				else rot.y += turnAxis*rotSnapAngle;

				transform.localEulerAngles = rot;
			}

			//transform.localRotation = Quaternion.AngleAxis(Yaw, Vector3.up);

		}

		var verticalAxis = Input.GetAxis(VoxInput.DPadY);

		if (Input.GetKey(KeyCode.R))
		{
			verticalAxis = 1f;
		}
		if (Input.GetKey(KeyCode.F))
		{
			verticalAxis = -1f;
		}


		var leftAxisX = Input.GetAxis(VoxInput.LeftXAxis) + Input.GetAxis("Horizontal");
		var leftAxisY = Input.GetAxis(VoxInput.LeftYAxis) + Input.GetAxis("Vertical");

		var mouseX = Input.GetAxis(VoxInput.MouseX);
		var mouseY = Input.GetAxis(VoxInput.MouseY);

		transform.position += cameraForward * MoveSpeed * Time.deltaTime * (leftAxisY);
		transform.position += cameraRight * MoveSpeed * Time.deltaTime * (leftAxisX);
		transform.position += Vector3.up * MoveSpeed * Time.deltaTime * verticalAxis;
	}
}
