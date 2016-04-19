using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Team
{
    Neutral,
    Team1,
    Team2
}

public class GameManager : MonoBehaviour
{
    #region Public Game Constants and Enums
    public const int NUM_PLAYERS = 4;
    public enum State { Error = 0, Menu, Countdown, InGame, EndGame, Tutorial, size }
    #endregion

    #region Private Members
    private static GameManager Singleton;
    private const string ES = "<b><color=red>GM Error:</color></b> ";
    private bool paused = false;
    #endregion

    #region Unity Inspector Fields
    [Header("GameManager: Status")]
    public State currstate = State.Menu;
    //inspector viewing purposes only, use the isPaused property for coding
    public bool PausedGame;

    [Header("GameManager: Settings")]
    public State sceneInitState = State.Countdown;
    public Color[] teamColors = { Color.green, Color.blue, Color.red };

    [Header("GameManager: Players")]
    public GameObject[] playersGO = new GameObject[NUM_PLAYERS];
    private Player[] players = new Player[NUM_PLAYERS];

    [Header("GameManager: Inspector Set Fields")]
    public bool useInvulnTime = false;
    public GameObject base_team1, base_team2;
    public int gameStartCountdown = 3;
    public float regenRate;
    public float regenRadius;
    #endregion

    #region Public Properties + Methods
    public static GameManager GM
    {
        get { return Singleton; }
    }

    public Color getTeamColor(Team team, bool enemyColor = false)
    {
        if(enemyColor)
        {
            if(team == Team.Team1)
            {
                return teamColors[(int)Team.Team2];
            }
            if(team == Team.Team2)
            {
                return teamColors[(int)Team.Team1];
            }
            return teamColors[(int)Team.Neutral];
        }
        return teamColors[(int)team];
    }

    public GameObject GetPlayerGO(int playerNum)
    {
        GameObject ret = null;
        if (playerNum < NUM_PLAYERS && playerNum >= 0)
        {
            ret = playersGO[playerNum];
        }
        return ret;
    }

    public Player GetPlayer(int playerNum)
    {
        Player ret = null;
        if (playerNum < NUM_PLAYERS && playerNum >= 0)
        {
            ret = players[playerNum];
        }
        return ret;
    }

    public bool isPaused
    {
        get { return paused; }
    }

    public void PauseGame()
    {
        paused = true;
        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        paused = false;
        Time.timeScale = 1;
    }

    public void StartCountDown()
    {
        currstate = State.Countdown;
    }

    public void StartTheGame()
    {
        currstate = State.InGame;
    }

    public void EndTheGame()
    {
        if (currstate == State.InGame)
        {
            currstate = State.EndGame;

            DisableAllVibration();
        }
        else
        {
            Debug.LogError(ES + "Game state is not InGame!" +
                " Current State is " + currstate + "!");
        }
    }
    #endregion

    #region Private Methods
    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        currstate = sceneInitState;

        if(currstate == State.Countdown)
        {
            for (int i = 0; i < NUM_PLAYERS; i++)
            {
                if (playersGO[i] == null)
                {
                    Debug.LogError(ES + "Player(" + i + ") is null!");
                }
                players[i] = playersGO[i].GetComponent<Player>();
                if (players[i] == null)
                {
                    Debug.LogError(ES + "All GOs must have a Player component!");
                }
            }
            StartCountDown();
        }
    }

    void Update()
    {
        PausedGame = isPaused;

        if (currstate == State.Countdown || currstate == State.EndGame)
        {
            PauseGame();
        }
        else
        {
            UnPauseGame();
        }

        if (currstate == State.EndGame)
        {
            foreach (GameObject player in playersGO)
            {
                if (player.GetComponent<Controls>().StartWasPressed)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }

        if (isPaused)
        {
            foreach (GameObject player in playersGO)
            {
                if (player.GetComponent<Controls>().SelectWasPressed)
                {
                    SceneManager.LoadScene("Menu");
                }
            }
        }
    }

    void DisableAllVibration()
    {
        foreach (GameObject player in playersGO)
        {
            player.GetComponent<Controls>().VibrateFor(0f, 0f);
        }
    }
    #endregion
}