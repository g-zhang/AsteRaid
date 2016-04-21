using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IndividualCanvas : MonoBehaviour {

	public GameObject go;
	public Player player;
	public ShipControls shipControls;

	public Image RBIcon;
	public Image Bomb1Icon;
	public Image Bomb2Icon;
	public Image Bomb3Icon;
	public Slider UltSlider;
	public Slider BoostSlider;
	public Slider HealthSlider;
	public Image tunnelVision;

	public float RBFlipTime = 0.5f;
	private float timeSinceRBFlip = 0f;

	// Use this for initialization
	void Start () {
		player = go.GetComponent<Player>();
		shipControls = go.GetComponent<ShipControls>();
		UltSlider.maxValue = player.chargesNeededForUlt;
		BoostSlider.maxValue = shipControls.boostCooldownTime;
		HealthSlider.maxValue = player.maxHealth;
	}
	
	// Update is called once per frame
	void Update () {

		Color tvColor = tunnelVision.color;
		tvColor.a = player.damageLayerOpaque;
		tunnelVision.color = tvColor;

		Bomb3Icon.enabled = (player.grenadeAmmo >= 3);
		Bomb2Icon.enabled = (player.grenadeAmmo >= 2);
		Bomb1Icon.enabled = (player.grenadeAmmo >= 1);
		UltSlider.value = player.ultCharges;
		BoostSlider.value = shipControls.boostCooldownTime - shipControls.boostCooldownRemaining;
		HealthSlider.value = player.currHealth;

		timeSinceRBFlip += Time.deltaTime;
		bool flip = false;
		if (timeSinceRBFlip >= RBFlipTime) {
			timeSinceRBFlip = 0f;
			flip = true;
		}

		if (player.ultCharges < player.chargesNeededForUlt) {
			RBIcon.enabled = false;
		} else if (flip){
			RBIcon.enabled = !RBIcon.enabled;
		}
	}
}
