using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
	public Player playerOne;
	public Player playerTwo;
	public TileMap tileMap;
	public float respawnCooldown;
	public int lives;

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

	// Use this for initialization
	void Start() 
	{
		StartCoroutine(MainLoop());
	}

	IEnumerator MainLoop()
	{
		while (true)
		{
			ShowIntro();

			while (_winningPlayer == WinningPlayer.Unknown)
			{
				yield return null;
			}

			StopCoroutine(_resetPlayerOneCoroutine);
			StopCoroutine(_resetPlayerTwoCoroutine);
			_inCoroutineOne = false;
			_inCoroutineTwo = false;

			ShowOutro();
		}
	}

	void ShowIntro()
	{
		_winningPlayer = WinningPlayer.Unknown;

		tileMap.Generate();

		ResetPlayer(playerOne);
		ResetPlayer(playerTwo);

		playerOne.goalTile = tileMap.goalTiles[0];
		playerTwo.goalTile = tileMap.goalTiles[1];
	}

	void ShowOutro()
	{

	}

	// Update is called once per frame
	void Update() 
	{
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
				if (playerOne.isDead == true && _inCoroutineOne == false && playerOne.lives > 0)
				{
					--playerOne.lives;
					_resetPlayerOneCoroutine = ResetPlayerAndWait(playerOne);
					StartCoroutine(_resetPlayerOneCoroutine);
				}
				if (playerTwo.isDead == true && _inCoroutineTwo == false && playerTwo.lives > 0)
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
	}

	void ResetPlayer(Player player)
	{
		player.Reset();
		player.SetStartingPosition(tileMap.startingTiles[(int)player.playerIndex].transform.localPosition);
		player.lives = lives;
	}
}
