using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	public class LeanFingerTrail : MonoBehaviour
	{
		[Serializable]
		public class LeanFingerEvent : UnityEvent<LeanFinger>
		{
		}

		[Serializable]
		public class Link
		{
			public LeanFinger Finger;

			public LineRenderer Line;
		}

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = true;

		[Tooltip("The line prefab")]
		public LineRenderer LinePrefab;

		[Tooltip("The distance from the camera the line points will be spawned in world space")]
		public float Distance = 1f;

		[Tooltip("The maximum amount of fingers used")]
		public int MaxLines;

		[Tooltip("The camera the translation will be calculated using (default = MainCamera)")]
		public Camera Camera;

		private List<Link> links = new List<Link>();

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerDown, new Action<LeanFinger>(FingerDown));
			LeanTouch.OnFingerSet = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerSet, new Action<LeanFinger>(FingerSet));
			LeanTouch.OnFingerUp = (Action<LeanFinger>)Delegate.Combine(LeanTouch.OnFingerUp, new Action<LeanFinger>(FingerUp));
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerDown, new Action<LeanFinger>(FingerDown));
			LeanTouch.OnFingerSet = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerSet, new Action<LeanFinger>(FingerSet));
			LeanTouch.OnFingerUp = (Action<LeanFinger>)Delegate.Remove(LeanTouch.OnFingerUp, new Action<LeanFinger>(FingerUp));
		}

		protected virtual void WritePositions(LineRenderer line, LeanFinger finger)
		{
			line.positionCount = finger.Snapshots.Count;
			for (int i = 0; i < finger.Snapshots.Count; i++)
			{
				LeanSnapshot leanSnapshot = finger.Snapshots[i];
				Vector3 worldPosition = leanSnapshot.GetWorldPosition(Distance, Camera);
				line.SetPosition(i, worldPosition);
			}
		}

		private void FingerDown(LeanFinger finger)
		{
			if (MaxLines <= 0 || links.Count < MaxLines)
			{
				Link link = new Link();
				link.Finger = finger;
				link.Line = UnityEngine.Object.Instantiate(LinePrefab);
				links.Add(link);
			}
		}

		private void FingerSet(LeanFinger finger)
		{
			Link link = FindLink(finger);
			if (link != null && link.Line != null)
			{
				WritePositions(link.Line, link.Finger);
			}
		}

		private void FingerUp(LeanFinger finger)
		{
			Link link = FindLink(finger);
			if (link != null)
			{
				links.Remove(link);
				LinkFingerUp(link);
				if (link.Line != null)
				{
					UnityEngine.Object.Destroy(link.Line.gameObject);
				}
			}
		}

		protected virtual void LinkFingerUp(Link link)
		{
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
