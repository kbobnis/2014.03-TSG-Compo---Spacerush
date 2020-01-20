using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Hud : MonoBehaviour {

	private List<MyKeyValue> blinking = new List<MyKeyValue>();

	private GUIStyle smallFont = new GUIStyle();
	private GUIStyle smallerFont = new GUIStyle();
	private GUIStyle bigFont = new GUIStyle();
	private GUIStyle bigFontLeft = new GUIStyle();

	public static ActionType actionType;
	public static float actionProgress;


	// Use this for initialization
	void Start () {
		smallFont.fontSize = 22 * Screen.width / 1900;
		smallFont.font = (Font)Resources.Load ("font/Dosis-Bold");
		smallFont.normal.textColor = new Color (9/255f, 231/255f, 117/255f);

		smallerFont.fontSize = 18 * Screen.width / 1900;
		smallerFont.font = (Font)Resources.Load ("font/Dosis-Bold");
		smallerFont.normal.textColor = new Color (9/255f, 231/255f, 117/255f);

		bigFont.fontSize = 50 * Screen.width / 1900;
		bigFont.font = (Font)Resources.Load ("font/Dosis-Bold");
		bigFont.normal.textColor = new Color (248/255f, 6/255f, 18/255f);
		bigFont.alignment = TextAnchor.UpperRight;

		bigFontLeft.fontSize = 25 * Screen.width / 1900;
		bigFontLeft.font = (Font)Resources.Load ("font/Dosis-Bold");
		bigFontLeft.normal.textColor = new Color (255/255f, 255/255f, 255/255f);
	}

	public static void ShowActionRequest (ActionType actionType, float actionProgress){
		Hud.actionType = actionType;
		Hud.actionProgress = actionProgress;
	}

	// Update is called once per frame
	void Update () {

		foreach(MyKeyValue one in blinking) {
			one.Value += Time.deltaTime;
		}

	}

	void OnGUI(){

		//oxygen bar
		int allTilesInShip = Ship.tilesInShip.Count;
		float oxygenatedTiles = 0;
		float fireValue = 0;

		foreach (Tile tile in Ship.tilesInShip) {
			oxygenatedTiles += tile.o2Level ;
			if (tile.fire != null){
				fireValue += (tile.fire.GetComponent<Fire>().level + 2)/12;
			}
		}
		float oxygenPercent = oxygenatedTiles / allTilesInShip;
		//float firePercent = fireValue / allTilesInShip;

		int hullBreaches = 0;
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag ("tile")) {

			foreach(TileEdge edge in gameObject.GetComponent<Tile>().GetSides()){
				if (edge != null && edge.isBroken && edge is Hull){
					hullBreaches ++;
				}
			}
		}

		DrawElement ("hud/game/disaster_oxygen_bar_bg", 0.006, 0.008, 0.293, 0.074 );
		DrawElement ("hud/game/disaster_oxygen_bar", 0.063, 0.031, 0.228,  0.041, oxygenPercent * 0.228,  0.041 );


		//oxygen alert
		if (oxygenPercent < 0.7) {
			DrawElementBlink ("hud/game/disaster_oxygen_alert", 0.304, 0.029, 0.127, 0.058);
			DrawElementBlink ("hud/game/disaster_oxygen_alert_text", 0.436, 0.039, 0.104, 0.041);
		}

		//player bar 
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag ("Player")) {
			double health = gameObject.GetComponent<Health>().health;
			DrawElement ("hud/game/player_bar_bg", 0.006, 0.072, 0.263, 0.068 );
			DrawElement ("hud/game/player_bar", 0.010, 0.106, 0.203 * health, 0.019 );

			DrawText (""+Mathf.RoundToInt((float)(health*100))+"/100", smallFont, 0.225, 0.103);

			//player alert
			if (health <= 0  ){
				DrawElementBlink ("hud/game/player_alert", 0.267, 0.099, 0.134, 0.041 );
			} 
			else if (health <= 0.2){
				DrawElement ("hud/game/player_alert_text", 0.269, 0.103, 0.124, 0.032 );
			} 
		}

		//temperature
		double overheat = Ship.me.overheat;

		DrawElement ("hud/game/disaster_temperature_bar_bg", 0.006, 0.614, 0.074, 0.373 );

		//DrawElement ("hud/game/disaster_temperature_bar", 0.016, 0.673, 0.029, 0.310, 0.029 , 0.310 * fireValue / allTilesInShip);
		DrawElement ("hud/game/disaster_temperature_bar", 0.016, 0.673, 0.029, 0.310, 0.029 , 0.310 * overheat, true);
		if (overheat > 0.2) {
			DrawElement ("hud/game/disaster_temperature_alert", 0.006, 0.516, 0.127, 0.092);
		}
		if (overheat > 0.4){
			DrawElementBlink ("hud/game/disaster_temperature_alert_text", 0.138, 0.535, 0.225, 0.054);
		}

		//nie chcemy tego pokazywac, bo karolina powiedziała, ze to jest brzydkie
		//float overheatFactorFire = Ship.me.overheatProgress;
		//DrawElement ("hud/game/system_bar_bg", 0.006, 0.6, 0.05, 0.015);
		//DrawElement ("hud/game/system_bar", 0.006, 0.6, 0.05 * overheatFactorFire, 0.015);


		//score
		DrawElement ("hud/game/score", 0.710, 0.010, 0.284, 0.092 );
		DrawText (Mathf.RoundToInt((float)Ship.score).ToString() + " sec ", bigFont, 0.88, 0.020, 0.1);

		//fires on board
		int fires = Mathf.RoundToInt ((float)fireValue);
		DrawText (fires.ToString(), smallerFont, 0.934, 0.115);
		if (fires > 4) {
			DrawElementBlink ("hud/game/disaster_alert", 0.899, 0.140, 0.056, 0.041);
		}

		//hull breaaches
		DrawText (hullBreaches.ToString (), smallerFont, 0.934, 0.193);
		if (hullBreaches > 0) {
			DrawElementBlink ("hud/game/disaster_alert", 0.899, 0.218, 0.056, 0.041);
		}


		DrawElement ("hud/game/disaster_counters", 0.918, 0.108, 0.076, 0.149 );


		//reactors
		int counter = 0;
		foreach (GameObject cooling in GameObject.FindGameObjectsWithTag("cooler")) {

			double y = 0.9 - counter++ * 0.095;
			double offsetBg = 0.008;
			double offsetBarGreen = 0.023;
			double offsetBarRed = 0.023;
			double offsetAlert = 0.032;
			double offsetAlertReactorText = 0.045;

			DrawElement ("hud/game/disaster_reactor_bg", 0.084, y + offsetBg, 0.054, 0.078);
			DrawText(counter.ToString(), smallerFont, 0.09, y + offsetBg + 0.002);
			DrawElement ("hud/game/disaster_reactor_bar_green", 0.130, y + offsetBarGreen, 0.006, 0.059);

			if (cooling.GetComponent<TileSystem>().isBroken){
				DrawElement ("hud/game/disaster_reactor_bar_red", 0.130, y + offsetBarRed, 0.006, 0.059);
				DrawElement ("hud/game/disaster_alert", 0.138, y + offsetAlert, 0.056, 0.041);
				DrawElementBlink ("hud/game/disaster_alert_reactor_text", 0.197, y + offsetAlertReactorText, 0.111, 0.016);
			}
		}

		//o2 systems
		counter = 0;
		foreach (GameObject o2 in GameObject.FindGameObjectsWithTag("oxy")) {

			double y = 0.140 + counter++ * 0.095;
			double offsetBg = 0.0;
			double offsetBarGreen = 0.016;
			double offsetBarRed = 0.015;
			double offsetAlert = 0.032;
			double offsetAlertReactorText = 0.045;

			DrawElement ("hud/game/disaster_o2_bg", 0.006, y + offsetBg, 0.054, 0.078);
			DrawText(counter.ToString(), smallerFont, 0.012, y + offsetBg + 0.004);
			DrawElement ("hud/game/disaster_reactor_bar_green", 0.051, y + offsetBarGreen, 0.006, 0.059);


			if (o2.GetComponent<TileSystem>().isBroken){
				DrawElement ("hud/game/disaster_reactor_bar_red", 0.0512, y + offsetBarRed, 0.006, 0.059);
				DrawElement ("hud/game/disaster_alert", 0.06, y + offsetAlert, 0.056, 0.041);
				DrawElementBlink ("hud/game/disaster_alert_o2_text", 0.119, y + offsetAlertReactorText, 0.113, 0.016);
			}
		}

		//medic systems
		counter = 0;
		foreach (GameObject medic in GameObject.FindGameObjectsWithTag("medic")) {

			double x = 0.954;
			double y = 0.263 + counter++ * 0.095;
			double offsetBg = 0.0;
			double offsetBarGreen = 0.010;
			double offsetBarRed = 0.010;

			double offsetYReactorBar = -0.006;
			
			DrawElement ("hud/game/medic", x , y + offsetBg, 0.04, 0.078);
			//DrawText(counter.ToString(), smallerFont, x - 0.01, y + offsetBg + 0.004);
			DrawElement ("hud/game/disaster_reactor_bar_green", x + offsetYReactorBar, y + offsetBarGreen, 0.006, 0.059);
			
			
			if (medic.GetComponent<TileSystem>().isBroken){
				DrawElement ("hud/game/disaster_reactor_bar_red", x + offsetYReactorBar, y + offsetBarRed, 0.006, 0.059);
				//DrawElement ("hud/game/disaster_alert", x, y + offsetAlert, 0.056, 0.041);
				//DrawElementBlink ("hud/game/disaster_alert_o2_text", x + 0.059, y + offsetAlertReactorText, 0.113, 0.016);
			}
		}

		Vector3 s = Camera.main.WorldToScreenPoint (GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().position);



		string text = "";
		switch (Hud.actionType) {
			case ActionType.OPEN_DOOR: text =  "Open door" ; break;
			case ActionType.CLOSE_DOOR: text =  "Close door" ; break;
			case ActionType.REPAIR: text =  "Repair" ;  break;
			case ActionType.EXTINGUISH: text =   "Extinguish fire " ; break;
		}
		if (text != "") {
			GUI.Label (new Rect (s.x, Screen.height - s.y, Screen.width, Screen.height), text, bigFontLeft);
			GUI.DrawTexture (new Rect (s.x, Screen.height - s.y + Screen.height * 0.03f, Screen.width * 0.05f, Screen.height * 0.015f), Resources.Load ("hud/game/system_bar_bg", typeof(Texture))as Texture);
			GUI.DrawTexture (new Rect (s.x, Screen.height - s.y + Screen.height * 0.03f, Screen.width * 0.05f * Hud.actionProgress, Screen.height * 0.015f), Resources.Load ("hud/game/system_bar", typeof(Texture))as Texture);
		}

		//ShowPressure();
	}
	
	private void ShowPressure(){
		DrawPressureBarOnUi();

		float scaleY = 0.06f;
		float scaleX = 0.04f;
		float widthScale = 0.01f;
		List<Room> rooms;
		rooms = Ship.me.GetRooms();
		foreach(Room room in rooms){
			Vector3 pos = Camera.main.WorldToScreenPoint(room.center);
			GUI.DrawTexture (new Rect (pos.x, Screen.height - pos.y + Screen.height * scaleY, Screen.width * scaleX, Screen.height * widthScale), Resources.Load ("hud/game/system_bar_bg", typeof(Texture))as Texture);
			GUI.DrawTexture (new Rect (pos.x, Screen.height - pos.y + Screen.height * scaleY, Screen.width * scaleX * room.pressure, Screen.height * widthScale), Resources.Load ("hud/game/system_bar", typeof(Texture))as Texture);
		}
	}


	private void DrawPressureBarOnUi(){
		List<Room> rooms = Ship.me.GetRooms();
		int counter = 0;
		float totalPressure = 0.0f;
		foreach(Room room in rooms){
			room.countPressure();
			totalPressure += room.pressure;
			counter++;
		}
		totalPressure = totalPressure / (float)counter;

		DrawElement ("hud/game/disaster_pressure_bar_bg", 0.920, 0.614, 0.074, 0.373 );
		DrawElement ("hud/game/disaster_pressure_bar", 0.956, 0.673, 0.029, 0.310, -1,  0.31*totalPressure, true);
		if(totalPressure < 0.7f){
			DrawElementBlink ("hud/game/disaster_pressure_alert", 0.790, 0.895, 0.127, 0.092);
			DrawElementBlink ("hud/game/disaster_pressure_alert_text", 0.577, 0.912, 0.206, 0.054 );
		}
	}


	private void DrawElement(string slotName, double x, double y, double w, double h, double actualW=-1, double actualH=-1, bool downUp=false){
		int tmpX = PercentW(x);
		int tmpY = PercentH (y);
		int tmpW = PercentW (w);
		int tmpH = PercentH (h);

		int tmpActualW = actualW==-1?tmpW : PercentW (actualW);
		int tmpActualH = actualH==-1?tmpH : PercentH (actualH);

		int groupX = tmpX;
		int groupY = tmpY;
		int textX = 0;
		int textY = 0;
		if (downUp) {
			groupY = Mathf.RoundToInt( (float)(tmpY + tmpH - tmpActualH) );
			textY = Mathf.RoundToInt( -tmpH + tmpActualH);
		}

		GUI.BeginGroup (new Rect (groupX, groupY, tmpActualW, tmpActualH));
		GUI.DrawTexture(new Rect(textX, textY , tmpW, tmpH), Resources.Load(slotName, typeof(Texture))as Texture);
		GUI.EndGroup ();
	}

	private void DrawElementBlink(string slotName, double x, double y, double w, double h, double actualW=-1, double actualH=-1){

		double blinkValue = 0;
		MyKeyValue one = null;
		foreach(MyKeyValue tmp in blinking) {
			if (slotName == tmp.Key){
				one = tmp;
				blinkValue = tmp.Value;
			}
		}

		if (blinkValue > 0) {
			DrawElement (slotName, x, y, w, h, actualW, actualH);
		}

		if (blinkValue > 0.5) {
			blinkValue = -0.5;
		}

		if (one == null) {
			one = new MyKeyValue();
			one.Key = slotName;
			blinking.Add (one);
		} 

		one.Value = blinkValue;

	}

	private int PercentW(double x){
		return (int)(x * Screen.width);
	}

	private int PercentH(double y){
		return (int)(y * Screen.height);
	}

	private void DrawText(string text, GUIStyle font, double x, double y, double w=-1, double h=-1){
		int tmpX = PercentW(x);
		int tmpY = PercentH (y);
		int tmpW = w==-1?Screen.width:PercentW(w);
		int tmpH = w == -1 ? Screen.height : PercentH (h);
		GUI.Label(new Rect(tmpX, tmpY, tmpW, tmpH), text, font);
	}
}
