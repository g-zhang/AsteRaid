using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {
	public bool[] playerReady = new bool[GameManager.NUM_PLAYERS];
	public GameObject[] players = new GameObject[GameManager.NUM_PLAYERS];

	public float gameStartDelay;
	float timeTilGameStart;

	public static TutorialManager TM;

	void Awake() {TM = this;}

	void Start() {
		timeTilGameStart = gameStartDelay;

		for (int i = 0; i < playerReady.Length; i++) {
			playerReady [i] = false;
		}
	}

	void FixedUpdate() {
		// Check if any player skips tutorial
		for (int i = 0; i < GameManager.NUM_PLAYERS; i++) {
			if (playerStartCheck (i) && !playerReady[i]) {
				playerReady [i] = true;
				GameManager.GM.GetPlayerGO (i).GetComponent<Player> ().currHealth = 0;
			}
		}
	}

	void Update () {	
		if (allReady ()) {
			// do ready stuff
			// move to next scene
			timeTilGameStart -= Time.deltaTime;
			if (timeTilGameStart <= 0f)
				SceneManager.LoadScene("TheOctagon");
		}
	}

	bool allReady() {
		for (int i = 0; i < playerReady.Length; i++) {
			if (!playerReady [i])
				return false;
		}
		return true;
	}

	bool playerStartCheck(int i) {
		return GameManager.GM.playersGO [i].GetComponent<Controls> ().StartWasPressed;
	}
}
