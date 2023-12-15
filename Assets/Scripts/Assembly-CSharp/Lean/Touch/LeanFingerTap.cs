using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	public class LeanFingerTap : MonoBehaviour
	{
		[Serializable]
		public class LeanFingerEvent : UnityEvent<LeanFinger>
		{
		}

		[Tooltip("If the finger is over the GUI, ignore it?")]
		public bool IgnoreIfOverGui;

		[Tooltip("If the finger started over the GUI, ignore it?")]
		public bool IgnoreIfStartedOverGui;

		[Tooltip("How many times must this finger tap before OnFingerTap gets called? (0 = every time)")]
		public int RequiredTapCount;

		[Tooltip("How many times repeating must this finger tap before OnFingerTap gets called? (e.g. 2 = 2, 4, 6, 8, etc) (0 = every time)")]
		public int RequiredTapInterval;

		public LeanFingerEvent OnFingerTap;

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerTap = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerTap, new Action<LeanFinger>(FingerTap));
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerTap = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerTap, new Action<LeanFinger>(FingerTap));
		}

		private void FingerTap(LeanFinger finger)
		{
			if ((!IgnoreIfOverGui || !finger.IsOverGui) && (!IgnoreIfStartedOverGui || !finger.StartedOverGui) && (RequiredTapCount <= 0 || finger.TapCount == RequiredTapCount) && (RequiredTapInterval <= 0 || finger.TapCount % RequiredTapInterval == 0))
			{
				OnFingerTap.Invoke(finger);
			}
		}
	}
}
