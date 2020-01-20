using UnityEngine;
using System.Collections;

public class Meteor : MonoBehaviour {
	public Vector3 forceVector;
	public Vector3 endPoint;
	public GameObject shakerPrefab;
	private bool atTarget = false;
	private Tile target;
	public bool toStartFire;
	public bool toDestroy;

	// Use this for initialization
	void Start () {
	
	}

	public void SetDestination(Tile destination) {
		target = destination;
		endPoint = destination.gameObject.transform.position;
		forceVector = endPoint - gameObject.GetComponent<Transform> ().position;
		forceVector.Normalize ();
		forceVector = forceVector * 90;
	}

	// Update is called once per frame
	void Update () {
		if (atTarget) {
			target.onMeteor(toStartFire, toDestroy);
			GameObject shaker = Instantiate(shakerPrefab) as GameObject;
			shaker.GetComponent<Shaker>().shakee = Camera.main;
			shaker.GetComponent<Shaker>().shake_intensity = .095f;
			shaker.GetComponent<Shaker>().shake_decay = .012f;
			GameObject.Destroy(gameObject);
			return;
		}	
		float distanceToEnd = Vector2.Distance (endPoint, this.gameObject.GetComponent<Transform> ().position);
		if (Vector2.Distance (endPoint, gameObject.transform.position) < forceVector.magnitude) {
			forceVector.Normalize();
			forceVector *= distanceToEnd;
		}

		this.gameObject.transform.position += forceVector * Time.deltaTime;
		if (Vector2.Distance (gameObject.transform.position, endPoint) < 1) {
			this.gameObject.transform.position = endPoint;
			atTarget = true;
		}
	}
}
