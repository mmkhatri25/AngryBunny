using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	public class LeanFingerDrag : MonoBehaviour
	{
		[Serializable]
		public class LeanFingerEvent : UnityEvent<LeanFinger>
		{
		}

		[Tooltip("If the finger is over the GUI, ignore it?")]
		public bool IgnoreIfOverGui;

		[Tooltip("If the finger started over the GUI, ignore it?")]
		public bool IgnoreIfStartedOverGui;

		public LeanFingerEvent OnFingerDrag;

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerSet = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerSet, new Action<LeanFinger>(FingerSet));
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerSet = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerSet, new Action<LeanFinger>(FingerSet));
		}

		private void FingerSet(LeanFinger finger)
		{
			if ((!IgnoreIfOverGui || !finger.IsOverGui) && (!IgnoreIfStartedOverGui || !finger.StartedOverGui) && !(finger.ScreenDelta.magnitude <= 0f))
			{
				OnFingerDrag.Invoke(finger);
			}
		}
	}
}
