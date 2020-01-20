using UnityEngine;
using System.Collections;

public class InGamePosition : MonoBehaviour {

	public int column;
	public int row;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdatePosition(){
		gameObject.GetComponent<Transform> ().position = new Vector3(column * 1, 1, row * 1);
	}


}
