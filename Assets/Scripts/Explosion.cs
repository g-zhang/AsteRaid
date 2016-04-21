using UnityEngine;

public class Explosion : MonoBehaviour
{
	ParticleSystem particles;

	void Awake()
	{
		particles = transform.Find("Particle").GetComponent<ParticleSystem>();
		return;
	}

	void Update()
	{
		if (!particles.isPlaying)
		{
			Destroy(gameObject);
		}
		return;
	}
}