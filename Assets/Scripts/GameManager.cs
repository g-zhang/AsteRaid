using UnityEngine;
using UnityEngine.UI;

public enum Team
{
	Neutral,
	Team1,
	Team2
}

public class GameManager : MonoBehaviour
{
	[Header("GameManager: Inspector Set Fields")]
	public bool useInvulnTime = false;
	public GameObject base_team1, base_team2;
	public Canvas UICanvas;
	bool win;

	void Start() {
		win = false;
	}

	void Update() {
		winManager ();
	}

	void winManager() {
		if ((base_team1 == null || base_team2 == null) && win == false) {
			// pause game
			Time.timeScale = 0;

			// Show WIN UI element
			if (base_team1 == null) {
				UICanvas.GetComponent<UpdateUI>().displayWin(2);
			} else if (base_team2 == null) {
				UICanvas.GetComponent<UpdateUI>().displayWin(1);
			}

			win = true;
		}

		if (win) {
			// If any player presses start, refresh scene
		}
	}
}