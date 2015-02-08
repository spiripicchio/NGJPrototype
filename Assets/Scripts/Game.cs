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
	public int maxScore;
	public Text message;
	public Text score1Text;
	public Text score2Text;
	public Text title;
	public Text credits1;
	public Text credits2;
	public Text instructions;
	public AudioClip lost1;
	public AudioClip lost2;
	public AudioClip winRound;
	public AudioSource soundtrack;

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
	bool _startGame;

	// Use this for initialization
	void Start() 
	{
		StartCoroutine(MainLoop());
	}

	IEnumerator MainLoop()
	{
		while (_startGame == false)
		{
			yield return null;
		}

		title.text = "";
		credits1.text = "";
		credits2.text = "";
		instructions.text = "";

		yield return new WaitForSeconds(0.8f);

		score1Text.text = "0";
		score2Text.text = "0";

		soundtrack.Play();

		while (true)
		{
			Reset();

			playerOne.isEnabled = true;
			playerTwo.isEnabled = true;

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

			yield return new WaitForSeconds(1.5f);

			// Outro
			switch (_winningPlayer)
			{
				case WinningPlayer.PlayerOne:
				{
					message.text = "Player One Wins!";
					++_score1;
					score1Text.text = _score1.ToString();

					AudioSource.PlayClipAtPoint(winRound, Vector3.zero);

					break;
				}

				case WinningPlayer.PlayerTwo:
				{
					message.text = "Player Two Wins!";
					++_score2;
					score2Text.text = _score2.ToString();

					AudioSource.PlayClipAtPoint(winRound, Vector3.zero);
				
					break;
				}

				case WinningPlayer.None:
				{
					message.text = "Everyone Loses!";

					AudioSource.PlayClipAtPoint(Random.Range(0, 2) == 0 ? lost1 : lost2, Vector3.zero);
				
					break;
				}
			}

			playerOne.isEnabled = false;
			playerTwo.isEnabled = false;
			
			tileMap.FadePits();
			
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

		message.text = "";
	}

	// Update is called once per frame
	void Update() 
	{
		if (Input.GetButtonDown("Jump") == true)
		{
			if (_startGame == true)
			{
				_score1 = 0;
				score1Text.text = "0";
				_score2 = 0;
				score2Text.text = "0";
			}

			_startGame = true;
		}

		if (_winningPlayer == WinningPlayer.Unknown)
		{
			if (playerOne.isDead == true && playerTwo.isDead == true)
			{
				_winningPlayer = WinningPlayer.None;
			}
			else if (playerOne.reachedGoal == true)
			{
				_winningPlayer = WinningPlayer.PlayerOne; 
			}
			else if (playerTwo.reachedGoal == true)
			{
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
