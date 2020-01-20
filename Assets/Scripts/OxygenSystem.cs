using UnityEngine;
using System.Collections;

public class OxygenSystem : TileSystem {

	// Use this for initialization
	void Start () {
		InvokeRepeating("FillWithOxy", 1.0f, 1.0f);
	}

	void FillWithOxy() {
    }

	// Update is called once per frame
	void Update () {
	}
}
