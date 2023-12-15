using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	public class LeanFingerHeld : MonoBehaviour
	{
		[Serializable]
		public class FingerEvent : UnityEvent<LeanFinger>
		{
		}

		[Serializable]
		public class Link
		{
			public LeanFinger Finger;

			public bool LastSet;

			public Vector2 TotalScaledDelta;
		}

		[Tooltip("If the finger started over the GUI, ignore it?")]
		public bool IgnoreIfStartedOverGui;

		[Tooltip("The finger must be held for this many seconds")]
		public float MinimumAge = 1f;

		[Tooltip("The finger cannot move more than this many pixels relative to the reference DPI")]
		public float MaximumMovement = 5f;

		public FingerEvent onFingerHeldDown;

		public FingerEvent onFingerHeldSet;

		public FingerEvent onFingerHeldUp;

		public static List<LeanFingerHeld> Instances = new List<LeanFingerHeld>();

		public static Action<LeanFinger> OnFingerHeldDown;

		public static Action<LeanFinger> OnFingerHeldSet;

		public static Action<LeanFinger> OnFingerHeldUp;

		private List<Link> links = new List<Link>();

		protected virtual void OnEnable()
		{
			Instances.Add(this);
			LeanTouch.OnFingerDown = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerDown, new Action<LeanFinger>(OnFingerDown));
			LeanTouch.OnFingerSet = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerSet, new Action<LeanFinger>(OnFingerSet));
			LeanTouch.OnFingerUp = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerUp, new Action<LeanFinger>(OnFingerUp));
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(this);
			LeanTouch.OnFingerDown = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerDown, new Action<LeanFinger>(OnFingerDown));
			LeanTouch.OnFingerSet = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerSet, new Action<LeanFinger>(OnFingerSet));
			LeanTouch.OnFingerUp = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerUp, new Action<LeanFinger>(OnFingerUp));
		}

		private void OnFingerDown(LeanFinger finger)
		{
			if (!IgnoreIfStartedOverGui || !finger.StartedOverGui)
			{
				Link link = FindLink(finger);
				if (link == null)
				{
					link = new Link();
					link.Finger = finger;
					links.Add(link);
				}
				link.LastSet = false;
				link.TotalScaledDelta = Vector2.zero;
			}
		}

		private void OnFingerSet(LeanFinger finger)
		{
			Link link = FindLink(finger);
			if (link == null)
			{
				return;
			}
			bool flag = finger.Age >= MinimumAge && link.TotalScaledDelta.magnitude < MaximumMovement;
			link.TotalScaledDelta += finger.ScaledDelta;
			if (flag && !link.LastSet)
			{
				onFingerHeldDown.Invoke(finger);
				if (Instances[0] == this && OnFingerHeldDown != null)
				{
					OnFingerHeldDown(finger);
				}
			}
			if (flag)
			{
				onFingerHeldSet.Invoke(finger);
				if (Instances[0] == this && OnFingerHeldSet != null)
				{
					OnFingerHeldSet(finger);
				}
			}
			if (!flag && link.LastSet)
			{
				onFingerHeldUp.Invoke(finger);
				if (Instances[0] == this && OnFingerHeldUp != null)
				{
					OnFingerHeldUp(finger);
				}
			}
			link.LastSet = flag;
		}

		private void OnFingerUp(LeanFinger finger)
		{
			Link link = FindLink(finger);
			if (link == null)
			{
				return;
			}
			if (link.LastSet)
			{
				onFingerHeldUp.Invoke(finger);
				if (Instances[0] == this && OnFingerHeldUp != null)
				{
					OnFingerHeldUp(finger);
				}
			}
			links.Remove(link);
		}

		private Link FindLink(LeanFinger finger)
		{
			for (int i = 0; i < links.Count; i++)
			{
				Link link = links[i];
				if (link.Finger == finger)
				{
					return link;
				}
			}
			return null;
		}
	}
}
