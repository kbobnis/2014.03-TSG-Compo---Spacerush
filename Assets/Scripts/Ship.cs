using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Threading;

public class Ship : MonoBehaviour {

	public GameObject tilePrefab;
	public GameObject oxygenSystemPrefab;
	public GameObject coolingSystemPrefab;
	public GameObject healingSystemPrefab;
	public GameObject meteoriteSystemPrefab;

	public TextAsset shipSpec;

	private List<Tile> tiles = new List<Tile>();
	private List<Room> rooms;

	public static List<Tile> tilesInShip = new List<Tile>();
	public static Ship me;
	public static double score;
	public double overheat;
	public float overheatProgress;

	// Use this for initialization
	void Start () {
		////CultureInfo.CreateSpecificCulture("en-GB")
		CultureInfo ci = new CultureInfo("en-GB");
		Thread.CurrentThread.CurrentCulture = ci;
		Thread.CurrentThread.CurrentUICulture = ci;
		
		LoadXml ();
		Ship.me = this;
	}

	private void LoadXml(){
		XmlDocument xmlDoc = new XmlDocument ();
		xmlDoc.LoadXml (shipSpec.text);
		XmlNode parameters = xmlDoc.GetElementsByTagName ("parameters").Item(0);
	
			
		Config me = new Config();
		XmlNodeList actionSpeeds = xmlDoc.GetElementsByTagName ("action_speed");
		XmlNode actionSpeed = actionSpeeds.Item (0);
		XmlAttribute actionSpeedAttribute = actionSpeed.Attributes["value"];
		string actionSpeedValue = actionSpeedAttribute.Value;
		me.ActionSpeed = float.Parse( actionSpeedValue );
		me.CoolingFactor = float.Parse( xmlDoc.GetElementsByTagName ("cooling_factor").Item (0).Attributes["value"].Value  );
		me.HealPercentage = float.Parse( xmlDoc.GetElementsByTagName ("heal_percentage").Item (0).Attributes["value"].Value  );
		me.OverheatFactor = float.Parse( xmlDoc.GetElementsByTagName ("overheat_factor").Item (0).Attributes["value"].Value  );
		me.SuffocateFactor = float.Parse( xmlDoc.GetElementsByTagName ("suffocate_factor").Item (0).Attributes["value"].Value  );
		me.MinO2ToThrive = float.Parse( xmlDoc.GetElementsByTagName ("min_o2_to_thrive").Item (0).Attributes["value"].Value );
		me.FireDamageFactor = float.Parse( xmlDoc.GetElementsByTagName ("fire_damage_factor").Item (0).Attributes["value"].Value );
		me.PropagateO2Factor = float.Parse( xmlDoc.GetElementsByTagName ("propagate_o2_factor").Item (0).Attributes["value"].Value );
		me.MinO2Level = float.Parse( xmlDoc.GetElementsByTagName ("min_o2_level").Item (0).Attributes["value"].Value );
		me.ReduceO2 = float.Parse( xmlDoc.GetElementsByTagName ("reduce_o2").Item (0).Attributes["value"].Value );
		me.PropagateFireFactor = float.Parse( xmlDoc.GetElementsByTagName ("propagate_fire_factor").Item (0).Attributes["value"].Value );
		me.OverheatProgressFactor = float.Parse( xmlDoc.GetElementsByTagName ("overheat_progress_factor").Item (0).Attributes["value"].Value );
		me.FireContagiousness = int.Parse( xmlDoc.GetElementsByTagName ("fire_contagiuosness").Item (0).Attributes["value"].Value );
		me.FireSuffocateFactor = float.Parse( xmlDoc.GetElementsByTagName ("fire_suffocate_factor").Item (0).Attributes["value"].Value );

		Config.me = me;

		XmlNode bg = xmlDoc.GetElementsByTagName ("bg").Item (0);
		foreach (XmlElement child in bg.ChildNodes) {
			AddBackgroundImage (child.GetAttribute ("url"),
           int.Parse (child.GetAttribute ("x")),
           int.Parse (child.GetAttribute ("y")));
		}

		foreach (XmlNode tile in xmlDoc.GetElementsByTagName("tile")) {
			string left = "empty", right = "empty", up = "empty", down = "empty", system = "none";
			if (tile.Attributes["left"] != null) {
				left = tile.Attributes ["left"].Value;
			}
			if (tile.Attributes["right"] != null) {
				right = tile.Attributes ["right"].Value;
			}
			if (tile.Attributes["up"] != null) {
				up = tile.Attributes ["up"].Value;
			}
			if (tile.Attributes["down"] != null) {
				down = tile.Attributes ["down"].Value;
			}
			if (tile.Attributes["system"] != null) {
				system = tile.Attributes ["system"].Value;
			}
			Tile newTile = AddTile(int.Parse (tile.Attributes ["c"].Value),
			        int.Parse (tile.Attributes ["r"].Value),
			        left,
			        up,
			        right,
			        down,
			        true,
			        system,
			                    tile.Attributes["floor"].Value);


			tiles.Add(newTile);
			if (newTile.floorType == FloorTypeEnum.INTERIOR){
				Ship.tilesInShip.Add(newTile);
			}
		}
		XmlNodeList voidGenerators = xmlDoc.GetElementsByTagName ("voidgenerator");
		foreach (XmlElement voidGenerator in voidGenerators) {
			Tile.reduceO2Factor = float.Parse(voidGenerator.Attributes["reduceO2Factor"].Value);
			Tile.reducePressureFactor = 0.1f;
			AddVoidGenerator (int.Parse (voidGenerator.Attributes ["c"].Value),
			                  int.Parse (voidGenerator.Attributes ["r"].Value));
		}
		XmlNodeList metoriteConfigs = xmlDoc.GetElementsByTagName ("meteorite");
		GameObject meteorSystem = Instantiate (meteoriteSystemPrefab) as GameObject;
		MeteoriteSystem meteorComponent = meteorSystem.GetComponent<MeteoriteSystem> ();
		foreach (XmlElement config in metoriteConfigs) {
			meteorComponent.AddConfig(int.Parse(config.GetAttribute("duration")),
			                          float.Parse(config.GetAttribute("probability")),
			            			  int.Parse(config.GetAttribute("cooldown")),
			                          float.Parse(config.GetAttribute("fireProbability")),
			                          float.Parse(config.GetAttribute("destroyProbability")));
		}

		

		XmlNodeList minigameConfigs = xmlDoc.GetElementsByTagName ("minigame");
		foreach (XmlElement minigameConfig in minigameConfigs) {
			Minigame.actionSpeed = float.Parse( minigameConfig.Attributes["actionSpeed"].Value);
		}


		XmlNodeList ships = xmlDoc.GetElementsByTagName ("ship");
		foreach (XmlElement ship in ships) {
			Tile.passiveReduceo2Factor = float.Parse( ship.Attributes["passiveReduceO2Factor"].Value);
		}

		PrepareTilesNeighbours();
		InvokeRepeating("GenerateVoidScent", 1.0f, 1.0f);
		GenerateVoidScent();
		PrepareRooms(false);
		Camera.main.GetComponent<Hud> ().enabled = true;
	}

	public List<Room> GetRooms(){
		return rooms;
	}

	void PrepareRooms(bool fakeData){
		rooms = new List<Room>();

		List<Tile> noScentTiles = new List<Tile>();
		foreach (Tile tile in tiles) {
			if(tile.voidScent == false){
				noScentTiles.Add(tile);
			}
		}
		 
		List<Tile> tmp = new List<Tile> ();
		HashSet<Tile> visited = new HashSet<Tile> ();
		Tile currentTile;

		int counter = 0;
		while(noScentTiles.Count > 0){
			Room room = new Room(fakeData ? 0.1f*(float)(counter++  % 10) : 1.0f);
			tmp.Add(noScentTiles[0]);
			room.AddTile(noScentTiles[0]);
			noScentTiles.Remove(noScentTiles[0]);
			while (tmp.Count > 0) {
				currentTile = tmp[0];
				tmp.RemoveAt(0);
				visited.Add(currentTile);
				
				if ( currentTile.upNeighbour && !visited.Contains(currentTile.upNeighbour) && TileEdge.IsPassable(currentTile.up) && TileEdge.IsPassable(currentTile.upNeighbour.down)) {
					room.AddTile(currentTile.upNeighbour);
					tmp.Add(currentTile.upNeighbour);
					noScentTiles.Remove(currentTile.upNeighbour);
				}
				if ( currentTile.downNeighbour && !visited.Contains(currentTile.downNeighbour) && TileEdge.IsPassable(currentTile.down) && TileEdge.IsPassable(currentTile.downNeighbour.up)) {
					room.AddTile(currentTile.downNeighbour);
					tmp.Add(currentTile.downNeighbour);
					noScentTiles.Remove(currentTile.downNeighbour);
				}
				if ( currentTile.leftNeighbour && !visited.Contains(currentTile.leftNeighbour) && TileEdge.IsPassable(currentTile.left) && TileEdge.IsPassable(currentTile.leftNeighbour.right)) {
					room.AddTile(currentTile.leftNeighbour);
					tmp.Add(currentTile.leftNeighbour);
					noScentTiles.Remove(currentTile.leftNeighbour);
				}
				if ( currentTile.rightNeighbour && !visited.Contains(currentTile.rightNeighbour) && TileEdge.IsPassable(currentTile.right) && TileEdge.IsPassable(currentTile.rightNeighbour.left)) {
					room.AddTile(currentTile.rightNeighbour);
					tmp.Add(currentTile.rightNeighbour);
					noScentTiles.Remove(currentTile.rightNeighbour);
				}
			}
			rooms.Add(room);
		}

	}

	void GenerateVoidScent() {
		Queue<Tile> queue = new Queue<Tile> ();
		HashSet<Tile> visitedTiles = new HashSet<Tile> ();
		foreach (Tile tile in tiles) {
			if (tile.isVoidGenerator) {
				queue.Enqueue (tile);
			}
			tile.voidScent = false;
		}
		while (queue.Count > 0) {
			Tile tile = queue.Dequeue();
			tile.voidScent = true;
			visitedTiles.Add (tile);
			if ( tile.upNeighbour && !visitedTiles.Contains(tile.upNeighbour) && TileEdge.IsPassable(tile.up) && TileEdge.IsPassable(tile.upNeighbour.down)) {
				tile.upNeighbour.voidScent = true;
				queue.Enqueue(tile.upNeighbour);
			}
			if ( tile.downNeighbour && !visitedTiles.Contains(tile.downNeighbour) && TileEdge.IsPassable(tile.down) && TileEdge.IsPassable(tile.downNeighbour.up)) {
				tile.downNeighbour.voidScent = true;
				queue.Enqueue(tile.downNeighbour);
			}
			if ( tile.leftNeighbour && !visitedTiles.Contains(tile.leftNeighbour) && TileEdge.IsPassable(tile.left) && TileEdge.IsPassable(tile.leftNeighbour.right)) {
				tile.leftNeighbour.voidScent = true;
				queue.Enqueue(tile.leftNeighbour);
			}
			if ( tile.rightNeighbour && !visitedTiles.Contains(tile.rightNeighbour) && TileEdge.IsPassable(tile.right) && TileEdge.IsPassable(tile.rightNeighbour.left)) {
				tile.rightNeighbour.voidScent = true;
				queue.Enqueue(tile.rightNeighbour);
			}
		}
	}

	void PrepareTilesNeighbours(){
		InGamePosition currentTilePos;
		InGamePosition pos;
		foreach (Tile currentTile in tiles) {
			currentTilePos = currentTile.gameObject.GetComponent<InGamePosition> ();
			foreach (Tile tile in tiles) {
				pos = tile.gameObject.GetComponent<InGamePosition> ();
				if(currentTilePos.row == pos.row){
					if(currentTilePos.column - 1 == pos.column){
						currentTile.leftNeighbour = tile;
					} else if(currentTilePos.column + 1 == pos.column ){
						currentTile.rightNeighbour = tile;
					}
				} else if(currentTilePos.column == pos.column){
					if(currentTilePos.row -1 == pos.row ){
						currentTile.upNeighbour = tile;
					} else if(currentTilePos.row +1 == pos.row ){
						currentTile.downNeighbour = tile;
					}
				}
			}

			foreach(TileEdge tileEdge in currentTile.GetSides()){
				if (tileEdge != null){
					if (tileEdge.side == Side.UP && currentTile.upNeighbour != null ){
						currentTile.upNeighbour.down.otherSideOfEdge = tileEdge;
						currentTile.up.otherSideOfEdge = currentTile.upNeighbour.down;
					}
					if ( tileEdge.side == Side.DOWN && currentTile.downNeighbour != null){
						currentTile.downNeighbour.up.otherSideOfEdge = tileEdge;
						currentTile.down.otherSideOfEdge = currentTile.downNeighbour.up;
					}
					if (tileEdge.side == Side.LEFT && currentTile.leftNeighbour != null){
						currentTile.leftNeighbour.right.otherSideOfEdge = tileEdge;
						currentTile.left.otherSideOfEdge = currentTile.leftNeighbour.right;
					}
					if (tileEdge.side == Side.RIGHT && currentTile.rightNeighbour != null){
						currentTile.rightNeighbour.left.otherSideOfEdge = tileEdge;
						currentTile.right.otherSideOfEdge = currentTile.rightNeighbour.left;
					}
				}
			}
		}


	}


	void AddVoidGenerator(int column, int row) {
		GetTileOnPosition (column, row).SetAsVoidGenerator();
	}

	public Tile GetTileOnPosition(InGamePosition pos){
		return GetTileOnPosition (pos.column, pos.row);
	}

	public Tile GetTileOnPosition(int column, int row) {
		foreach (Tile tile in tiles) {
			if(tile.GetComponent<InGamePosition>().column == column && 
			   tile.GetComponent<InGamePosition>().row == row){
				return tile;
			}
		}
		return null;
	}

	void AddBackgroundImage(string imageName, int x, int y) {
		Texture background = Resources.Load<Texture> (imageName);
	}

	Tile AddTile(int column, int row, string left, string top, string right, string bottom, bool walkable, string system, string floorType) {
		GameObject newTile = (GameObject) Instantiate (tilePrefab);
		Tile tileComponent = newTile.GetComponent<Tile> ();
		tileComponent.walkable = walkable;
		InGamePosition position = newTile.GetComponent<InGamePosition> ();
		position.column = column;
		position.row = row;
		position.UpdatePosition();
		switch (system) {
			case "o2":
				tileComponent.system = Instantiate(oxygenSystemPrefab) as GameObject;
				break;
			case "medic":
				tileComponent.system = Instantiate(healingSystemPrefab) as GameObject;
				break;
			case "cooling":
				tileComponent.system = Instantiate(coolingSystemPrefab) as GameObject;
				break;
			default:
				break;
		}
		if (tileComponent.system != null) {
			tileComponent.system.transform.position = position.transform.position;
		}
		tileComponent.left = createEdge (left, Side.LEFT);
		tileComponent.right = createEdge (right, Side.RIGHT);
		tileComponent.up = createEdge (top, Side.UP);
		tileComponent.down = createEdge (bottom, Side.DOWN);
		if (floorType == "interior") {
			tileComponent.floorType = FloorTypeEnum.INTERIOR;
			tileComponent.o2Level = 1;
		}
		else {
			tileComponent.floorType = FloorTypeEnum.EXTERIOR;
			tileComponent.o2Level = 0;
		}
		return tileComponent;
	}

	TileEdge createEdge(string edgeType, Side side) {
		switch (edgeType) {
		case "door":
			return new Door(side);
		case "Sdoor":
			Door strongDoor = new Door(side);
			strongDoor.isStrong = true;
			return strongDoor;
		case "wall":
			return new Wall(side);
		case "hull":
			return new Hull(side);
		}
		return null;
	}

	// Update is called once per frame
	void Update () {
		score += Time.deltaTime;

		//get all cooling systems
		foreach (GameObject cooler in GameObject.FindGameObjectsWithTag ("cooler")) {
			if (!cooler.GetComponent<TileSystem>().isBroken){
				overheat -= Time.deltaTime * Config.me.CoolingFactor;
			}
			overheat += Time.deltaTime * Config.me.OverheatFactor;
		}
		if (overheat > 1) {
			overheat = 1;
		} else if (overheat < 0) {
			overheat = 0;
		}

		if (overheat >= 1) {
			overheatProgress += Time.deltaTime * Config.me.OverheatProgressFactor;
			if (overheatProgress >= 1){
				overheatProgress = 0;

				GameObject[] tiles = GameObject.FindGameObjectsWithTag("tile");
				int index = new System.Random().Next(0, tiles.Length);
				tiles[index].GetComponent<Tile>().StartFire();
			}
		} else {
			overheatProgress -= Time.deltaTime * Config.me.OverheatProgressFactor;
		}
		if (overheatProgress <= 0) {
			overheatProgress = 0;
		}
	}

	public Tile GetTileForGM(GameObject gameObject){
		return GetTileOnPosition (gameObject.GetComponent<InGamePosition> ());
	}
	
}
