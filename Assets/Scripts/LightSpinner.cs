using UnityEngine;
using System.Collections;

public class LightSpinner : MonoBehaviour {

	public float spinSpeed = 1f;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(spinSpeed * Time.deltaTime, 0f, 0f);
	}
}
