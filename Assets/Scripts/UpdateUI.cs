using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour {

    public GameObject Player0;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;

    public GameObject Base1;
    public GameObject Base2;

	public Text Player0Text;
	public Text Player1Text;
	public Text Player2Text;
	public Text Player3Text;

	public Text Player0Nades;
	public Text Player1Nades;
	public Text Player2Nades;
	public Text Player3Nades;

	public Slider Player0Boost;
	public Slider Player1Boost;
	public Slider Player2Boost;
	public Slider Player3Boost;

	public Slider Player0Ult;
	public Slider Player1Ult;
	public Slider Player2Ult;
	public Slider Player3Ult;

	public Slider Base1HP;
	public Slider Base2HP;

	public ShipControls Player0SC;
	public ShipControls Player1SC;
	public ShipControls Player2SC;
	public ShipControls Player3SC;

	public Text Base1Text;
    public Text Base2Text;

	public int maxCountdown;
	int currCountdown;
	float loadLevelTime;

    // Use this for initialization
    void Start () {

		currCountdown = maxCountdown;
		loadLevelTime = Time.unscaledTime;

		Base1HP.maxValue = Base1.GetComponent<BaseHealth> ().maxHealth;
		Base2HP.maxValue = Base2.GetComponent<BaseHealth> ().maxHealth;

	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.GM != null)
        {
            if (GameManager.GM.currstate == GameManager.State.Countdown)
            {
                maxCountdown = GameManager.GM.gameStartCountdown;
                countdown();
            }

            // Show WIN UI element
            if (GameManager.GM.base_team1.GetComponent<BaseHealth>().isDestroyed)
            {
                displayWin(2);
            }
            else if (GameManager.GM. base_team2.GetComponent<BaseHealth>().isDestroyed)
            {
                displayWin(1);
            }
        }

        Base1Text.text = "Blue Base HP: " + Base1.GetComponent<BaseHealth>().currHealth;
        Base2Text.text = "Red Base HP: " + Base2.GetComponent<BaseHealth>().currHealth;

		Base1HP.value = Base1.GetComponent<BaseHealth> ().currHealth;
		Base2HP.value = Base2.GetComponent<BaseHealth> ().currHealth;

		if (currCountdown > 0) {
			countdown ();
		}
    }

	public void displayWin(int winningTeam) {
		GameObject wintext = transform.Find ("WinText").gameObject;
		wintext.SetActive (true);

		if (winningTeam == 1) {
			wintext.GetComponent<Text> ().color = Color.blue;
		} else if (winningTeam == 2) {
			wintext.GetComponent<Text> ().color = Color.red;
		}

		wintext.GetComponent<Text>().text = "Team " + winningTeam + " WINS";
	}

	public void countdown() {
		currCountdown = (int)Mathf.Ceil (maxCountdown - (Time.unscaledTime - loadLevelTime));

        transform.Find("CountdownText").gameObject.SetActive(true);
        transform.Find ("CountdownText").gameObject.GetComponent<Text> ().text = currCountdown.ToString();

		if (currCountdown <= 0) {
            GameManager.GM.StartTheGame();
			transform.Find ("CountdownText").gameObject.SetActive (false);
		}
	}
}
