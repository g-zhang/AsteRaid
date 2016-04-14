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
	public bool canChangeTeam = false;

	protected override void OnAwake()
	{

		SetTeamLayer();

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

        GameObject target = null;
        if(range.inRange.Count > 0) GetTarget(range.inRange);
        if (target == null)
        {
            coolDownRemaining = startFiringDelay;
            return;
        }

        Vector3 direction =
			target.transform.position - transform.position;
		Quaternion targetRotation =
			Quaternion.LookRotation(direction, Vector3.up);
		gun.rotation = Quaternion.Slerp(
			gun.rotation, targetRotation, Time.deltaTime * rotationSpeed);
		
		if (coolDownRemaining == 0f)
		{
			GameObject weapon = Instantiate(weaponPrefab);
			weapon.transform.position = gun.position;

			if (teamNumber == Team.Team1) {
				weapon.layer = LayerMask.NameToLayer ("BlueWeapon");
			} else if (teamNumber == Team.Team2) {
				weapon.layer = LayerMask.NameToLayer ("RedWeapon");
			}

			Weapon weaponComp = weapon.GetComponent<Weapon>();
			weaponComp.startingVelocity = gun.forward;
			weaponComp.originator = this;

			coolDownRemaining += weaponComp.coolDownTime;
		}

		return;
	}

	void ChangeColor()
	{
		SetTeamLayer();

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



	void SetTeamLayer() {

		if (teamNumber == Team.Team1) {
			gameObject.layer = LayerMask.NameToLayer("BlueTurret");
			foreach (Transform tr in transform) {
				if (tr.gameObject.name == "Range")
					tr.gameObject.layer = LayerMask.NameToLayer("BlueWeapon");
				else {
					tr.gameObject.layer = LayerMask.NameToLayer("BlueTurret");
					foreach (Transform tr2 in tr) {
						tr2.gameObject.layer = LayerMask.NameToLayer("BlueTurret");
					}
				}
			}
		}

		if (teamNumber == Team.Team2) {
			gameObject.layer = LayerMask.NameToLayer("RedTurret");
			foreach (Transform tr in transform) {
				if (tr.gameObject.name == "Range")
					tr.gameObject.layer = LayerMask.NameToLayer("RedWeapon");
				else {
					tr.gameObject.layer = LayerMask.NameToLayer("RedTurret");
					foreach (Transform tr2 in tr) {
						tr2.gameObject.layer = LayerMask.NameToLayer("RedTurret");
					}
				}
			}
		}
	}
}