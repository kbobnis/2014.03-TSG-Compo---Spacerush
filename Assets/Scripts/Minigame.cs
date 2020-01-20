using UnityEngine;
using System.Collections;

public class Minigame : MonoBehaviour {

	public float actionProgress = 0; // float 0-1
	public static float actionSpeed;

	private bool actionDone;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		actionSpeed = Config.me.ActionSpeed;
		InGamePosition pos = GetComponent<InGamePosition>() as InGamePosition;
		Tile tile = GameObject.FindGameObjectWithTag ("ship").GetComponent<Ship> ().GetTileOnPosition (pos);
		ActionType actionType = tile.getAction ();
		Hud.ShowActionRequest (actionType, actionProgress);

		if (actionProgress >= 1) {
			Ship ship = GameObject.FindGameObjectWithTag ("ship").GetComponent<Ship>();
			tile.DoAction(actionType);
			actionProgress = 0;
			actionDone = true;
		}

	}

	public void PlayerIsDoingAction(bool doingIt){
		if (doingIt) {
			if (!actionDone){
				gameObject.GetComponent<Controls>().state = 1;
				actionProgress += Time.deltaTime * actionSpeed;
				actionProgress = actionProgress>=1?1:actionProgress;
			}
		} else {
			actionProgress = 0;
			actionDone = false;
			gameObject.GetComponent<Controls>().state = 5;
		}
	}
}
