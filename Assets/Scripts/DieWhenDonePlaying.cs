using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DieWhenDonePlaying : MonoBehaviour {

	public float threshold = 20f;
	private AudioSource src;
	void Start(){
		bool kill = true;
		foreach (GameObject ship in GameManager.GM.playersGO) {
			if ((ship.transform.position - transform.position).magnitude < threshold) {
				kill = false;
				break;
			}
		}
		if (kill) Destroy (gameObject);
		src = GetComponent<AudioSource>();
		src.Play ();
	}
	// Update is called once per frame
	void Update () {
		if (!src.isPlaying) Destroy(gameObject);
	}
}
