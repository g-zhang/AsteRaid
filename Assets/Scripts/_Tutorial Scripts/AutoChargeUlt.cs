using UnityEngine;
using System.Collections;

public class AutoChargeUlt : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		if (other.transform.parent.GetComponent<Player> () != null) {
			other.transform.parent.GetComponent<Player> ().ultCharges = other.transform.parent.GetComponent<Player> ().chargesNeededForUlt;
			Destroy (this.gameObject);
		}
	}
}
