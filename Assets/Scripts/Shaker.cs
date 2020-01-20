using UnityEngine;
using System.Collections;

public class Shaker : MonoBehaviour {
	public Camera shakee;
	public float shake_intensity;
	public float shake_decay;

	private Vector3 originPosition;
	private Quaternion originRotation;
	// Use this for initialization
	void Start () {
		originPosition = shakee.transform.position;
		originRotation = shakee.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if(shake_intensity > 0){
			shakee.transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
			shakee.transform.rotation =  new Quaternion(
				originRotation.x + Random.Range(-shake_intensity,shake_intensity)*.2f,
				originRotation.y + Random.Range(-shake_intensity,shake_intensity)*.2f,
				originRotation.z + Random.Range(-shake_intensity,shake_intensity)*.2f,
				originRotation.w + Random.Range(-shake_intensity,shake_intensity)*.2f);
			shake_intensity -= shake_decay;
			return;
		}
		GameObject.Destroy (gameObject);
	}
}
