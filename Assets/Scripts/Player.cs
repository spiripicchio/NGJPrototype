using UnityEngine;
using System.Collections;
using XInputDotNetPure; // Required in C#



public class Player : MonoBehaviour {

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

	float moveTimer;
	Vector2 previousDirectionInput;
	bool vibrating;

	GamePadState gamepad;

	// Use this for initialization
	void Start () {
		RotateTo (currentDirection);
		transform.localPosition = new Vector3(coord.x, coord.y, -1)	;
	}

	Vector2 GetDirectionInput()
	{
		moveTimer -= Time.deltaTime;
		Vector2 directionInput = Vector2.zero;
		if (gamepad.DPad.Up == ButtonState.Pressed) {
			directionInput = Vector2.up;
		} else if (gamepad.DPad.Down == ButtonState.Pressed) {
			directionInput = -Vector2.up;
		} else if (gamepad.DPad.Left == ButtonState.Pressed) {
			directionInput = -Vector2.right;
		} else if (gamepad.DPad.Right == ButtonState.Pressed) {
			directionInput = Vector2.right;
		}

		if (directionInput  == previousDirectionInput || moveTimer > 0) {
			return Vector2.zero;
		}
		moveTimer = moveCooldown;
		previousDirectionInput = directionInput;
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
		Tile currentTile = tileMap.GetTileAt (coord);

		if (targetTile.allowsFootsteps || currentTile.allowsFootsteps) {
			Instantiate (footstepsPrefab, (coord + targetCoord) / 2, Quaternion.LookRotation (new Vector3 (0, 0, -1), currentDirection));
		}

		transform.localPosition = targetCoord;
		coord = targetCoord;

		if (targetTile.type == Tile.TileType.Pit) {
			GetComponent<SpriteRenderer> ().color = Color.red;
		}
	}

	void CheckDanger(Vector2 atCoord)
	{
		//Debug.Log (atCoord);
		Tile targetTile = tileMap.GetTileAt (atCoord);
		bool danger = targetTile.type == Tile.TileType.Pit;

		//targetTile.GetComponent<SpriteRenderer> ().color = new Color(Random.value * .2f,Random.value * .5f, 1);
		if (danger) {
			receivingPlayer.HearDanger ();
		}
	}

	public void HearDanger()
	{
		StartCoroutine (HearDangerCoroutine ());
	}

	IEnumerator HearDangerCoroutine()
	{
		vibrating = true;
		GamePad.SetVibration (playerIndex, vibrateStrengthLeft, vibrateStrengthRight);

		yield return new WaitForSeconds (vibrateDuration);
		vibrating = false;
		GamePad.SetVibration (playerIndex, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		// update state once per frame;
		gamepad = GamePad.GetState (playerIndex);

		Vector2 dir = GetDirectionInput ();

		if (dir.sqrMagnitude > 0) {

			Vector2 targetCoord = coord + dir;

			if (dir == currentDirection) {
				MoveTo(targetCoord);
			} else {
				RotateTo(dir);
			}

			CheckDanger(coord + dir);
		}

		// Update vibration
		if (vibrating) {
			//GamePad.SetVibration (playerIndex, vibrateStrengthLeft, vibrateStrengthRight);
		}
	}
}
