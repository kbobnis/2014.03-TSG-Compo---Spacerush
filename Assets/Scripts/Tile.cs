  using UnityEngine;
using System.Collections.Generic;


public class Tile : MonoBehaviour {

	public bool walkable;
	public GameObject system;
	public TileEdge up;
	public TileEdge down;
	public TileEdge left;
	public TileEdge right;

	public Tile upNeighbour;
	public Tile downNeighbour;
	public Tile leftNeighbour;
	public Tile rightNeighbour;

	public float pressure; // value 0-1
	public float o2Level; // value 0-1
	public float pLevel; // value 0-1
	public FloorTypeEnum floorType;
	public bool isVoidGenerator;
	public bool voidScent;
	public GameObject firePrefab;
	public GameObject fire;
	public Material floorMaterial;

	public bool toShowVisibleThings = true;
	public static float reduceO2Factor;
	public static float reducePressureFactor;
	public static float passiveReduceo2Factor;

	// Use this for initialization
	void Start () {
	
	}

	public bool StartFire() {
		if (o2Level < Config.me.MinO2Level || pressure == 0.0f) {
			return false;
		}
		if (o2Level > Config.me.MinO2Level && fire == null) {
			fire = Instantiate(firePrefab) as GameObject;
			fire.AddComponent<InGamePosition>();
			InGamePosition position = fire.GetComponent<InGamePosition>();
			position.column = GetComponent<InGamePosition>().column;
			position.row = GetComponent<InGamePosition>().row;
			position.UpdatePosition();
			fire.GetComponent<Fire>().tile = this;
			return true;
		}
		return false;
	}

	// Update is called once per frame
	void Update () {
		//all tiles passively loose o2
		o2Level -= passiveReduceo2Factor * Time.deltaTime;

		if (voidScent && o2Level > 0) {
			o2Level -= Time.deltaTime * reduceO2Factor;


			pressure -= Time.deltaTime * reducePressureFactor;
			if (pressure < 0) {
				pressure = 0f;
			}
		}
		if (!voidScent && o2Level < 1 && fire == null) {
			GameObject[] oxySystems = GameObject.FindGameObjectsWithTag("oxy");
			foreach (GameObject oxySystem  in oxySystems) {
				if (!oxySystem.GetComponent<TileSystem>().isBroken) {
					o2Level += Time.deltaTime * Config.me.PropagateO2Factor;
					pressure += Time.deltaTime * TileSystem.propagatePressureFactor;
				}
			}


			if (pressure > 1) {
				pressure = 1;
			}

		}
		if (toShowVisibleThings) {
			showVisibleThings();
			toShowVisibleThings = false;
		}
		floorMaterial.color = new Color(1, Mathf.Pow(o2Level, 2), Mathf.Pow(o2Level, 2), 1);
		computeOxyLevel ();

		if (o2Level > 1) {
			o2Level = 1;
		}
		if (o2Level < 0) {
			o2Level = 0f;
		}
	}

	public void showVisibleThings(){
		foreach(Transform transform in gameObject.transform){
			GameObject element = transform.gameObject;
			element.SetActive(false);
			string name = element.name;
			if ((name == "floorInterior" && floorType == FloorTypeEnum.INTERIOR)
			    || (name == "floorExterior" && floorType == FloorTypeEnum.EXTERIOR)){
				element.SetActive(true);
				floorMaterial = element.GetComponent<Renderer>().material;
			}
			
			
			if (
				(up is Door && name == "doorUp"  )
				|| (up is Wall && name == "wallUp" )
				|| (up is Hull && name == "hullUp" )
				|| (down is Door && name == "doorDown") 
				|| (down is Wall && name == "wallDown")
				|| (down is Hull && name == "hullDown")
				|| (left is Door && name == "doorLeft" )
				|| (left is Wall && name == "wallLeft")
				|| (left is Hull && name == "hullLeft") 
				|| (right is Door && name == "doorRight" )
				|| (right is Wall && name == "wallRight")
				|| (right is Hull && name == "hullRight" )
				){
				element.SetActive(true);
			}
		}
	}

	private void computeOxyLevel() {
		int deriver = 1;
		float value = o2Level;
		if (TileEdge.IsPassable(down)  && downNeighbour != null && TileEdge.IsPassable(downNeighbour.up)) {
			deriver ++;
			value += downNeighbour.o2Level;
		}
		if (TileEdge.IsPassable(up) && upNeighbour != null && TileEdge.IsPassable(upNeighbour.down)) {
			deriver ++;
			value += upNeighbour.o2Level;
		}
		if (TileEdge.IsPassable(left)  && leftNeighbour != null && TileEdge.IsPassable(leftNeighbour.right)) {
			deriver ++;
			value += leftNeighbour.o2Level;
		}
		if (TileEdge.IsPassable(right)  && rightNeighbour != null && TileEdge.IsPassable(rightNeighbour.left)) {
			deriver ++;
			value += rightNeighbour.o2Level;
		}
		o2Level = value / deriver;
	}

	public void SetAsVoidGenerator() {
		isVoidGenerator = true;
	}


	public List<TileEdge> GetSides(){
		List<TileEdge> sides = new List<TileEdge> ();
		sides.Add (up);
		sides.Add (right);
		sides.Add (down);
		sides.Add (left);
		return sides;
	}

	public virtual void onMeteor(bool startFire, bool destroy) {
		if (startFire) StartFire ();
		if (destroy)  DoAction (ActionType.DESTROY);
	}


	public virtual ActionType getAction(){
		//mozna zawsze drzwi otworzyc lub zamknacs
		if (fire != null) {
			return ActionType.EXTINGUISH;
		}

		if (system != null) {
			if (system.GetComponent<TileSystem>().isBroken){
				return ActionType.REPAIR;
			}
		}

		foreach( TileEdge side in GetSides()){
			if (side != null && side.GetAction() != ActionType.NONE){
				return side.GetAction();
			}
		}



		return ActionType.NONE;
	}

	private GameObject GetGameObjectForSide(Side side){
		foreach (Transform transform in gameObject.transform) {
			GameObject element = transform.gameObject;
			string name = element.name;
			if (element.activeSelf == false){
				continue;
			}


			switch(name){
				//up
				case "doorUp":
				case "hullUp":
				case "wallUp":
					return element;
				//down
				case "doorDown":
				case "hullDown":
				case "wallDown":
					return element;
				//left 
				case "doorLeft":
				case "hullLeft":
				case "wallLeft":
					return element;
				//right
				case "doorRight":
				case "hullRight":
				case "wallRight":
					return element;
			}
		}
		return null; //czasami 
	}

	public void DoAction(ActionType actionType){
		if (fire != null && actionType == ActionType.EXTINGUISH) {
			GameObject.Destroy(fire);
			fire = null;
			return;
		}
		if (system != null && actionType == ActionType.DESTROY) {
			system.GetComponent<TileSystem>().isBroken = true;
		}

		if (system != null && actionType == ActionType.REPAIR && system.GetComponent<TileSystem>().isBroken) {
			system.GetComponent<TileSystem>().isBroken = false;
		}

		foreach (TileEdge tileEdge in GetSides()) {
			if (tileEdge != null){
				tileEdge.DoAction(actionType, GetGameObjectForSide(tileEdge.side));

				if (upNeighbour != null && upNeighbour.down != null){
					upNeighbour.down.DoAction(actionType, upNeighbour.GetGameObjectForSide(Side.DOWN));
				}
				if (leftNeighbour != null && leftNeighbour.right != null){
					leftNeighbour.right.DoAction(actionType, leftNeighbour.GetGameObjectForSide(Side.LEFT));
				}
				if (rightNeighbour != null && rightNeighbour.left != null){
					rightNeighbour.left.DoAction(actionType, rightNeighbour.GetGameObjectForSide(Side.RIGHT));
				}
				if (downNeighbour != null && downNeighbour.up != null){
					downNeighbour.up.DoAction(actionType, downNeighbour.GetGameObjectForSide(Side.UP));
				}
			}
		}
	}


}
