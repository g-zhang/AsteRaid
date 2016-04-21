using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour {

    public GameObject Base1;
    public GameObject Base2;
	public Slider Base1HP;
	public Slider Base2HP;

    public Slider BlueNorthSlider;
    public Slider BlueSouthSlider;
    public Slider RedNorthSlider;
    public Slider RedSouthSlider;

    public LaserWallManager T1NL;
    public LaserWallManager T1SL;
    public LaserWallManager T2NL;
    public LaserWallManager T2SL;

    public int maxCountdown;
	int currCountdown;
	float loadLevelTime;

    // Use this for initialization
    void Start () {

		currCountdown = maxCountdown;
		loadLevelTime = Time.unscaledTime;

		Base1HP.maxValue = Base1.GetComponent<BaseHealth> ().maxHealth;
		Base2HP.maxValue = Base2.GetComponent<BaseHealth> ().maxHealth;

        T1NL = Announcer.announcer.T1N.GetComponent<LaserWallManager>();
        T1SL = Announcer.announcer.T1S.GetComponent<LaserWallManager>();
        T2NL = Announcer.announcer.T2N.GetComponent<LaserWallManager>();
        T2SL = Announcer.announcer.T2S.GetComponent<LaserWallManager>();
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

            if(GameManager.GM.currstate == GameManager.State.EndGame)
            {
                transform.Find("RestartText").gameObject.SetActive(true);
            }

            if(GameManager.GM.currstate == GameManager.State.InGame)
            {
                if(T1NL.isDown)
                {
                    BlueNorthSlider.value = 1f - T1NL.currRespawnTime / T1NL.respawnTime;
                } else
                {
                    if(T1NL.wallIcon.activeSelf)
                    {
                        BlueNorthSlider.value = 1f;
                    } else
                    {
                        BlueNorthSlider.value = .5f;
                    }  
                }

                if (T1SL.isDown)
                {
                    BlueSouthSlider.value = 1f - T1SL.currRespawnTime / T1SL.respawnTime;
                }
                else
                {
                    if (T1SL.wallIcon.activeSelf)
                    {
                        BlueSouthSlider.value = 1f;
                    }
                    else
                    {
                        BlueSouthSlider.value = .5f;
                    }
                }

                if (T2NL.isDown)
                {
                    RedNorthSlider.value = 1f - T2NL.currRespawnTime / T2NL.respawnTime;
                }
                else
                {
                    if (T2NL.wallIcon.activeSelf)
                    {
                        RedNorthSlider.value = 1f;
                    }
                    else
                    {
                        RedNorthSlider.value = .5f;
                    }
                }

                if (T2SL.isDown)
                {
                    RedSouthSlider.value = 1f - T2SL.currRespawnTime / T2SL.respawnTime;
                }
                else
                {
                    if (T2SL.wallIcon.activeSelf)
                    {
                        RedSouthSlider.value = 1f;
                    }
                    else
                    {
                        RedSouthSlider.value = .5f;
                    }
                }
            }
        }

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
