using UnityEngine;

public class MoonzInput
{
	public const string ARROW_LEFT = "arrow_left";
	public const string ARROW_RIGHT = "arrow_right";
	public const string ARROW_UP = "arrow_up";
	public const string ARROW_DOWN = "arrow_down";
	public const string A = "a";
	public const string B = "b";
	public const string X = "c";
	public const string Y = "d";
	public const string RB = "RB";
	public const string START = "start";

	public const string USE = "Use";
	
	public const string DEFAULT_MODE = "default";
	public const string INPUT_MANAGER_MODE= "input_manager";
	
	public static string mode = MoonzInput.INPUT_MANAGER_MODE;
	
	public static bool GetKeyDown(string keyCode, string inputSuffix) {
		if (keyCode == MoonzInput.USE) return Input.GetButton(MoonzInput.USE + inputSuffix) || Input.GetKey("space");
		if (keyCode == MoonzInput.ARROW_LEFT) return Input.GetKeyDown("a");
		if (keyCode == MoonzInput.ARROW_RIGHT) return Input.GetKeyDown("d");
		if (keyCode == MoonzInput.ARROW_UP) return Input.GetKeyDown("w");
		if (keyCode == MoonzInput.ARROW_DOWN) return Input.GetKeyDown("s");
		return false;
	}
	
	public static float GetAxis(string axis, string inputSuffix) {
		if (mode == DEFAULT_MODE) {
			switch (axis) {
			case "V": return Input.GetAxis("V" + inputSuffix);
			case "H": return Input.GetAxis("H" + inputSuffix);
			case "FV": return Input.GetAxis("FV" + inputSuffix);
			case "FH": return Input.GetAxis("FH" + inputSuffix);
			}
		} else if (mode == INPUT_MANAGER_MODE) {
			switch (axis) {
			case "V": return Input.GetAxis("Vertical" + inputSuffix);
			case "H": return Input.GetAxis("Horizontal" + inputSuffix);
			}
		} else {
			if (axis == "FV") return Input.GetAxis("MacFV");
			if (axis == "FH") return Input.GetAxis("MacFH");
			return Input.GetAxis(axis+"1");
		}
		return 0;
	}
	
}

