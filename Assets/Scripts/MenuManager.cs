using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public GameObject[] playersGO = new GameObject[GameManager.NUM_PLAYERS];

    // Use this for initialization
    void Start () {
        Time.timeScale = 1f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        foreach (GameObject player in playersGO)
        {
            if (player.GetComponent<Controls>().StartWasPressed)
            {
				SceneManager.LoadScene("Loading(MenuToTutorial)");
            }
            if(player.GetComponent<Controls>().UltButtonWasPressed)
            {
                SceneManager.LoadScene("Loading(TutorialToOctagon)");
            }
            if (player.GetComponent<Controls>().QuitWasPressed)
            {
                Debug.Log("<b><color=red>Quitting Application.</color></b>");
                Application.Quit();
            }
        }
    }
}
