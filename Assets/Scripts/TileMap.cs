using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMap : MonoBehaviour 
{
	public List<Sprite> sprites;
	public GameObject tilePrefab;
	public GameObject footstepsPrefab;
	public int mapWidth;
	public int mapHeight;

	List<GameObject> _tiles;

	void Awake()
	{
		_tiles = new List<GameObject>();
	}

	// Use this for initialization
	void Start() 
	{
		for (int y = 0; y < mapHeight; ++y)
		{
			for (int x = 0; x < mapWidth; ++x)
			{
				GameObject tileObject = (GameObject)Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
				tileObject.transform.parent = transform;
				tileObject.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, 4)];

				Tile tile = tileObject.GetComponent<Tile>();
				tile.type = Tile.TileType.Bomb;
				tile.allowsFootsteps = Random.Range(0, 2) == 0 ? false : true;

				_tiles.Add(tileObject);
			}
		}
	}

	public GameObject GetTileAt(int x, int y)
	{
		return _tiles[(y * mapWidth) + y];
	}
}
