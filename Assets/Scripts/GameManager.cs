using UnityEngine;

public enum Team
{
	Neutral,
	Team1,
	Team2
}

public class GameManager : MonoBehaviour
{
	[Header("GameManager: Inspector Set Fields")]
	public bool useInvulnTime = false;
}