using UnityEngine;

public class InputHandler
{
	public static float GetAxis(string axis, string player) {
		switch(axis){
			case "H" : return Input.GetAxis("HorizontalMoveAxis" + player);
			case "V" : return Input.GetAxis("VerticalMoveAxis" + player);
		}
		return 0;
	}

	public static bool GetActionButton(string player){
		return Input.GetKey ("joystick 1 button 0");
	}

	
}

