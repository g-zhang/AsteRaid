using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class Weapon_Grenade : Weapon
{
    private float nextTime;
    private Color tcolor;

	[Header("Weapon_Grenade: Inspector Set Fields")]
	public float startingSpeed = 5f;
	public float explosionSpeed = 5f;
	public float maxExplosionSize = 10f;
	public float fudgeValue = 0.1f;
    public float colorFlashSpeed = .25f;

	public float flatForceDamage = 5f;
	public float ejectForce = 1f;
	public float ejectStunTime = 0.8f;
	public float fadeTime = 2.5f;

	public Material explosionMat;

	private AudioSource audioSource;
	public AudioClip explosionClip;

	[Header("Weapon_Grenade: Dynamically Set Fields")]
    public float currFlashSpeed = 0f;
    public Rigidbody rigid;
	public Renderer rend;

	public Vector3 startPosition;
	public bool isExploding;

	public bool isFading;
	public float fadeTimeElapsed;


	void Start()
	{
		rigid = GetComponent<Rigidbody>();
		rend = GetComponent<Renderer>();
		audioSource = GetComponent<AudioSource>();

		startPosition = transform.position;
		isExploding = false;
		isFading = false;
		fadeTimeElapsed = 0f;

		rigid.velocity = startingVelocity * startingSpeed;
		startingVelocity = rigid.velocity;

        nextTime = Time.time;
        tcolor = GameManager.GM.teamColors[(int)originator.teamNumber];

		return;
	}

	void FixedUpdate()
	{
		if (isExploding)
		{
			if (isFading)
			{
				Fade();
			}
			else
			{
				Explode();
			}
		}
		else
		{
			Move();
            Controls playerControls = originator.gameObject.GetComponent<Controls>();
            if (playerControls != null)
            {
                if (playerControls.SecondFireButtonWasPressed)
                {
                    InitExplode();
                }
            }
		}

		return;
	}

    void Update()
    {
        if(!isExploding)
        {
            if(Time.time > nextTime)
            {
                nextTime += currFlashSpeed;
                if (rend.material.color == Color.black)
                {
                    rend.material.color = tcolor;
                } else
                {
                    rend.material.color = Color.black;
                }
            }
        }
    }

	void OnTriggerEnter(Collider other)
	{
		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		if (isExploding)
		{
			ExertExplosionForce(parent);
			return;
		}
		/*
		Weapon otherWeapon = parent.GetComponent<Weapon>();
		Player otherPlayer = parent.GetComponent<Player>();
		AI otherAI = parent.GetComponent<AI>();

		if (otherWeapon != null)
		{
			return;
		}
		if (otherPlayer != null)
		{
			if (otherPlayer.teamNumber == originator.teamNumber)
			{
				return;
			}
		}
		if (otherAI != null)
		{
			if (otherAI.teamNumber == originator.teamNumber)
			{
				return;
			}
		}
		*/
		ExertExplosionForce(parent);
		InitExplode();

        return;
	}

	void Explode()
	{
		rigid.velocity = Vector3.zero;

		if (transform.localScale.x >= maxExplosionSize)
		{
			isFading = true;
			GetComponent<Collider>().enabled = false;

			return;
		}

		float sizeChange = explosionSpeed * Time.fixedDeltaTime;
		transform.localScale +=
			new Vector3(sizeChange, sizeChange, sizeChange);
		
		return;
	}

	void Move()
	{
		float currentDistance =
			Vector3.Magnitude(transform.position - startPosition);
		if (currentDistance >= (maxDistance - fudgeValue))
		{
            InitExplode();
        }

		rigid.velocity = Vector3.Lerp(
			startingVelocity, Vector3.zero, currentDistance / maxDistance);

        currFlashSpeed = colorFlashSpeed * (1f - (currentDistance / maxDistance)) + .05f;

        return;
	}

	void Fade()
	{
		fadeTimeElapsed += Time.fixedDeltaTime;
		if (fadeTimeElapsed >= fadeTime)
		{
			Destroy(gameObject);
			return;
		}

		Color fadeColor = rend.material.color;
		fadeColor.a = Mathf.Lerp(explosionMat.color.a, 0f, fadeTimeElapsed / fadeTime);

		rend.material.color = fadeColor;
		return;
	}

    void InitExplode()
    {
        if(rend != null) rend.material = explosionMat;
		isExploding = true;
		audioSource.clip = explosionClip;
		audioSource.Play();
    }

	void ExertExplosionForce(Transform other)
	{
		HealthSystem otherHS = other.GetComponent<HealthSystem>();
		if (otherHS == null)
		{
			return;
		}

		otherHS.currHealth -= flatForceDamage;

		if ((otherHS is AI_Turret) || (otherHS is AI_WallTurret))
		{
			return;
		}

		other.GetComponent<Rigidbody>().AddExplosionForce(
			ejectForce, transform.position, transform.localScale.x, 0f, ForceMode.Impulse);

		ShipControls otherControls = other.GetComponent<ShipControls>();
		if (otherControls != null)
		{
			other.GetComponent<Controls>().VibrateFor(1f, 0.5f);
			otherControls.remainingStunTime = ejectStunTime;
		}

		return;
	}
}