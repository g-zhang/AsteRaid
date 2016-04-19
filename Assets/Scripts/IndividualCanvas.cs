using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IndividualCanvas : MonoBehaviour {


	public Player player;
	public GameObject go;
	public ShipControls shipControls;

	public Image RBIcon;
	public Image Bomb1Icon;
	public Image Bomb2Icon;
	public Image Bomb3Icon;
	public Slider UltSlider;
	public Slider BoostSlider;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Bomb3Icon.enabled = (player.grenadeAmmo >= 3);
		Bomb2Icon.enabled = (player.grenadeAmmo >= 2);
		Bomb1Icon.enabled = (player.grenadeAmmo >= 1);

	}
}
