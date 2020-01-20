using UnityEngine;
using System.Collections;

public class TileEdge {

	public bool isBroken;

	public Side side;
	public TileEdge otherSideOfEdge;

	public TileEdge(Side side){
		this.side = side;
	}

	public static Side oppositeSide(Side side){
		switch (side) {
			case Side.DOWN: return Side.UP;
			case Side.UP: return Side.DOWN;
			case Side.LEFT: return Side.RIGHT;
			case Side.RIGHT: return Side.LEFT;
		}
		return Side.UP; //bo nie moge zwrocic null
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public static bool IsPassable(TileEdge edge){
		if(edge == null)
			return true;

		if (edge.isBroken) {
			return true;
		}

		if(edge is Door && (edge as Door).isOpened){
			return true;
		}

		return false;
	}

	public virtual ActionType GetAction(){
		return isBroken ? ActionType.REPAIR : ActionType.NONE;
	}

	public virtual void DoAction(ActionType actionType, GameObject gameObject){
		if (gameObject == null) {
			return;
		}

		switch (actionType) {
			case ActionType.DESTROY:{
				if (gameObject.tag == "hull" /* nie niszczymy scian, bo nie ma animacji || gameObject.tag == "wall" */){
					gameObject.GetComponent<Animator>().SetTrigger("destroy");
					isBroken = true;
				}
				
				break;
			}
			case ActionType.REPAIR:{
   				if (gameObject.tag == "hull" || gameObject.tag == "wall"){
					gameObject.GetComponent<Animator>().SetTrigger("repair");
				}
				isBroken = false;
				break;
			}
			case ActionType.OPEN_DOOR:{
				if (gameObject.tag == "door"){
					(this as Door).isOpened = true;
					gameObject.GetComponent<Animator> ().SetTrigger ("toOpen");
				}
				break;
			}
			case ActionType.CLOSE_DOOR:{
				if (gameObject.tag == "door"){
					gameObject.GetComponent<Animator> ().SetTrigger ("toClose");
					(this as Door).isOpened = false;
				}
				break;	
			}
		}
	}

}
