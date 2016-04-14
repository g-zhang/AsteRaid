using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Weapon_Bullet : Weapon
{
	[Header("Weapon_Bullet: Inspector Set Fields")]
	public float speed = 5f;
    public float lingerTime = .5f;

	[Header("Weapon_Bullet: Dynamically Set Fields")]
	public Rigidbody rigid;
	public Vector3 startPosition;
    public float currLingerTime = 0f;
    public bool isDead = false;

	void Start()
	{
		rigid = GetComponent<Rigidbody>();
		startPosition = transform.position;
		rigid.velocity = startingVelocity * speed;
        currLingerTime = lingerTime;

        GetComponent<Renderer>().material.color = GameManager.GM.teamColors[(int)originator.teamNumber];

        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail != null)
        {
            Color teamColor = GameManager.GM.teamColors[(int)originator.teamNumber];
            //trail.material.SetColor("_Color", new Color(teamColor.r, teamColor.b, teamColor.g, 1f));
            trail.material.SetColor("_EmissionColor", teamColor);
        }

		return;
	}

	void Update()
	{
		if (Vector3.Magnitude(transform.position - startPosition) > maxDistance)
		{
            isDead = true;
		}

        if(isDead)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            currLingerTime -= Time.deltaTime;

            if(currLingerTime <= 0f)
            {
                Destroy(gameObject);
            }
        }
		return;
	}

	void OnTriggerEnter(Collider other)
	{
		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

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

        isDead = true;
		return;
	}
}