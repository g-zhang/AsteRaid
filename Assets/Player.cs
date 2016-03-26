using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	Queue<MoveFrame> queuedFrames = new Queue<MoveFrame>();
	Queue<MoveFrame> activeFrames = new Queue<MoveFrame>();
	MoveFrame currentFrame = null;
	float timeInCurrentFrame = 0f;
	float timePerFrame = 1f;

	void Start() {
		MoveFrame testFrame1 = new MoveFrame();
		testFrame1.motion = new BezierCurve(
			new Vector3[] {
				Vector3.zero,
				Vector3.up,
				Vector3.right
			});
		QueueFrame(testFrame1);
		ActivateFrames();
	}

	void Update () {
		if (currentFrame == null) {
			NextFrame();
			return;
		}
		float u = timeInCurrentFrame / timePerFrame;
		Vector3 posiitonInFrame = currentFrame.motion.getPoint(u);
		transform.position = currentFrame.startPosition + posiitonInFrame;
		timeInCurrentFrame += Time.deltaTime;
		if (timeInCurrentFrame > timePerFrame) currentFrame = null;
	}

	private void NextFrame() {
		try { currentFrame = activeFrames.Dequeue(); }
		catch { currentFrame = null; return; }
		timeInCurrentFrame = 0f;
	}

	public void ActivateFrames() {
		while (true) {
			try {
				activeFrames.Enqueue(queuedFrames.Dequeue());
			} catch {
				break;
			}
		}
	}

	public void QueueFrame(MoveFrame frame) {
		frame.startPosition = endPointOfFrames;
		queuedFrames.Enqueue(frame);
	}

	private Vector3 endPointOfFrames {
		get {
			if (lastFrame == null) return transform.position;
			return lastFrame.endPosition;
		}
	}

	private MoveFrame lastFrame {
		get {
			if (queuedFrames.Count > 0)
				return queuedFrames.ToArray()[queuedFrames.Count - 1];
			if (activeFrames.Count > 0)
				return activeFrames.ToArray()[activeFrames.Count - 1];
			if (currentFrame != null) return currentFrame;
			return null;
		}
	}
}
