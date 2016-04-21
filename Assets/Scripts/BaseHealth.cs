using UnityEngine;
using System.Collections;

public class BaseHealth : HealthSystem {

    public bool isDestroyed = false;

	private float announcementCooldownMax = 30f;
	private float announcementCooldownRemaining = 0f;

	protected override void DoOnUpdate(){
		announcementCooldownRemaining -= Time.deltaTime;
		if (announcementCooldownRemaining < 0f)
			announcementCooldownRemaining = 0f;
	}

	protected override void DoOnDamage(){
		if (announcementCooldownRemaining == 0f) {
			if (teamNumber == Team.Team1)
				Announcer.announcer.AddAnnouncement (Announcer.announcer.BlueBaseAttack);
			else if (teamNumber == Team.Team2)
				Announcer.announcer.AddAnnouncement (Announcer.announcer.RedBaseAttack);
			announcementCooldownRemaining = announcementCooldownMax;
		}
	}

	protected override void OnAwake(){
		if (teamNumber == Team.Team1)
			gameObject.layer = LayerMask.NameToLayer ("BlueBase");
		if (teamNumber == Team.Team2)
			gameObject.layer = LayerMask.NameToLayer ("RedBase");
	}

    public override void DeathProcedure()
    {
        if(!isDestroyed)
        {
            isDestroyed = true;
            GetComponent<MeshRenderer>().enabled = false;
			GetComponent<Collider>().enabled = false;
			if (deathExplosion != null) {
				Instantiate (MusicMan.MM.baseExplosionSoundSource, transform.position, Quaternion.identity);
				GameObject explosion = Instantiate (deathExplosion) as GameObject;
				explosion.transform.position = transform.position;
			}

            //begin end game stuff
            GameManager.GM.EndTheGame();
        }
    }
}
