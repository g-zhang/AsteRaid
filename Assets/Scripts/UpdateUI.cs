﻿using UnityEngine;
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


	public Slider Player0Boost;
	public Slider Player1Boost;
	public Slider Player2Boost;
	public Slider Player3Boost;

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

		Player0SC = Player0.GetComponent<ShipControls>();
		Player1SC = Player1.GetComponent<ShipControls>();
		Player2SC = Player2.GetComponent<ShipControls>();
		Player3SC = Player3.GetComponent<ShipControls>();

		Player0Boost.maxValue = Player0SC.maxBoostTime;
		Player1Boost.maxValue = Player1SC.maxBoostTime;
		Player2Boost.maxValue = Player2SC.maxBoostTime;
		Player3Boost.maxValue = Player3SC.maxBoostTime;

	}
	
	// Update is called once per frame
	void Update () {

		Base1Text.text = "Blue Base HP: " + Base1.GetComponent<BaseHealth>().currHealth;
        Base2Text.text = "Red Base HP: " + Base2.GetComponent<BaseHealth>().currHealth;

		Player0Boost.value = Player0SC.boostTime;
		Player1Boost.value = Player1SC.boostTime;
		Player2Boost.value = Player2SC.boostTime;
		Player3Boost.value = Player3SC.boostTime;

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
