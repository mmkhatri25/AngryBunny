using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lean.Touch
{
	public class LeanTouchEvents : MonoBehaviour
	{
		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerDown, new Action<LeanFinger>(OnFingerDown));
			LeanTouch.OnFingerSet = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerSet, new Action<LeanFinger>(OnFingerSet));
			LeanTouch.OnFingerUp = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerUp, new Action<LeanFinger>(OnFingerUp));
			LeanTouch.OnFingerTap = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerTap, new Action<LeanFinger>(OnFingerTap));
			LeanTouch.OnFingerSwipe = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerSwipe, new Action<LeanFinger>(OnFingerSwipe));
			LeanTouch.OnGesture = (Action<List<LeanFinger>>)Delegate.Combine(LeanTouch.OnGesture, new Action<List<LeanFinger>>(OnGesture));
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerDown, new Action<LeanFinger>(OnFingerDown));
			LeanTouch.OnFingerSet = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerSet, new Action<LeanFinger>(OnFingerSet));
			LeanTouch.OnFingerUp = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerUp, new Action<LeanFinger>(OnFingerUp));
			LeanTouch.OnFingerTap = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerTap, new Action<LeanFinger>(OnFingerTap));
			LeanTouch.OnFingerSwipe = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerSwipe, new Action<LeanFinger>(OnFingerSwipe));
			LeanTouch.OnGesture = (Action<List<LeanFinger>>)Delegate.Remove(LeanTouch.OnGesture, new Action<List<LeanFinger>>(OnGesture));
		}

		public void OnFingerDown(LeanFinger finger)
		{
			Debug.Log("Finger " + finger.Index + " began touching the screen");
		}

		public void OnFingerSet(LeanFinger finger)
		{
			Debug.Log("Finger " + finger.Index + " is still touching the screen");
		}

		public void OnFingerUp(LeanFinger finger)
		{
			Debug.Log("Finger " + finger.Index + " finished touching the screen");
		}

		public void OnFingerTap(LeanFinger finger)
		{
			Debug.Log("Finger " + finger.Index + " tapped the screen");
		}

		public void OnFingerSwipe(LeanFinger finger)
		{
			Debug.Log("Finger " + finger.Index + " swiped the screen");
		}

		public void OnGesture(List<LeanFinger> fingers)
		{
			Debug.Log("Gesture with " + fingers.Count + " finger(s)");
			Debug.Log("    pinch scale: " + LeanGesture.GetPinchScale(fingers));
			Debug.Log("    twist degrees: " + LeanGesture.GetTwistDegrees(fingers));
			Debug.Log("    twist radians: " + LeanGesture.GetTwistRadians(fingers));
			Debug.Log("    screen delta: " + LeanGesture.GetScreenDelta(fingers));
		}
	}
}
