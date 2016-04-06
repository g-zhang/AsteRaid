using UnityEngine;

public class AI_Turret : AI
{
	[Header("AI_Turret: Inspector Set Fields")]
	public float rotationSpeed = 5f;
	public GameObject weaponPrefab;

	public float startFiringDelay = 0.5f;

	public Material neutralMat;
	public Material team1Mat;
	public Material team2Mat;

	[Header("AI_Turret: Dynamically Set Fields")]
	public Transform gun;
	public float coolDownRemaining;

	public Renderer bodyRend;
	public Renderer gunRend;
	public Team rendTeam;

	protected override void OnAwake()
	{
		gun = transform.Find("Gun");
		coolDownRemaining = startFiringDelay;

		bodyRend = transform.Find("Body").GetComponent<Renderer>();
		gunRend = gun.Find("GunBody").GetComponent<Renderer>();

		ChangeColor();

		base.OnAwake();
		return;
	}

	protected override void DoOnUpdate()
	{
		if (rendTeam != teamNumber)
		{
			ChangeColor();
		}

		coolDownRemaining -= Time.deltaTime;
		if (coolDownRemaining < 0f)
		{
			coolDownRemaining = 0f;
		}

		if (range.inRange.Count == 0)
		{
			coolDownRemaining = startFiringDelay;
			return;
		}

		if (range.inRange[0] == null) return;

		Vector3 direction =
			range.inRange[0].transform.position - transform.position;
		Quaternion targetRotation =
			Quaternion.LookRotation(direction, Vector3.up);
		gun.rotation = Quaternion.Slerp(
			gun.rotation, targetRotation, Time.deltaTime * rotationSpeed);
		
		if (coolDownRemaining == 0f)
		{
			GameObject weapon = Instantiate(weaponPrefab);
			weapon.transform.position = gun.position;

			Weapon weaponComp = weapon.GetComponent<Weapon>();
			weaponComp.startingVelocity = gun.forward;
			weaponComp.teamNumber = teamNumber;

			coolDownRemaining += weaponComp.coolDownTime;
		}

		return;
	}

	void ChangeColor()
	{
		switch (teamNumber)
		{
		case Team.Neutral:
		{
			bodyRend.material = neutralMat;
			gunRend.material = neutralMat;
			break;
		}

		case Team.Team1:
		{
			bodyRend.material = team1Mat;
			gunRend.material = team1Mat;
			break;
		}

		case Team.Team2:
		{
			bodyRend.material = team2Mat;
			gunRend.material = team2Mat;
			break;
		}
		}

		rendTeam = teamNumber;
		return;
	}
}