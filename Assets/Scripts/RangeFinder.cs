﻿using System;
using UnityEngine;
using System.Collections.Generic;

public class RangeFinder : MonoBehaviour
{
	[Header("RangeFinder: Dynamically Set Fields")]
	public List<GameObject> inRange;
	public HealthSystem parentHealthSystem;

	void Awake()
	{
		inRange = new List<GameObject>();

		Transform parent = transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		AI parentAI = parent.GetComponent<AI>();
		if (parent != null)
		{
			parentHealthSystem = parentAI;
		}

		return;
	}

	// Change later?
	// Rough possible fix for player and other object death	
	void Update() {
		for (int i = inRange.Count - 1; i >= 0; i--) {
			if (inRange [i] == null) {
				inRange.RemoveAt (i);
			}
			else if (inRange [i].GetComponent<HealthSystem> ().currHealth <= 0) {
				inRange.RemoveAt (i);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		HealthSystem otherHealthSystem = parent.GetComponent<HealthSystem>();

		if (otherHealthSystem == null)
		{
			return;
		}

		if (otherHealthSystem.teamNumber == parentHealthSystem.teamNumber)
		{
			return;
		}

		if (inRange.Find(go => go == parent.gameObject) != null)
		{
			return;
		}

		inRange.Add(parent.gameObject);
		otherHealthSystem.OnDeathEvent += OnInRangeDeath;

		return;
	}

	void OnTriggerExit(Collider other)
	{
		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		HealthSystem otherHealthSystem = parent.GetComponent<HealthSystem>();

		if (otherHealthSystem == null)
		{
			return;
		}

		if (inRange.Find(go => go == parent.gameObject) == null)
		{
			return;
		}

		inRange.Remove(parent.gameObject);
		otherHealthSystem.OnDeathEvent -= OnInRangeDeath;

		return;
	}

	void OnInRangeDeath(object sender, EventArgs e)
	{
		HealthSystem senderHealth = sender as HealthSystem;
		if (senderHealth == null)
		{
			return;
		}

		Transform parent = senderHealth.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		if (inRange.Find(go => go == parent.gameObject) == null)
		{
			return;
		}

		inRange.Remove(parent.gameObject);
		return;
	}
}