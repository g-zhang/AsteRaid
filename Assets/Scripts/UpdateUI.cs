using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour {

	/*
    public GameObject Player0;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
	*/

    public GameObject Base1;
    public GameObject Base2;

	/*
    public Text Player0Text;
    public Text Player1Text;
    public Text Player2Text;
    public Text Player3Text;
	*/

    public Text Base1Text;
    public Text Base2Text;

	public int maxCountdown;
	int currCountdown;
	float loadLevelTime;

    // Use this for initialization
    void Start () {
		currCountdown = maxCountdown;
		loadLevelTime = Time.unscaledTime;
	}
	
	// Update is called once per frame
	void Update () {
		/*
        Player0Text.text = "P1 HP: " + Player0.GetComponent<HealthSystem>().currHealth;
        Player1Text.text = "P2 HP: " + Player1.GetComponent<HealthSystem>().currHealth;
        Player2Text.text = "P3 HP: " + Player2.GetComponent<HealthSystem>().currHealth;
        Player3Text.text = "P4 HP: " + Player3.GetComponent<HealthSystem>().currHealth;
		*/

        Base1Text.text = "Blue Base HP: " + Base1.GetComponent<BaseHealth>().currHealth;
        Base2Text.text = "Red Base HP: " + Base2.GetComponent<BaseHealth>().currHealth;


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
		Time.timeScale = 0;

		currCountdown = (int)Mathf.Ceil (maxCountdown - (Time.unscaledTime - loadLevelTime));

		transform.Find ("CountdownText").gameObject.GetComponent<Text> ().text = currCountdown.ToString();

		if (currCountdown <= 0) {
			Time.timeScale = 1;
			transform.Find ("CountdownText").gameObject.SetActive (false);
		}
	}
}
