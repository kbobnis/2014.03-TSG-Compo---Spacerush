using UnityEngine;
using System.Collections.Generic;

public class MeteoriteSystem : MonoBehaviour {

	public GameObject meteorPrefab;

	class MeteorConfig {
		public int cooldown;
		public float duration;
		public float probability;
		public float fireProbability; //value 0-1
		public float destroyProbability; //value 0-1
	}

	Queue<MeteorConfig> meteorConfigs = new Queue<MeteorConfig>();
	MeteorConfig currentConfig;

	// Use this for initialization
	void Start () {
	
	}

	public void AddConfig(int duration, float probability, int cooldown, float fireProbability, float destroyProbability) {
		MeteorConfig config = new MeteorConfig ();
		config.cooldown = cooldown;
		config.duration = duration;
		config.probability = probability;
		config.fireProbability = fireProbability;
		config.destroyProbability = destroyProbability;
		meteorConfigs.Enqueue (config);
	}

	void MeteorSpawn() {
		if (currentConfig.duration < 0 && meteorConfigs.Count > 0) {
			currentConfig = null;
			return;
		}
		currentConfig.duration -= 1;
		System.Random random = new System.Random ();
		if (random.NextDouble () < currentConfig.probability) {
			GameObject meteor = Instantiate(meteorPrefab) as GameObject;
			GameObject[] tiles = GameObject.FindGameObjectsWithTag("tile");
			int index = new System.Random().Next(0, tiles.Length);
			meteor.GetComponent<Meteor>().SetDestination(tiles[index].GetComponent<Tile>());

			Meteor meteorScript = meteor.GetComponent<Meteor>();
			meteorScript.toStartFire = Random.value < currentConfig.fireProbability;
			meteorScript.toDestroy = Random.value < currentConfig.destroyProbability;

			currentConfig.duration -= currentConfig.cooldown;
			Invoke("MeteorSpawn", currentConfig.cooldown);
			return;
		}
		Invoke ("MeteorSpawn", 1);
	}
	
	// Update is called once per frame
	void Update () {
		if (currentConfig == null && meteorConfigs.Count == 0) {
			return;
		}
		if (currentConfig == null) {
			currentConfig = meteorConfigs.Dequeue();
			Invoke("MeteorSpawn", 1);
		}

	}
}
