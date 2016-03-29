using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DevTest_Player : MonoBehaviour
{
	[Header("DevTest_Player: Inspector Set Fields")]
	public int teamNumber = 0;

	[Header("DevTest_Player: Dynamically Set Fields")]
	public Rigidbody rigid;

	void Awake()
	{
		rigid = GetComponent<Rigidbody>();
		return;
	}
}