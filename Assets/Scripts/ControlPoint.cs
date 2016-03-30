using UnityEngine;
using System.Collections.Generic;

public class ControlPoint : MonoBehaviour
{
	[Header("ControlPoint: Inspector Set Fields")]
	public float captureAbsValue = 10f;
	public bool captureFlatTime = false;

	public float spectrumDriftSpeed = 0f;

	public bool anyOpposingBlocks = false;
	public bool additiveCaptureTime = false;

	public Color neutralColor;
	public Color team1Color;
	public Color team2Color;
	public float alpha = 0.25f;

	[Header("Control Point: Dynamically Set Fields")]
	public List<DevTest_Player> capturingObjects;

	public float captureSpectrum;
	public float driftPoint;

	public Material mat;

	void Awake()
	{
		mat = GetComponent<MeshRenderer>().material;
		neutralColor.a = alpha;
		team1Color.a = alpha;
		team2Color.a = alpha;

		capturingObjects = new List<DevTest_Player>();
		captureSpectrum = 0f;
		driftPoint = 0f;

		mat.color = neutralColor;

		return;
	}

	void Update()
	{
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
			List<DevTest_Player> team1 =
				capturingObjects.FindAll(p => p.teamNumber == 1);
			List<DevTest_Player> team2 =
				capturingObjects.FindAll(p => p.teamNumber == 2);

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
			mat.color = Color.Lerp(neutralColor, team1Color,
				captureSpectrum / captureAbsValue);
		}
		else if (captureSpectrum < 0f)
		{
			mat.color = Color.Lerp(neutralColor, team2Color,
				-captureSpectrum / captureAbsValue);
		}
		else
		{
			mat.color = neutralColor;
		}

		return;
	}
	
	void OnTriggerEnter(Collider other)
	{
		DevTest_Player player = other.GetComponent<DevTest_Player>();
		if (player == null)
		{
			return;
		}

		if (capturingObjects.Find(p => p == player) != null)
		{
			return;
		}

		capturingObjects.Add(player);
		return;
	}

	void OnTriggerExit(Collider other)
	{
		DevTest_Player player = other.GetComponent<DevTest_Player>();
		if (player == null)
		{
			return;
		}

		if (capturingObjects.Find(p => p == player) == null)
		{
			return;
		}

		capturingObjects.Remove(player);

		if (captureFlatTime)
		{
			if ((captureSpectrum > driftPoint) &&
				(capturingObjects.Find(p => p.teamNumber == 1) == null))
			{
				captureSpectrum = driftPoint;
			}
			else if ((captureSpectrum < driftPoint) &&
				(capturingObjects.Find(p => p.teamNumber == 2) == null))
			{
				captureSpectrum = driftPoint;
			}
		}

		return;
	}
}