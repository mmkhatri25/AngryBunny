using System.Collections.Generic;
using UnityEngine;

namespace Lean.Touch
{
	public class LeanRotate : MonoBehaviour
	{
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers;

		[Tooltip("Allows you to force rotation with a specific amount of fingers (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("Does rotation require an object to be selected?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("The camera we will be moving (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("The rotation axis used for non-relative rotations")]
		public Vector3 RotateAxis = Vector3.forward;

		[Tooltip("Should the rotation be performanced relative to the finger center?")]
		public bool Relative;

		protected virtual void Start()
		{
			if (RequiredSelectable == null)
			{
				RequiredSelectable = GetComponent<LeanSelectable>();
			}
		}

		protected virtual void Update()
		{
			if (!(RequiredSelectable != null) || RequiredSelectable.IsSelected)
			{
				List<LeanFinger> fingers = LeanTouch.GetFingers(IgnoreGuiFingers, RequiredFingerCount);
				Vector2 screenCenter = LeanGesture.GetScreenCenter(fingers);
				float twistDegrees = LeanGesture.GetTwistDegrees(fingers);
				Rotate(screenCenter, twistDegrees);
			}
		}

		private void Rotate(Vector3 center, float degrees)
		{
			if (Relative)
			{
				Camera camera = LeanTouch.GetCamera(Camera, base.gameObject);
				if (camera != null)
				{
					Vector3 point = camera.ScreenToWorldPoint(center);
					base.transform.RotateAround(point, camera.transform.forward, degrees);
				}
			}
			else
			{
				base.transform.rotation *= Quaternion.AngleAxis(degrees, RotateAxis);
			}
		}
	}
}
