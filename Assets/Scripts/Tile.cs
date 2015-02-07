using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour 
{
	public Sprite defaultSprite;
	public Sprite sandSprite;

	public enum TileType
	{
		Normal,
		Pit
	}

	public TileType type = TileType.Normal;
	public bool allowsFootsteps;

	List<Tile> _neighbors;

	[HideInInspector]
	public bool visited = false;

	public void Awake()
	{
		_neighbors = new List<Tile> ();
	}

	public void SetFootsteps(bool allow) 
	{
		allowsFootsteps = allow;

		if (allowsFootsteps) {
			GetComponent<SpriteRenderer> ().sprite = sandSprite;
		} else {
			GetComponent<SpriteRenderer> ().sprite = defaultSprite;
			//GetComponent<SpriteRenderer> ().color = new Color(0,0,0,0);
		}
	}

	public void SetPit(bool isPit)
	{
		if (isPit) {
			type = TileType.Pit;
			//GetComponent<SpriteRenderer> ().color = Color.red;
		}
	}

	public void AddNeighbors(Tile neighbor)
	{
		_neighbors.Add (neighbor);
		neighbor._neighbors.Add (this);
	}

	public bool IsDeadly()
	{
		return type == TileType.Pit;
	}

	public bool CanReachTile(Tile target)
	{
		if (this == target) {
			return true;
		}
		if (!visited) {
			visited = true;
			foreach (Tile neighbor in _neighbors) {
				if (!neighbor.IsDeadly() && neighbor.CanReachTile(target))
				{
					return true;
				}
			}
		}
		return false;
	}
}
