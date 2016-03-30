using UnityEngine;
using System.Collections;

public class MoveFrame {

	public MoveFrame( BezierCurve b) {
		motion = b;
	}

	public MoveFrame (MoveFrame m) {
		motion = new BezierCurve(m.motion);
		startPosition = m.startPosition;
	}

	public MoveFrame() { }

	public BezierCurve motion;
	public Vector3 startPosition;

	public Vector3 endPosition {
		get {
			return startPosition + motion.getPoint(1f);
		}
	}
	
}
