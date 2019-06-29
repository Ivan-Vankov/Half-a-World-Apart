using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

	public GameObject playerPrefab;

	private PlayerDamageReceived playerOneDamageReceived;
	private PlayerDamageReceived playerTwoDamageReceived;
	private PlayerTurnManager playerOnesTurn;
	private PlayerTurnManager playerTwosTurn;
	private PlayerMovement playerOneMovement;
	private PlayerMovement playerTwoMovement;

	private Vector2 SpawnPositionOne = new Vector2(0,  6);
	private Vector2 SpawnPositionTwo = new Vector2(0, -6);

	// false will be player ones turn
	//  true will be player twos turn
	private bool currentTurn = false;

	void Awake() {
		GameObject playerOne = 
			Instantiate(
				playerPrefab, 
				SpawnPositionOne, 
				Quaternion.identity);

		GameObject playerTwo =
			Instantiate(
				playerPrefab,
				SpawnPositionTwo,
				Quaternion.Euler(0, 0, 180));

		playerOnesTurn = playerOne.GetComponent<PlayerTurnManager>();
		playerTwosTurn = playerTwo.GetComponent<PlayerTurnManager>();

		playerOneDamageReceived = playerOne.GetComponent<PlayerDamageReceived>();
		playerTwoDamageReceived = playerTwo.GetComponent<PlayerDamageReceived>();

		playerOneMovement = playerOne.GetComponent<PlayerMovement>();
		playerTwoMovement = playerTwo.GetComponent<PlayerMovement>();

		playerOnesTurn.IsYourTurn = true;
	}

	private void OnGUI() {

		EndGameCheck();

		//GUIStyle style = new GUIStyle { richText = true };
		//GUILayout.Label("<size=30>Some <color=yellow>RICH</color> text</size>", style);

		GUI.Label(new Rect(20, 20, 80, 40), "Player1: " + playerOneDamageReceived.DamageReceived.ToString());
		GUI.Label(new Rect(20, Screen.height - 40, 80, 40), "Player2: " + playerTwoDamageReceived.DamageReceived.ToString());
	}

	private void EndGameCheck() {
		int player1damage = playerOneDamageReceived.DamageReceived;
		int player2damage = playerTwoDamageReceived.DamageReceived;

		int winThreshold = 49;

		if (player1damage > winThreshold && player2damage > winThreshold) {
			UnityEngine.SceneManagement.SceneManager.LoadScene("TieScene");
		}
		if (player1damage > winThreshold && player2damage < winThreshold) {
			UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerTwoWinScene");
		}
		if (player1damage < winThreshold && player2damage > winThreshold) {
			UnityEngine.SceneManagement.SceneManager.LoadScene("PlayerOneWinScene");
		}
	}

	public void SwitchTurns() {
		if (!currentTurn) {
			playerOnesTurn.IsYourTurn = false;
			playerTwosTurn.IsYourTurn = true;
			playerTwoMovement.movesLeft = PlayerMovement.movesPerTurn;
		} else {
			playerOnesTurn.IsYourTurn = true;
			playerTwosTurn.IsYourTurn = false;
			playerOneMovement.movesLeft = PlayerMovement.movesPerTurn;
		}

		currentTurn = !currentTurn;
	}
}
