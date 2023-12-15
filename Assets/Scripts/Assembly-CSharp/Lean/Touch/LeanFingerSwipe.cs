using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	public class LeanFingerSwipe : MonoBehaviour
	{
		[Serializable]
		public class FingerEvent : UnityEvent<LeanFinger>
		{
		}

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = true;

		[Tooltip("Must the swipe be in a specific direction?")]
		public bool CheckAngle;

		[Tooltip("The required angle of the swipe in degrees, where 0 is up, and 90 is right")]
		public float Angle;

		[Tooltip("The left/right tolerance of the swipe angle in degrees")]
		public float AngleThreshold = 90f;

		public FingerEvent OnFingerSwipe;

		protected void CheckSwipe(LeanFinger finger, Vector2 delta)
		{
			if (CheckAngle)
			{
				float current = Mathf.Atan2(delta.x, delta.y) * 57.29578f;
				if (Mathf.Abs(Mathf.DeltaAngle(current, Angle)) >= AngleThreshold * 0.5f)
				{
					return;
				}
			}
			OnFingerSwipe.Invoke(finger);
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerSwipe = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerSwipe, new Action<LeanFinger>(FingerSwipe));
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerSwipe = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerSwipe, new Action<LeanFinger>(FingerSwipe));
		}

		private void FingerSwipe(LeanFinger finger)
		{
			if (!IgnoreGuiFingers || !finger.StartedOverGui)
			{
				CheckSwipe(finger, finger.SwipeScreenDelta);
			}
		}
	}
}
