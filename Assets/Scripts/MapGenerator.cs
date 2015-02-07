using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour 
{
	public List<GameObject> tilesPrefabs;
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
				GameObject tile = (GameObject)Instantiate(tilesPrefabs[Random.Range(0, 3)], new Vector3(x, y), Quaternion.identity);
				tile.transform.parent = transform;

				_tiles.Add(tile);
			}
		}
	}

	public GameObject GetTileAt(int x, int y)
	{
		return _tiles[(y * mapWidth) + y];
	}
}
