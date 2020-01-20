using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Controls : MonoBehaviour {
	public string player = "1";
	private double lastChangeTime;
	Tweener tweener;
	int _state = 5;

	public int state {
		set {
			if (_state == 0) {
				return;
			}
			//int baseRotation = 2;
			float sqrt_two_div_two = Mathf.Sqrt (2.0f)/ 2.0f;
			if (value == 2) {
				gameObject.GetComponent<Animator>().transform.rotation = new Quaternion(0, 0, 0, 1);
			}
			if (value == 8) {
				gameObject.GetComponent<Animator>().transform.rotation = new Quaternion(0,sqrt_two_div_two , 0, sqrt_two_div_two);
			}
			if (value == 6) {
				gameObject.GetComponent<Animator>().transform.rotation = new Quaternion(0, 1, 0, 0);
			}
			if (value == 4) {
				gameObject.GetComponent<Animator>().transform.rotation = new Quaternion(0, -sqrt_two_div_two, 0, sqrt_two_div_two);
			}
			_state = value;
			gameObject.GetComponent<Animator>().SetInteger("state", value);
				}
		get {
			return _state;
				}
	}

	// Use this for initialization
	void Start () {
		InGamePosition pos = gameObject.GetComponent<InGamePosition> ();
		pos.UpdatePosition ();
		tweener = GameObject.FindGameObjectWithTag ("tweener").GetComponent<Tweener> ();
		state = 5;
	}
	
	// Update is called once per frame
	void Update () {
		lastChangeTime += Time.deltaTime;
		bool use = MoonzInput.GetKeyDown (MoonzInput.USE, player);

		GetComponent<Minigame> ().PlayerIsDoingAction (use);

		//if (state != 5 || lastChangeTime < 0.1) //nie chcemy za czesto zmieniac, bo get axis bedzie przychodzic co kazdego update
		//	return;

		lastChangeTime = 0;

		bool up = false;
		bool down = false;
		bool left = false;
		bool right = false;

		left = (int)MoonzInput.GetAxis("H", player)==-1;
		right = (int)MoonzInput.GetAxis("H", player)==1;
		if(!left && !right){
			up = (int)MoonzInput.GetAxis("V", player)==1;
			down = (int)MoonzInput.GetAxis("V", player)==-1;
		}

		if (GetComponent<Health> ().health <= 0) {
			return ;
		}

		if (MoonzInput.GetKeyDown (MoonzInput.ARROW_UP, player)){
			up = true;
		}
		if (MoonzInput.GetKeyDown (MoonzInput.ARROW_DOWN, player)) {
			down = true;
		}
		if (MoonzInput.GetKeyDown (MoonzInput.ARROW_LEFT, player)) {
			left = true;
		}
		if (MoonzInput.GetKeyDown (MoonzInput.ARROW_RIGHT, player)) {
			right = true;
		}

		InGamePosition pos = gameObject.GetComponent<InGamePosition> ();

		if(checkIfCanMove(pos, up, down, left, right) ){
			pos.row += left?1:0;
			pos.row += right?-1:0;
			pos.column += up?1:0;
			pos.column += down?-1:0;
			if (left) {
				state = 2;
			}
			if (right) {
				state = 6;
			}
			if (up) {
				state = 8;
			}
			if (down) {
				state = 4;
			}
			Tweener.Handler handler = MakeIdle;
			if (pos != null){
				Debug.Log(pos.row.ToString() + " " + pos.column.ToString());
				tweener.AddNewTween(gameObject, new Vector3(pos.column, 1, pos.row), 0.2f, handler);
			}
		}

	}

	void MakeIdle() {
		state = 5;
		lastChangeTime = DateTime.Now.Ticks;
	}

	Boolean checkIfCanMove(InGamePosition pos, bool up, bool down, bool left, bool right){
		Ship ship = GameObject.FindGameObjectWithTag ("ship").GetComponent<Ship>();
		Tile tile = ship.GetTileOnPosition(pos);

		if (left && 
		    TileEdge.IsPassable (tile.down) && 
		    tile.downNeighbour != null && 
		    TileEdge.IsPassable (tile.downNeighbour.up)) 
			return true;

		if (right && 
		    TileEdge.IsPassable (tile.up) && 
		    tile.upNeighbour != null && 
		    TileEdge.IsPassable (tile.upNeighbour.down)) 
			return true;

		if (down && 
		    TileEdge.IsPassable (tile.left) && 
		    tile.leftNeighbour != null && 
		    TileEdge.IsPassable (tile.leftNeighbour.right)) 
			return true;

		if (up && 
		    TileEdge.IsPassable (tile.right) && 
		    tile.rightNeighbour != null && 
		    TileEdge.IsPassable (tile.rightNeighbour.left)) 
			return true;

		return false;
	}


}
