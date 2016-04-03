using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

    [Header("Config")]
    public float maxLen = 2f;

    HealthSystem player;
    GameObject healthBarObj;
    GameObject baseBarObj;

	// Use this for initialization
	void Start () {
        player = transform.parent.GetComponent<HealthSystem>();
        healthBarObj = transform.Find("Health").gameObject;
        baseBarObj = transform.Find("Base").gameObject;
        //maxLen = baseBarObj.transform.localScale.z;
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Vector3 scale = healthBarObj.transform.localScale;
        scale.z = player.currHealth * maxLen / player.maxHealth;
        healthBarObj.transform.localScale = scale;

        Vector3 bpos = baseBarObj.transform.localPosition;
        Vector3 bscale = baseBarObj.transform.localScale;
        bpos.x = scale.z;
        bscale.z = maxLen - scale.z;
        baseBarObj.transform.localPosition = bpos;
        baseBarObj.transform.localScale = bscale;
	}
}
