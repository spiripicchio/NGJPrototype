using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMap : MonoBehaviour 
{
	public Game game;
	public Tile tilePrefab;
	public GameObject footstepsPrefab;
	public int mapWidth;
	public int mapHeight;

	public float seed;
	[Range(2,8)]
	public float sandScale = 4.0f;
	[Range(0,1)]
	public float sandAmount = 0.4f;

	[Range(2,8)]
	public float pitScale = 4.0f;
	[Range(0,1)]
	public float pitsAmount = 0.4f;

	List<Tile> _tiles;

	//[HideInInspector]
	public List<Tile> startingTiles;

	//[HideInInspector]
	public List<Tile> goalTiles;


	void Awake()
	{
	}
	// Use this for initialization
	void Start() 
	{
		Generate();
	}

	public void Generate()
	{
		StartCoroutine (GenerateCoroutine ());
	}

	IEnumerator GenerateCoroutine() 
	{
		int attempts = 10;
		while (attempts -- > 0) 
		{
			bool success = GenerateAttempt();

			if (success) 
			{
				yield break;
			}
			//yield return new WaitForSeconds(2.0f);
		}
		Debug.LogError ("Failed to generate map");
	}

	bool GenerateAttempt()
	{
		// Destroy tiles.
		foreach (Transform child in transform) 
		{
			Destroy (child.gameObject);
		}
		_tiles = new List<Tile>();
		startingTiles = new List<Tile> ();
		goalTiles = new List<Tile> ();

		seed = Random.value;

		for (int y = 0; y < mapHeight; ++y)
		{
			for (int x = 0; x < mapWidth; ++x)
			{
				Tile tile = (Tile)Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
				tile.transform.parent = transform;
				
				float perlinSand = Mathf.PerlinNoise(x / sandScale + seed * 1000000, y / sandScale);
				tile.SetFootsteps(perlinSand < sandAmount);

				float perlinPit = Mathf.PerlinNoise(x / pitScale + (seed - .5f) * 1000000, y / pitScale);
				tile.SetPit(perlinPit < pitsAmount);

				if (x == 0 || y == 0 || x == (mapWidth - 1) || y == (mapHeight - 1))
				{
					tile.isObstacle = true;
				}
				
				_tiles.Add(tile);

				if (x > 0)
				{
					tile.AddNeighbors(GetTileAt(x - 1, y));
				}
				if (y > 0)
				{
					tile.AddNeighbors(GetTileAt(x, y - 1));
				}
			}
		}

		_tiles.ForEach (tile => {
			tile.AutoTile(); });

		// Add spawns
		Tile leftSpawn = GetTileAt (1, Random.Range (1, mapHeight - 1));
		Tile rightSpawn = GetTileAt (mapWidth - 2, Random.Range (1, mapHeight - 1)); 

		startingTiles.Add( leftSpawn );
		startingTiles.Add( rightSpawn );
		goalTiles.Add ( rightSpawn );
		goalTiles.Add ( leftSpawn );

		leftSpawn.SetGoalForPlayer (game.playerTwo);
		rightSpawn.SetGoalForPlayer (game.playerOne);

		return (IsPath (startingTiles[0], goalTiles[0]) && 
			IsPath(startingTiles[1], goalTiles[1]));
	}
	
	public bool IsPath(Tile start, Tile end)
	{
		_tiles.ForEach (tile => {
			tile.visited = false; });
		return start.CanReachTile(end);
	}
	
	public Tile GetTileAt(Vector2 coord)
	{
		return GetTileAt(Mathf.FloorToInt(coord.x), Mathf.FloorToInt(coord.y));
	}

	public Tile GetTileAt(int x, int y)
	{
		return _tiles[(y * mapWidth) + x];
	}

	public void FadePits()
	{
		foreach (Tile tile in _tiles)
		{
			tile.ShowPit();
		}
	}
}
