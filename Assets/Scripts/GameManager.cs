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
    public enum State { Error = 0, Menu, PreGame, Countdown, InGame, PostGame, EndGame, Tutorial, size }
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
    public float currEndDelay = 0f;

    [Header("GameManager: Settings")]
    public State sceneInitState = State.Countdown;
    public float gameEndDelay = 5f;
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
        if (enemyColor)
        {
            if (team == Team.Team1)
            {
                return teamColors[(int)Team.Team2];
            }
            if (team == Team.Team2)
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

    public void PausePlayers(float time)
    {
        foreach (GameObject player in playersGO)
        {
            player.GetComponent<ShipControls>().remainingStunTime = time;
        }
    }

    public void KillPlayers()
    {
        foreach (GameObject player in playersGO)
        {
            player.GetComponent<Player>().EndPlayer();
        }
    }

    public void FreezePlayers()
    {
        foreach (GameObject player in playersGO)
        {
            //player.GetComponent<Player>().DisableShip();
            player.GetComponent<Player>().enabled = false;
        }
    }

    public void StartCountDown()
    {
        if (currstate == State.PreGame)
        {
            currstate = State.Countdown;
        }
        else
        {
            Debug.LogError(ES + "Cannot start game from State:" + currstate);
        }
    }

    public void StartTheGame()
    {
        if (currstate == State.Countdown)
        {
            currstate = State.InGame;
            PausePlayers(0f);
        }
        else
        {
            Debug.LogError(ES + "Cannot start game from State:" + currstate);
        }
    }

    public void EndTheGame()
    {
        if (currstate == State.InGame)
        {
            currstate = State.PostGame;
            currEndDelay = gameEndDelay;
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
        UnPauseGame();

        if (currstate != State.Tutorial && currstate != State.Menu)
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
        }
    }

    void Update()
    {
        PausedGame = isPaused;

        //game state machine
        switch (currstate)
        {
            case State.PreGame:
                PausePlayers(1f);
                break;

            case State.Countdown:
                PausePlayers(1f);
                break;

            case State.PostGame:

                if (currEndDelay >= 0f)
                {
                    currEndDelay -= Time.deltaTime;
                    if (currEndDelay <= .5f)
                    {
                        KillPlayers();
                    }
                }
                else
                {
                    currstate = State.EndGame;
                    DisableAllVibration();
                }
                break;

            case State.EndGame:
                FreezePlayers();
                break;

            default:
                break;
        }

        if (currstate == State.EndGame || isPaused)
        {
            foreach (GameObject player in playersGO)
            {
                if (player.GetComponent<Controls>().SelectWasPressed)
                {
                    SceneManager.LoadScene("Menu");
                }
                if (player.GetComponent<Controls>().StartWasPressed && currstate == State.EndGame)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                if (player.GetComponent<Controls>().RestartWasPressed && isPaused)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }

        if (currstate == State.InGame)
        {
            foreach (GameObject player in playersGO)
            {
                if (player.GetComponent<Controls>().QuitWasPressed && isPaused)
                {
                    UnPauseGame();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (currstate == State.InGame)
        {
            foreach (GameObject player in playersGO)
            {
                if (player.GetComponent<Controls>().StartWasPressed && !isPaused)
                {
                    DisableAllVibration();
                    PauseGame();
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