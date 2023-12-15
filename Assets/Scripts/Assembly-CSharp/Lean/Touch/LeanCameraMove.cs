using System.Collections.Generic;
using UnityEngine;

namespace Lean.Touch
{
	public class LeanCameraMove : MonoBehaviour
	{
		[Tooltip("The camera the movement will be done relative to (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = true;

		[Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("The distance from the camera the world drag delta will be calculated from (this only matters for perspective cameras)")]
		public float Distance = 1f;

		[Tooltip("The sensitivity of the movement, use -1 to invert")]
		public float Sensitivity = 1f;

		protected virtual void LateUpdate()
		{
			Camera camera = LeanTouch.GetCamera(Camera, base.gameObject);
			if (camera != null)
			{
				List<LeanFinger> fingers = LeanTouch.GetFingers(IgnoreGuiFingers, RequiredFingerCount);
				Vector3 worldDelta = LeanGesture.GetWorldDelta(fingers, Distance, camera);
				base.transform.position -= Vector3.right * (worldDelta.x * Sensitivity);
			}
		}
	}
}
