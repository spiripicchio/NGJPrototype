using UnityEngine;
using System.Collections;

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

	public void SetFootsteps(bool allow) 
	{
		allowsFootsteps = allow;

		if (allowsFootsteps) {
			GetComponent<SpriteRenderer> ().sprite = sandSprite;
		} else {
			GetComponent<SpriteRenderer> ().sprite = defaultSprite;
		}
	}

	public void SetPit(bool isPit)
	{
		if (isPit) {
			type = TileType.Pit;
			//GetComponent<SpriteRenderer> ().color = Color.red;
		}

	}
}
