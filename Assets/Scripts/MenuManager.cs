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
				print ("HELP");
                SceneManager.LoadScene("Tutorial");
            }
            if (player.GetComponent<Controls>().BoostButtonWasPressed)
            {
                print("quit");
                Application.Quit();
            }
        }
    }
}
