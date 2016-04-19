using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {
	public Canvas T1P1, T1P2, T2P1, T2P2;
	public bool[] playerReady = new bool[GameManager.NUM_PLAYERS];
	public GameObject[] players = new GameObject[GameManager.NUM_PLAYERS];

	void Start() {
		for (int i = 0; i < playerReady.Length; i++) {
			playerReady [i] = false;
		}
	}

	void Update () {
		if (allReady ()) {
			// do ready stuff
			// move to next scene
		}

	}

	bool allReady() {
		for (int i = 0; i < playerReady.Length; i++) {
			if (!playerReady [i])
				return false;
		}
		return true;
	}
}
