using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	public class LeanFingerDown : MonoBehaviour
	{
		[Serializable]
		public class LeanFingerEvent : UnityEvent<LeanFinger>
		{
		}

		[Tooltip("If the finger is over the GUI, ignore it?")]
		public bool IgnoreIfOverGui;

		public LeanFingerEvent OnFingerDown;

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerDown, new Action<LeanFinger>(FingerDown));
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerDown, new Action<LeanFinger>(FingerDown));
		}

		private void FingerDown(LeanFinger finger)
		{
			if (!IgnoreIfOverGui || !finger.IsOverGui)
			{
				OnFingerDown.Invoke(finger);
			}
		}
	}
}
