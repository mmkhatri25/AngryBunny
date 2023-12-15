using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	public class LeanFingerUp : MonoBehaviour
	{
		[Serializable]
		public class LeanFingerEvent : UnityEvent<LeanFinger>
		{
		}

		[Tooltip("If the finger is over the GUI, ignore it?")]
		public bool IgnoreIfOverGui;

		[Tooltip("If the finger started over the GUI, ignore it?")]
		public bool IgnoreIfStartedOverGui;

		public LeanFingerEvent OnFingerUp;

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerUp = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerUp, new Action<LeanFinger>(FingerUp));
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerUp = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerUp, new Action<LeanFinger>(FingerUp));
		}

		private void FingerUp(LeanFinger finger)
		{
			if ((!IgnoreIfOverGui || !finger.IsOverGui) && (!IgnoreIfStartedOverGui || !finger.StartedOverGui))
			{
				OnFingerUp.Invoke(finger);
			}
		}
	}
}
