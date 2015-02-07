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
	public bool allowsFootsteps;

	// Use this for initialization
	void Start() 
	{
	
	}
}
