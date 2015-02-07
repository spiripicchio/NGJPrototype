using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	public Player playerOne;
	public Player playerTwo;
	public TileMap tileMap;
	public float respawnCooldown;
	public int lives;
	public Text outroMessage;
	public Text score1Text;
	public Text score2Text;

	enum WinningPlayer
	{
		PlayerOne,
		PlayerTwo,
		None,
		Unknown
	}

	WinningPlayer _winningPlayer;
	IEnumerator _resetPlayerOneCoroutine;
	IEnumerator _resetPlayerTwoCoroutine;
	bool _inCoroutineOne;
	bool _inCoroutineTwo;
	int _score1;
	int _score2;

	// Use this for initialization
	void Start() 
	{
		StartCoroutine(MainLoop());
	}

	IEnumerator MainLoop()
	{
		while (true)
		{
			Reset();

			while (_winningPlayer == WinningPlayer.Unknown)
			{
				yield return null;
			}

			if (_inCoroutineOne == true)
			{
				StopCoroutine(_resetPlayerOneCoroutine);
				_inCoroutineOne = false;
			}
			if (_inCoroutineTwo == true)
			{
				StopCoroutine(_resetPlayerTwoCoroutine);
				_inCoroutineTwo = false;
			}

			// Outro
			switch (_winningPlayer)
			{
				case WinningPlayer.PlayerOne:
				{
					outroMessage.text = "PLAYER ONE WINS!";
					++_score1;
					score1Text.text = _score1.ToString();
					break;
				}

				case WinningPlayer.PlayerTwo:
				{
					outroMessage.text = "PLAYER TWO WINS!";
					++_score2;
					score2Text.text = _score2.ToString();
					break;
				}

				case WinningPlayer.None:
				{
					outroMessage.text = "FAIL!";
					break;
				}
			}

			yield return new WaitForSeconds(5);
		}
	}

	void Reset()
	{
		_winningPlayer = WinningPlayer.Unknown;

		tileMap.Generate();

		ResetPlayer(playerOne);
		ResetPlayer(playerTwo);
		playerOne.lives = lives;
		playerTwo.lives = lives;

		playerOne.goalTile = tileMap.goalTiles[0];
		playerTwo.goalTile = tileMap.goalTiles[1];

		outroMessage.text = "";
	}

	// Update is called once per frame
	void Update() 
	{
		if (Input.GetButtonDown("Jump") == true)
		{
			_score1 = 0;
			score1Text.text = "0";
			_score2 = 0;
			score2Text.text = "0";
		}

		if (_winningPlayer == WinningPlayer.Unknown)
		{
			if (playerOne.isDead == true && playerTwo.isDead == true)
			{
				Debug.Log("Game over\n");
				_winningPlayer = WinningPlayer.None;
			}
			else if (playerOne.reachedGoal == true)
			{
				Debug.Log("Won 1\n");
				_winningPlayer = WinningPlayer.PlayerOne; 
			}
			else if (playerTwo.reachedGoal == true)
			{
				Debug.Log("Won 2\n");
				_winningPlayer = WinningPlayer.PlayerTwo; 
			}
			else
			{
				if (playerOne.isDead == true && _inCoroutineOne == false && playerOne.lives > 1)
				{
					--playerOne.lives;
					_resetPlayerOneCoroutine = ResetPlayerAndWait(playerOne);
					StartCoroutine(_resetPlayerOneCoroutine);
				}
				if (playerTwo.isDead == true && _inCoroutineTwo == false && playerTwo.lives > 1)
				{
					--playerTwo.lives;
					_resetPlayerTwoCoroutine = ResetPlayerAndWait(playerTwo);
					StartCoroutine(_resetPlayerTwoCoroutine);
				}
			}
		}
	}

	IEnumerator ResetPlayerAndWait(Player player)
	{
		if (player.playerIndex == XInputDotNetPure.PlayerIndex.One)
		{
			_inCoroutineOne = true;
		}
		if (player.playerIndex == XInputDotNetPure.PlayerIndex.Two)
		{
			_inCoroutineTwo = true;
		}

		yield return new WaitForSeconds(respawnCooldown);

		ResetPlayer(player);

		if (player.playerIndex == XInputDotNetPure.PlayerIndex.One)
		{
			_inCoroutineOne = false;
		}
		if (player.playerIndex == XInputDotNetPure.PlayerIndex.Two)
		{
			_inCoroutineTwo = false;
		}
	}

	void ResetPlayer(Player player)
	{
		player.Reset();
		player.SetStartingPosition(tileMap.startingTiles[(int)player.playerIndex].transform.localPosition);
	}
}
