using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	public class LeanFingerLine : LeanFingerTrail
	{
		[Serializable]
		public class Vector3Vector3Event : UnityEvent<Vector3, Vector3>
		{
		}

		[Serializable]
		public class Vector3Event : UnityEvent<Vector3>
		{
		}

		[Tooltip("The thickness scale per unit (0 = no scaling)")]
		public float ThicknessScale;

		[Tooltip("Limit the length (0 = none)")]
		public float LengthMin;

		[Tooltip("Limit the length (0 = none)")]
		public float LengthMax;

		[Tooltip("Should the line originate from a target point?")]
		public Transform Target;

		public Vector3Vector3Event OnLineFingerUp;

		public Vector3Event OnLineFingerUpVelocity;

		protected override void LinkFingerUp(Link link)
		{
			Vector3 startPoint = GetStartPoint(link.Finger);
			Vector3 endPoint = GetEndPoint(link.Finger, startPoint);
			if (OnLineFingerUp != null)
			{
				OnLineFingerUp.Invoke(startPoint, endPoint);
			}
			if (OnLineFingerUpVelocity != null)
			{
				OnLineFingerUpVelocity.Invoke(endPoint - startPoint);
			}
		}

		protected override void WritePositions(LineRenderer line, LeanFinger finger)
		{
			Vector3 startPoint = GetStartPoint(finger);
			Vector3 endPoint = GetEndPoint(finger, startPoint);
			if (ThicknessScale > 0f)
			{
				float endWidth = (line.startWidth = Vector3.Distance(startPoint, endPoint) * ThicknessScale);
				line.endWidth = endWidth;
			}
			line.positionCount = 2;
			line.SetPosition(0, startPoint);
			line.SetPosition(1, endPoint);
		}

		private Vector3 GetStartPoint(LeanFinger finger)
		{
			if (Target != null)
			{
				return Target.position;
			}
			Camera camera = LeanTouch.GetCamera(Camera, base.gameObject);
			if (camera != null)
			{
				return finger.GetStartWorldPosition(Distance, camera);
			}
			return default(Vector3);
		}

		private Vector3 GetEndPoint(LeanFinger finger, Vector3 start)
		{
			Camera camera = LeanTouch.GetCamera(Camera, base.gameObject);
			if (camera != null)
			{
				float z = camera.WorldToScreenPoint(start).z;
				Vector3 worldPosition = finger.GetWorldPosition(z, camera);
				float num = Vector3.Distance(start, worldPosition);
				if (LengthMin > 0f && num < LengthMin)
				{
					num = LengthMin;
				}
				if (LengthMax > 0f && num > LengthMax)
				{
					num = LengthMax;
				}
				return start + Vector3.Normalize(worldPosition - start) * num;
			}
			return default(Vector3);
		}
	}
}
