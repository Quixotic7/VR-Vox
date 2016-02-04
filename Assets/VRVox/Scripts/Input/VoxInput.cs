using UnityEngine;
using System.Collections;

public static class VoxInput {

	public static string selectButton = "Fire1";
	public static string backButton = "Fire2";
	public static string leftShoulder = "Left Shoulder";
	public static string rightShoulder = "Right Shoulder";
	public static string LeftXAxis = "Left_X_Axis";
	public static string LeftYAxis = "Left_Y_Axis";
	public static string RightXAxis = "Right_X_Axis";
	public static string RightYAxis = "Right_Y_Axis";
	public static string ButtonA = "Button A";
	public static string ButtonB = "Button B";
	public static string ButtonX = "Button X";
	public static string ButtonY = "Button Y";
	public static string Start = "Start";
	public static string Select = "Select";
	public static string MouseX = "Mouse X";
	public static string MouseY = "Mouse Y";
	public static string DPadX = "DPad_X_Axis";
	public static string DPadY = "DPad_Y_Axis";

	public static bool GetSelectDown()
	{
		return Input.GetButtonDown(selectButton);
	}

	public static bool GetSelect()
	{
		return Input.GetButton(selectButton);
	}

	public static bool GetSelectUp()
	{
		return Input.GetButtonUp(selectButton);
	}

	public static bool GetBackDown()
	{
		return Input.GetKeyDown(KeyCode.Escape);
	}

	public static bool GetBack()
	{
		return Input.GetKey(KeyCode.Escape);
	}

	public static bool GetBackUp()
	{
		return Input.GetKeyUp(KeyCode.Escape);
	}

	
}
