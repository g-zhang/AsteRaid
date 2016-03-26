using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	Queue<MoveFrame> queuedFrames;
	Queue<MoveFrame> activeFrames;
	MoveFrame currentFrame;
	float timeInCurrentFrame = 0f;
	float timePerFrame;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame

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
