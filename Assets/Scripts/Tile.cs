using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour 
{
	public Sprite defaultSprite;
	public List<Sprite> snowTiles;

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

	public Player goalFor;

	public void Awake()
	{
		_neighbors = new List<Tile> ();
	}

	public void SetFootsteps(bool allow) 
	{
		allowsFootsteps = allow;

		if (allowsFootsteps) {
			GetComponent<SpriteRenderer> ().sprite = snowTiles[15];
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

	public void SetGoalForPlayer(Player player)
	{
		goalFor = player;
		GetComponent<SpriteRenderer> ().color = Color.blue;
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

	
	public void AutoTile()
	{
		SpriteRenderer snow = transform.FindChild ("Snow").GetComponent<SpriteRenderer> ();
		//snow.gameObject.SetActive (false);
		if (allowsFootsteps) {
			snow.sprite = snowTiles[0];
			return;
		}
		Vector2 me = transform.localPosition;
		int idx = 0;
		foreach (Tile neighbor in _neighbors) {
			Vector2 other = neighbor.transform.localPosition;

			if (neighbor.allowsFootsteps) {
				if (me.y < other.y) {
					idx += 1;
				} else if (me.x < other.x) {
					idx += 2;
				} else if (me.y > other .y) {
					idx += 4;
				} else if (me.x > other.x) {
					idx += 8;
				}
			}
		}
		if (idx == 0) {
			snow.enabled = false;
		} 
		snow.sprite = snowTiles [idx];
	}
}
