using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Tile : MonoBehaviour 
{
	public Sprite defaultSprite;
	public Sprite trashedIglooSprite;
	public List<Sprite> snowTiles;
	public bool isObstacle;

	public enum TileType
	{
		Normal,
		Pit
	}

	public TileType type = TileType.Normal;
	public bool allowsFootsteps;


	[HideInInspector]
	public bool visited = false;
	public Player goalFor;

	List<Tile> _neighbors;
	SpriteRenderer _pitSprite;

	public void Awake()
	{
		_neighbors = new List<Tile>();
		_pitSprite = transform.FindChild("Pit").GetComponent<SpriteRenderer>();
	}

	public void SetFootsteps(bool allow) 
	{
		allowsFootsteps = allow;

		if (allowsFootsteps) 
		{
			GetComponent<SpriteRenderer> ().sprite = snowTiles[15];
		} 
		else 
		{
			GetComponent<SpriteRenderer> ().sprite = defaultSprite;
			//GetComponent<SpriteRenderer> ().color = new Color(0,0,0,0);
		}
	}

	public void SetPit(bool isPit)
	{
		if (isPit) 
		{
			type = TileType.Pit;
			//GetComponent<SpriteRenderer> ().color = Color.red;
		}
		else
		{
			type = TileType.Normal;
		}
	}

	public void SetGoalForPlayer(Player player)
	{
		goalFor = player;
		//GetComponent<SpriteRenderer> ().color = Color.blue;
		//transform.FindChild ("Snow").GetComponent<SpriteRenderer>().color = Color.blue;
		transform.FindChild ("Iglo").GetComponent<SpriteRenderer>().color = Color.white;
		if (transform.localPosition.x > 10)
		{
			transform.FindChild ("Iglo").transform.localScale = new Vector3(-1, 1, 1);
		}
	}

	public void TrashIgloo()
	{
		transform.FindChild("Iglo").GetComponent<SpriteRenderer>().sprite = trashedIglooSprite;
		transform.FindChild("Poop").GetComponent<ParticleSystem>().Emit(50);
	}
	
	public void AddNeighbors(Tile neighbor)
	{
		_neighbors.Add(neighbor);
		neighbor._neighbors.Add(this);
	}

	public bool IsDeadly()
	{
		return type == TileType.Pit;
	}

	public bool CanReachTile(Tile target)
	{
		if (IsDeadly () || isObstacle)
		{
			return false;
		}
		if (this == target) 
		{
			return true;
		}
		if (!visited) 
		{
			visited = true;
			foreach (Tile neighbor in _neighbors) 
			{
				if (neighbor.CanReachTile(target))
				{
					return true;
				}
			}
		}
		return false;
	}
	
	public void AutoTile()
	{
		SpriteRenderer snow = transform.FindChild ("Snow").GetComponent<SpriteRenderer>();
		//snow.gameObject.SetActive (false);
		if (allowsFootsteps) 
		{
			snow.sprite = snowTiles[0];
			return;
		}
		Vector2 me = transform.localPosition;
		int idx = 0;
		foreach (Tile neighbor in _neighbors) 
		{
			Vector2 other = neighbor.transform.localPosition;

			if (neighbor.allowsFootsteps) 
			{
				if (me.y < other.y) 
				{
					idx += 1;
				} 
				else if (me.x < other.x) 
				{
					idx += 2;
				} 
				else if (me.y > other.y) 
				{
					idx += 4;
				} 
				else if (me.x > other.x) 
				{
					idx += 8;
				}
			}
		}
		if (idx == 0) 
		{
			snow.enabled = false;
		} 
		snow.sprite = snowTiles[idx];
	}

	public void ShowPit( bool fadeOut = true)
	{
		if (IsDeadly() == true)
		{
			StartCoroutine(FadePit(fadeOut: fadeOut));
		}
	}

	public void ShowSplash()
	{
		transform.FindChild("Splash").GetComponent<ParticleSystem>().Emit(50);
	}

	IEnumerator FadePit( bool fadeOut = true)
	{
		while (_pitSprite.color.a < 1.0f)
		{
			_pitSprite.color = new Color(_pitSprite.color.r, _pitSprite.color.g, _pitSprite.color.b, _pitSprite.color.a + 0.06f);

			yield return new WaitForSeconds(0.01f);
		}

		if (fadeOut)
		{
			yield return new WaitForSeconds(3);
			
			while (_pitSprite.color.a > 0)
			{
				_pitSprite.color = new Color(_pitSprite.color.r, _pitSprite.color.g, _pitSprite.color.b, _pitSprite.color.a - 0.06f);
				
				yield return new WaitForSeconds(0.01f);
			}
		}

	}
}
