using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room {
	
	public List<Tile> tiles;
	public float pressure;
	
	public Vector3 center;
	private Vector3 startPoint;
	private Vector3 endPoint;
	private Vector3 offsetVector;
	
	private float initialPressure;
	
	public Room(float initialPressure){
		tiles = new List<Tile>();
		center = new Vector3();
		startPoint = new Vector3(10000.0f,1000.0f,1000.0f);
		endPoint = new Vector3(-10000.0f,-1000.0f,-1000.0f);
		offsetVector = new Vector3(0.6f,1.0f,1.2f);
		this.initialPressure = initialPressure;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void countPressure(){
		int d = 0;
		pressure = 0;
		foreach(Tile tile in tiles){
			d++;
			pressure += tile.pressure;
		}
		pressure = pressure / (float)d;
	}
	
	public void AddTile(Tile tile){
		
		Vector3 pos = tile.GetComponent<Transform>().position;
		if(startPoint.x > pos.x) startPoint.x = pos.x;
		if(startPoint.y > pos.y) startPoint.y = pos.y;
		if(startPoint.z > pos.z) startPoint.z = pos.z;
		
		if(endPoint.x < pos.x) endPoint.x = pos.x;
		if(endPoint.y < pos.y) endPoint.y = pos.y;
		if(endPoint.z < pos.z) endPoint.z = pos.z;
		
		center.x = (startPoint.x + endPoint.x)*0.5f + offsetVector.x;
		center.y = (startPoint.y + endPoint.y)*0.5f + offsetVector.y;
		center.z = (startPoint.z + endPoint.z)*0.5f + offsetVector.z;
		
		tile.pressure = initialPressure;
		tiles.Add(tile);
	}
	
}
