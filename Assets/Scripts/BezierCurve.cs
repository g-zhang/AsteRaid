using UnityEngine;
using System.Collections;

public class BezierCurve {

	private readonly Vector3[] controlPoints;

	public BezierCurve (BezierCurve b) {
		controlPoints = new Vector3[b.controlPoints.Length];
		b.controlPoints.CopyTo(controlPoints, 0);
	}

	public BezierCurve(params Vector3[] pts) {
		controlPoints = pts;
	}

	public Vector3 getPoint(float u) {
		return evaluateBezier(u, controlPoints);
	}

	private Vector3 evaluateBezier(float u, Vector3[] points) {
		if (points.Length == 0) return Vector3.zero;
		if (points.Length == 1) return points[0];
		Vector3[] nextPoints = new Vector3[points.Length - 1];
		for (int i=0; i < points.Length - 1; i++)
			nextPoints[i] = u * points[i+1] + (1 - u) * points[i];
		return evaluateBezier(u, nextPoints);
	}
}
