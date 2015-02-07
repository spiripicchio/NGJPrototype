using UnityEngine;
using System.Collections;
using XInputDotNetPure; // Required in C#


public class Player : MonoBehaviour 
{
	public PlayerIndex playerIndex;

	public Vector2 currentDirection = Vector2.right;
	public Vector2 coord;

	public float moveCooldown = 0.1f;

	[Range(0,1)]
	public float vibrateStrengthLeft = 1.0f;
	[Range(0,1)]
	public float vibrateStrengthRight = 0.0f;
	public float vibrateDuration = 1.0f;

	public Player receivingPlayer;
	public GameObject footstepsPrefab;

	public TileMap tileMap;

	[HideInInspector]
	public bool isDead;
	[HideInInspector]
	public bool reachedGoal;
	[HideInInspector]
	public Tile goalTile;
	public int lives;

	float _moveTimer;
	Vector2 _previousDirectionInput;
	bool _vibrating;

	// Use this for initialization
	void Start() 
	{
		RotateTo (currentDirection);
		transform.localPosition = new Vector3(coord.x, coord.y, -1)	;
	}

	public void Reset()
	{
		isDead = false;
		reachedGoal = false;
		_moveTimer = 0;
		_previousDirectionInput = Vector2.zero;
		_vibrating = false;
		GetComponent<SpriteRenderer> ().color = Color.white;
	}

	Vector2 GetDirectionInput()
	{
		_moveTimer -= Time.deltaTime;
		Vector2 directionInput = Vector2.zero;

#if UNITY_STANDALONE_WIN
		GamePadState gamepad = GamePad.GetState (playerIndex);
		if (gamepad.DPad.Up == ButtonState.Pressed) 
		{
			directionInput = Vector2.up;
		} else if (gamepad.DPad.Down == ButtonState.Pressed) 
		{
			directionInput = -Vector2.up;
		} else if (gamepad.DPad.Left == ButtonState.Pressed) 
		{
			directionInput = -Vector2.right;
		} else if (gamepad.DPad.Right == ButtonState.Pressed) 
		{
			directionInput = Vector2.right;
		}
#else
		if (playerIndex == PlayerIndex.One)
		{
			if (Input.GetAxis("Horizontal") < 0)
			{
				directionInput = -Vector2.right;
			}
			if (Input.GetAxis("Horizontal") > 0)
			{
				directionInput = Vector2.right;
			}
			if (Input.GetAxis("Vertical") > 0)
			{
				directionInput = Vector2.up;
			}
			if (Input.GetAxis("Vertical") < 0)
			{
				directionInput = -Vector2.up;
			}
		}
		else
		{
			if (Input.GetAxis("Horizontal2") < 0)
			{
				directionInput = -Vector2.right;
			}
			if (Input.GetAxis("Horizontal2") > 0)
			{
				directionInput = Vector2.right;
			}
			if (Input.GetAxis("Vertical2") > 0)
			{
				directionInput = Vector2.up;
			}
			if (Input.GetAxis("Vertical2") < 0)
			{
				directionInput = -Vector2.up;
			}
		}
		#endif

		if (directionInput  == _previousDirectionInput || _moveTimer > 0) 
		{
			return Vector2.zero;
		}
		_moveTimer = moveCooldown;
		_previousDirectionInput = directionInput;

		return directionInput;
	}

	void RotateTo(Vector2 direction)
	{
		currentDirection = direction;
		transform.localRotation = Quaternion.LookRotation (new Vector3(0,0,-1), direction);
	}

	void MoveTo(Vector2 targetCoord)
	{
		Tile targetTile = tileMap.GetTileAt (targetCoord);
		if (targetTile.isObstacle == false)
		{
			Tile currentTile = tileMap.GetTileAt (coord);
			
			if (targetTile.allowsFootsteps || currentTile.allowsFootsteps) 
			{
				Instantiate (footstepsPrefab, (coord + targetCoord) / 2, Quaternion.LookRotation (new Vector3 (0, 0, -1), currentDirection));
			}
			
			transform.localPosition = targetCoord;
			coord = targetCoord;
			
			if (targetTile.IsDeadly()) 
			{
				GetComponent<SpriteRenderer> ().color = Color.red;
				isDead = true;
			}
			else if (targetTile == goalTile)
			{
				GetComponent<SpriteRenderer> ().color = Color.green;
				reachedGoal = true;
			}
		}
	}

	public void SetStartingPosition(Vector2 position)
	{
		transform.localPosition = position;
		coord = position;
	}

	void CheckDanger(Vector2 atCoord)
	{
		//Debug.Log (atCoord);
		Tile targetTile = tileMap.GetTileAt(atCoord);
		if (targetTile.isObstacle == false)
		{
			bool danger = targetTile.type == Tile.TileType.Pit;
			
			//targetTile.GetComponent<SpriteRenderer> ().color = new Color(Random.value * .2f,Random.value * .5f, 1);
			if (danger) 
			{
				receivingPlayer.HearDanger();
			}
		}
	}

	public void HearDanger()
	{
		StartCoroutine(HearDangerCoroutine());
	}

	IEnumerator HearDangerCoroutine()
	{
		_vibrating = true;
#if UNITY_STANDALONE_WIN
		GamePad.SetVibration (playerIndex, vibrateStrengthLeft, vibrateStrengthRight);
#else
		Debug.Log("BBBBRRRRRR\n");
#endif
		yield return new WaitForSeconds (vibrateDuration);
		_vibrating = false;
#if UNITY_STANDALONE_WIN
		GamePad.SetVibration (playerIndex, 0, 0);
#else
		Debug.Log("Done\n");
#endif
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (isDead == false)
		{
			Vector2 dir = GetDirectionInput();
			
			if (dir.sqrMagnitude > 0) 
			{
				Vector2 targetCoord = coord + dir;
				
				if (dir == currentDirection) 
				{
					MoveTo(targetCoord);
				}
				else 
				{
					RotateTo(dir);
				}
				
				CheckDanger(coord + dir);
			}
			
			// Update vibration
			if (_vibrating) 
			{
				//GamePad.SetVibration (playerIndex, vibrateStrengthLeft, vibrateStrengthRight);
			}
		}
	}
}
