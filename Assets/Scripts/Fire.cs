using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	public Tile tile;
	public float level = 0f;

	// Use this for initialization
	void Start () {
		
	}

	void Propagate() {
		int firesStarted = 0;
		int contagiousness = Config.me.FireContagiousness; 

		if (tile.system != null) {
			tile.system.GetComponent<TileSystem>().isBroken = true;
		}

		if (firesStarted < contagiousness && tile.downNeighbour != null && (TileEdge.IsPassable(tile.down)) && TileEdge.IsPassable(tile.downNeighbour.up))  {
			if (tile.downNeighbour.StartFire()){
				firesStarted ++;
			}

		}
		if (firesStarted < contagiousness && tile.upNeighbour != null && TileEdge.IsPassable(tile.up) && TileEdge.IsPassable(tile.upNeighbour.down)) {
			if (tile.upNeighbour.StartFire()){
				firesStarted ++;
			}
		}
		if (firesStarted < contagiousness && tile.leftNeighbour != null && TileEdge.IsPassable(tile.left) && TileEdge.IsPassable(tile.leftNeighbour.right)) {
			if (tile.leftNeighbour.StartFire()){
				firesStarted ++;
			}
		}
		if (firesStarted < contagiousness && tile.rightNeighbour != null && TileEdge.IsPassable(tile.right) && TileEdge.IsPassable(tile.rightNeighbour.left)) {
			if (tile.rightNeighbour.StartFire()){
				firesStarted ++;
			}
		}
		level = 0;
	}

	void Extinguish() {
		tile.fire = null;
		GameObject.Destroy (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		float minO2Level = Config.me.MinO2Level;
		float propagateFactor = Config.me.PropagateFireFactor;
		float suffocateFactor = Config.me.FireSuffocateFactor; 
		float reduceO2 = Config.me.ReduceO2;
		if (tile.o2Level > minO2Level) {
			tile.o2Level -= reduceO2 * Time.deltaTime;
			level += Time.deltaTime * propagateFactor ;
		}
		else {
			level -= Time.deltaTime * suffocateFactor;
		}
		if (level > 10) {
			Propagate ();
			return;
		} else if (level < -2) {
			Extinguish();
			return;
		}

	}
}