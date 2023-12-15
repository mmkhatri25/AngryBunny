using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	public class LeanMultiTap : MonoBehaviour
	{
		[Serializable]
		public class IntEvent : UnityEvent<int, int>
		{
		}

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = true;

		[Tooltip("This is set to true the frame a multi-tap occurs")]
		public bool MultiTap;

		[Tooltip("This is set to the current multi-tap count")]
		public int MultiTapCount;

		[Tooltip("Highest number of fingers held down during this multi-tap")]
		public int HighestFingerCount;

		public IntEvent OnMultiTap;

		private float age;

		private int lastFingerCount;

		protected virtual void Update()
		{
			List<LeanFinger> fingers = LeanTouch.GetFingers(IgnoreGuiFingers);
			int fingerCount = GetFingerCount(fingers);
			if (fingerCount > 0)
			{
				if (lastFingerCount == 0)
				{
					age = 0f;
					HighestFingerCount = fingerCount;
				}
				else if (fingerCount > HighestFingerCount)
				{
					HighestFingerCount = fingerCount;
				}
			}
			age += Time.unscaledDeltaTime;
			MultiTap = false;
			if (age <= LeanTouch.CurrentTapThreshold)
			{
				if (fingerCount == 0 && lastFingerCount > 0)
				{
					MultiTapCount++;
					OnMultiTap.Invoke(MultiTapCount, HighestFingerCount);
				}
			}
			else
			{
				MultiTapCount = 0;
				HighestFingerCount = 0;
			}
			lastFingerCount = fingerCount;
		}

		private int GetFingerCount(List<LeanFinger> fingers)
		{
			int num = 0;
			for (int num2 = fingers.Count - 1; num2 >= 0; num2--)
			{
				if (!fingers[num2].Up)
				{
					num++;
				}
			}
			return num;
		}
	}
}
