using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour 
{
	public enum TileType
	{
		Normal,
		Pit,
		Bomb
	}

	public TileType type;

	// Use this for initialization
	void Start() 
	{
	
	}
}
