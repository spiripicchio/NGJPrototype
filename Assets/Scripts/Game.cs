using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
	public Player playerOne;
	public Player playerTwo;
	public TileMap tileMap;

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
			if (playerOne.isDead == true)
			{
				_resetPlayerOneCoroutine = ResetPlayerAndWait(playerOne);
				StartCoroutine(_resetPlayerOneCoroutine);
			}
			if (playerTwo.isDead == true)
			{
				_resetPlayerTwoCoroutine = ResetPlayerAndWait(playerTwo);
				StartCoroutine(_resetPlayerTwoCoroutine);
			}
		}
	}

	IEnumerator ResetPlayerAndWait(Player player)
	{
		yield return new WaitForSeconds(5);

		ResetPlayer(player);
	}

	void ResetPlayer(Player player)
	{
		player.Reset();
		player.SetStartingPosition(tileMap.startingTiles[(int)player.playerIndex].transform.localPosition);
	}
}
