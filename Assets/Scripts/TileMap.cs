using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMap : MonoBehaviour 
{
	public List<Sprite> sprites;
	public Tile tilePrefab;
	public GameObject footstepsPrefab;
	public int mapWidth;
	public int mapHeight;

	List<Tile> _tiles;

	void Awake()
	{
		_tiles = new List<Tile>();
	}

	// Use this for initialization
	void Start() 
	{
		for (int y = 0; y < mapHeight; ++y)
		{
			for (int x = 0; x < mapWidth; ++x)
			{
				Tile tile = (Tile)Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
				tile.transform.parent = transform;

				tile.SetFootsteps(Random.value > 0.5);

				tile.SetPit(Random.value > 0.8);

				_tiles.Add(tile);
			}
		}
	}

	public Tile GetTileAt(Vector2 coord)
	{
		return _tiles[(Mathf.FloorToInt(coord.y) * mapWidth) + Mathf.FloorToInt(coord.x)];
	}
}
