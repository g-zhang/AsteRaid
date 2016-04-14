using UnityEngine;
using System.Collections;

public class FootageCamera : MonoBehaviour {

	public float cameraSpeed;

	// Camera controls are i,j,k,l and are similar to wasd

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.I)) {
			transform.position += transform.forward * cameraSpeed;
		}
		if (Input.GetKey (KeyCode.K)) {
			transform.position -= transform.forward * cameraSpeed;
		}
		if (Input.GetKey (KeyCode.J)) {
			transform.position -= transform.right * cameraSpeed;
		}
		if (Input.GetKey (KeyCode.L)) {
			transform.position += transform.right * cameraSpeed;
		}
	}
}
