using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class Announcer : MonoBehaviour {

	public static Announcer announcer;
	private Queue<KeyValuePair<AudioClip, float>> clips;
	private AudioSource source;

	public GameObject T1N;
	public GameObject T1S;
	public GameObject T2N;
	public GameObject T2S;

	private LaserWallManager T1NL;
	private LaserWallManager T1SL;
	private LaserWallManager T2NL;
	private LaserWallManager T2SL;

	private bool T1Nd = false;
	private bool T1Sd = false;
	private bool T2Nd = false;
	private bool T2Sd = false;

	public AudioClip RedNorthDown;
	public AudioClip RedSouthDown;
	public AudioClip RedNorthUp;
	public AudioClip RedSouthUp;
	public AudioClip RedBaseAttack;

	public AudioClip BlueNorthDown;
	public AudioClip BlueSouthDown;
	public AudioClip BlueNorthUp;
	public AudioClip BlueSouthUp;
	public AudioClip BlueBaseAttack;

	public float expiryTime = 5f;

	// Use this for initialization
	void Awake () {
		announcer = this;
		source = GetComponent<AudioSource>();
		clips = new Queue<KeyValuePair<AudioClip,float>> ();
	}

	void Start(){
		T1NL = T1N.GetComponent<LaserWallManager> ();
		T1SL = T1S.GetComponent<LaserWallManager> ();
		T2NL = T2N.GetComponent<LaserWallManager> ();
		T2SL = T2S.GetComponent<LaserWallManager> ();
	}

	public void AddAnnouncement(AudioClip clip) {
		clips.Enqueue(new KeyValuePair<AudioClip, float> (clip, Time.realtimeSinceStartup));
	}

	// Update is called once per frame
	void Update () {

		if (!T1Nd && T1NL.isDown)
			AddAnnouncement (BlueNorthDown);
		if (!T1Sd && T1SL.isDown)
			AddAnnouncement (BlueSouthDown);
		if (!T2Nd && T2NL.isDown)
			AddAnnouncement (RedNorthDown);
		if (!T2Sd && T2SL.isDown)
			AddAnnouncement (RedSouthDown);

		T1Nd = T1NL.isDown;
		T1Sd = T1SL.isDown;
		T2Nd = T2NL.isDown;
		T2Sd = T2SL.isDown;

		if (source.isPlaying) return;
		if (clips.Count == 0) return;
		KeyValuePair<AudioClip,float> pair = clips.Dequeue ();
		while (pair.Value - Time.realtimeSinceStartup > expiryTime) {
			if (clips.Count == 0) return;
			pair = clips.Dequeue ();
		}
		source.clip = pair.Key;
		source.Play ();
	}
}
