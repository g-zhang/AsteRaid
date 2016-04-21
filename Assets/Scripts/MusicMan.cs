using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicMan : MonoBehaviour {

	public AudioClip bullet;
	public AudioClip explosion;
	public AudioClip laser;
	public AudioSource source;

	public GameObject damageSoundSource;
	public GameObject droneBulletSoundSource;
	public GameObject turretBulletSoundSource;
	public GameObject ultimateSoundSource;
	public GameObject baseExplosionSoundSource;

	public static MusicMan MM;

	void Awake(){
		MM = this;
	}

	void Start(){
		source = GetComponent<AudioSource> ();
	}

	public void playClip(AudioClip clip){
		source.clip = clip;
		source.Play();
	}
}
