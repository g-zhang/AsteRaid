using UnityEngine;
using System.Collections.Generic;

public class ControlPoint : MonoBehaviour
{
	[Header("ControlPoint: Inspector Set General Fields")]
	public bool playersCanCapture = true;
	public bool minionsCanCapture = true;

	public Color neutralAreaColor;
	public Color team1AreaColor;
	public Color team2AreaColor;
	public float areaAlpha = 0.25f;

	public Team startingTeam = Team.Neutral;

    public float healthRegenRate = 5f;

    private GameObject captureBars;

	[Header("ControlPoint: Inspector Set Proximity Capture Fields")]
	public float captureAbsValue = 10f;
	public bool captureFlatTime = false;

	public float spectrumDriftSpeed = 1f;

	public bool anyOpposingBlocks = false;
	public bool additiveCaptureTime = true;

	[Header("ControlPoint: Dynamically Set Proximity Capture Fields")]
	public List<Player> capturingPlayers;
	public List<AI> capturingMinions;

	public float captureSpectrum;
	public float driftPoint;

	public Material mat;

	void Awake()
	{
        captureBars = transform.Find("CaptureBar").gameObject;
		mat = GetComponent<Renderer>().material;
		neutralAreaColor.a = areaAlpha;
		team1AreaColor.a = areaAlpha;
		team2AreaColor.a = areaAlpha;

		capturingPlayers = new List<Player>();
		capturingMinions = new List<AI>();

		switch (startingTeam)
		{
		case Team.Neutral:
		{
			captureSpectrum = 0f;
			driftPoint = 0f;
			mat.color = neutralAreaColor;

			break;
		}

		case Team.Team1:
		{
			captureSpectrum = Mathf.Abs(captureAbsValue);
			driftPoint = Mathf.Abs(captureAbsValue);
			mat.color = team1AreaColor;

			break;
		}

		case Team.Team2:
		{
			captureSpectrum = -Mathf.Abs(captureAbsValue);
			driftPoint = -Mathf.Abs(captureAbsValue);
			mat.color = team2AreaColor;

			break;
		}
		}

		return;
	}

	void Update()
	{
		List<HealthSystem> capturingObjects = new List<HealthSystem>();
        if(capturingPlayers.Count > 0)
        {
            for(int i = capturingPlayers.Count - 1; i >= 0; i--)
            {
                //remove dead players
                if(capturingPlayers[i].currState == Player.State.Dead)
                {
                    capturingPlayers.RemoveAt(i);
                }

                if (captureSpectrum >= captureAbsValue) // blue
                {
                    if (capturingPlayers[i].teamNumber == Team.Team1)
                    {
                        capturingPlayers[i].regenHealth(healthRegenRate, Time.deltaTime);
                    }
                }
                else if(captureSpectrum <= -captureAbsValue)
                {
                    if (capturingPlayers[i].teamNumber == Team.Team2) //red
                    {
                        capturingPlayers[i].regenHealth(healthRegenRate, Time.deltaTime);
                    }
                }
            }
        }

		if (playersCanCapture)
		{
			foreach (Player p in capturingPlayers)
			{
				capturingObjects.Add(p);
			}
		}
		if (minionsCanCapture)
		{
			foreach (AI m in capturingMinions)
			{
				capturingObjects.Add(m);
			}
		}

		if (capturingObjects.Count == 0)
		{
			if (captureSpectrum < driftPoint)
			{
				captureSpectrum += Time.deltaTime * spectrumDriftSpeed;
				if (captureSpectrum > driftPoint)
				{
					captureSpectrum = driftPoint;
				}
			}
			else if (captureSpectrum > driftPoint)
			{
				captureSpectrum -= Time.deltaTime * spectrumDriftSpeed;
				if (captureSpectrum < driftPoint)
				{
					captureSpectrum = driftPoint;
				}
			}
		}
		else
		{
			List<HealthSystem> team1 = new List<HealthSystem>();
			team1.AddRange(capturingObjects.FindAll(o => o.teamNumber == Team.Team1));

			List<HealthSystem> team2 = new List<HealthSystem>();
			team2.AddRange(capturingObjects.FindAll(o => o.teamNumber == Team.Team2));

			int team1Weight = (team1.Count == 0) ? 0 : 1;
			int team2Weight = (team2.Count == 0) ? 0 : 1;
			if (additiveCaptureTime)
			{
				team1Weight = team1.Count;
				team2Weight = team2.Count;
			}
			else
			{
				if (team1.Count > team2.Count)
				{
					team2Weight = 0;
				}
				else if (team2.Count > team1.Count)
				{
					team1Weight = 0;
				}
			}

			if (anyOpposingBlocks)
			{
				if (team2.Count == 0)
				{
					captureSpectrum += Time.deltaTime * team1Weight;
					if ((driftPoint < 0f) && (captureSpectrum > 0f))
					{
						driftPoint = 0f;
					}
				}
				else if (team1.Count == 0)
				{
					captureSpectrum -= Time.deltaTime * team2Weight;
					if ((driftPoint > 0f) && (captureSpectrum < 0f))
					{
						driftPoint = 0f;
					}
				}
			}
			else
			{
				float change = Time.deltaTime * (team1Weight - team2Weight);
				captureSpectrum += change;

				if ((change > 0f) &&
					(driftPoint < 0f) && (captureSpectrum > 0f))
				{
					driftPoint = 0f;
				}
				else if ((change < 0f) &&
					(driftPoint > 0f) && (captureSpectrum < 0f))
				{
					driftPoint = 0f;
				}
			}
		}

		if (captureSpectrum < -captureAbsValue)
		{
			captureSpectrum = -captureAbsValue;
			driftPoint = captureSpectrum;
		}
		else if (captureSpectrum > captureAbsValue)
		{
			captureSpectrum = captureAbsValue;
			driftPoint = captureSpectrum;
		}

		if (captureSpectrum > 0f)
		{
			mat.color = Color.Lerp(neutralAreaColor, team1AreaColor,
				captureSpectrum / captureAbsValue);
		}
		else if (captureSpectrum < 0f)
		{
			mat.color = Color.Lerp(neutralAreaColor, team2AreaColor,
				-captureSpectrum / captureAbsValue);
		}
		else
		{
			mat.color = neutralAreaColor;
		}
        UpdateBar();

        return;
	}

    void UpdateBar()
    {
        captureBars.GetComponent<CaptureBar>().currVal = 1f - ((captureSpectrum + captureAbsValue) / (2f * captureAbsValue));
    }

	void OnTriggerEnter(Collider other)
	{
		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		Player player = parent.GetComponent<Player>();
		AI ai = parent.GetComponent<AI>();
		
		if (player != null)
		{
			if (capturingPlayers.Find(p => p == player) == null)
			{
				capturingPlayers.Add(player);
			}
		}
		if (ai != null)
		{
			if (capturingMinions.Find(m => m == ai) == null)
			{
				capturingMinions.Add(ai);
			}
		}

		return;
	}

	void OnTriggerExit(Collider other)
	{
		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		Player player = parent.GetComponent<Player>();
		AI ai = parent.GetComponent<AI>();

		if (player != null)
		{
			if (capturingPlayers.Find(p => p == player) != null)
			{
				capturingPlayers.Remove(player);
			}
		}
		if (ai != null)
		{
			if (capturingMinions.Find(m => m == ai) != null)
			{
				capturingMinions.Remove(ai);
			}
		}

		if (captureFlatTime)
		{
			List<HealthSystem> team1 = new List<HealthSystem>();
			List<HealthSystem> team2 = new List<HealthSystem>();

			if (playersCanCapture)
			{
				foreach (Player p in capturingPlayers)
				{
					if (p.teamNumber == Team.Team1)
					{
						team1.Add(p);
					}
					else if (p.teamNumber == Team.Team2)
					{
						team2.Add(p);
					}
				}
			}
			if (minionsCanCapture)
			{
				foreach (AI m in capturingMinions)
				{
					if (m.teamNumber == Team.Team1)
					{
						team1.Add(m);
					}
					else if (m.teamNumber == Team.Team2)
					{
						team2.Add(m);
					}
				}
			}

			if ((captureSpectrum > driftPoint) && (team1.Count == 0))
			{
				captureSpectrum = driftPoint;
			}
			else if ((captureSpectrum < driftPoint) && (team2.Count == 0))
			{
				captureSpectrum = driftPoint;
			}
		}

		return;
	}
}