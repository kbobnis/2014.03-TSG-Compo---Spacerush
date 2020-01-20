using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Config config = Config.me;

		Tile myTile = Ship.me.GetTileForGM (gameObject);
		if (myTile.system != null && myTile.system.tag == "medic" && !myTile.system.GetComponent<TileSystem>().isBroken) {
			double health = gameObject.GetComponent<Health>().health;
			health += Time.deltaTime * config.HealPercentage;
			if (health > 1) {
				health = 1;
			}
			gameObject.GetComponent<Health> ().health = health;
		}
		if (myTile.o2Level <= config.MinO2ToThrive){
			Suffocate();
		}
		if (gameObject.GetComponent<Health> ().health <= 0) {
			gameObject.GetComponent<Controls> ().state = 0;
		}
		GameObject fire = myTile.fire;
		if (fire != null && fire.GetComponent<Fire>().level > 0) {
			Burn(fire.GetComponent<Fire>().level);
		}
	}

	private void Burn(float fireLevel){
		Config config = Config.me;
		double health = gameObject.GetComponent<Health>().health;
		health -= Time.deltaTime * config.FireDamageFactor;
		if (health < 0){
			health = 0;
		}
		gameObject.GetComponent<Health> ().health = health;
	}

	private void Suffocate(){
		Config config = Config.me;
		double health = gameObject.GetComponent<Health>().health;
		health -= Time.deltaTime * config.SuffocateFactor;
		if (health < 0){
			health = 0;
		}
		gameObject.GetComponent<Health> ().health = health;
	}
}
