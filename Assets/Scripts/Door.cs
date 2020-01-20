using UnityEngine;
using System.Collections;

public class Door : TileEdge {
	public bool isStrong;
	public bool isOpened;



	public Door(Side side):base(side){
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	public override ActionType GetAction(){
		if (isBroken) {
			return ActionType.REPAIR;
		}	
		return isOpened ? ActionType.CLOSE_DOOR : ActionType.OPEN_DOOR;
	}

}

