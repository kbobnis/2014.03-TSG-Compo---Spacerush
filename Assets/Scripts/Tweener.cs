using UnityEngine;
using System.Collections.Generic;

public class Tweener : MonoBehaviour {

	HashSet<Tween> activeTweens;
	
	public delegate void Handler();
	
	class Tween {
		public Transform transform;
		public Vector3 direction;
		public Vector3 endPoint;
		public float pace;
		public float time;
		public Handler callback;
	}

	// Use this for initialization
	void Start () {
		activeTweens = new HashSet<Tween> ();
	}

	public void AddNewTween(GameObject tweenee, Vector3 endPoint, float time, Handler callback) {
		Transform transform = tweenee.GetComponent<Transform> ();
		float distance = Vector3.Distance (transform.position, endPoint);
		Tween tween = new Tween ();
		tween.transform = transform;
		float pace = distance / time;
		tween.pace = pace;
		tween.direction = (endPoint - transform.position);
		tween.endPoint = endPoint;
		tween.direction.Normalize();
		tween.direction *= pace;
		tween.time = time;
		tween.callback = callback;
		activeTweens.Clear ();
		activeTweens.Add (tween);
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Tween tween in activeTweens) {
			if (Vector3.Distance(tween.transform.position, tween.endPoint) <= 0.1) {
				tween.transform.position = tween.endPoint;
				Debug.Log("Tween ended. End position " + tween.endPoint.ToString() + ", actual position " + tween.transform.position.ToString());
				activeTweens.Remove(tween);
				tween.callback();
				return;
			}
			float distancePossible = tween.pace * Time.deltaTime;
			bool shouldBeRemoved = false;
			Vector3 moveVector;
			tween.time -= Time.deltaTime;
			moveVector = tween.endPoint - tween.transform.position;
			if (Vector3.Distance(tween.transform.position, tween.endPoint) >= distancePossible) {
				moveVector.Normalize();
				moveVector *= distancePossible;
			}
			//Debug.Log(tween.transform.position.ToString() + " " + tween.endPoint.ToString() + " " + moveVector.ToString());
			tween.transform.position += moveVector;
		}
	}
}
