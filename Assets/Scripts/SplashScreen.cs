using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {
	public GameObject player;
	public GameObject ship;

	GUIStyle blackSplash = new GUIStyle ();
	// Use this for initialization
	void Start () {
		Texture2D background = new Texture2D (1, 1);
		background.SetPixel (0, 0, Color.black);
		background.wrapMode = TextureWrapMode.Repeat;
		background.Apply();
		GUIStyle splash = new GUIStyle ();
		splash.normal.background = background;
	}

	void OnGUI() {
		GUI.backgroundColor = Color.black;
		Vector3 worldPostion = Camera.main.ViewportToScreenPoint (new Vector3 (0.5f, 0.5f));
		GUI.DrawTexture (new Rect (worldPostion.x - 256, worldPostion.y, 512, 64), Resources.Load<Texture>("hud/start/start_button"));
		Vector3 logoPosition = Camera.main.ViewportToScreenPoint (new Vector3 (0.5f, 0.25f));
		GUI.DrawTexture (new Rect (logoPosition.x - 256, logoPosition.y, 512, 128), Resources.Load<Texture>("hud/start/start"));
	}
	
	// Update is called once per frame

	void BeginGame() {
		Instantiate (player);
		Instantiate (ship);
		this.enabled = false;
	}

	void Update () {
		if (Input.anyKey) {
			BeginGame();
		}
	}
}
