using System.Collections.Generic;
using UnityEngine;

namespace Lean.Touch
{
	[ExecuteInEditMode]
	public class LeanCameraZoom : MonoBehaviour
	{
		[Tooltip("The camera that will be zoomed (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = true;

		[Tooltip("Allows you to force rotation with a specific amount of fingers (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("If you want the mouse wheel to simulate pinching then set the strength of it here")]
		[Range(-1f, 1f)]
		public float WheelSensitivity;

		[Tooltip("The current FOV/Size")]
		public float Zoom = 50f;

		[Tooltip("Limit the FOV/Size?")]
		public bool ZoomClamp;

		[Tooltip("The minimum FOV/Size we want to zoom to")]
		public float ZoomMin = 10f;

		[Tooltip("The maximum FOV/Size we want to zoom to")]
		public float ZoomMax = 60f;

		[SerializeField]
		private Vector2 m_zoomPositionLimitsY;

		protected virtual void LateUpdate()
		{
			List<LeanFinger> fingers = LeanTouch.GetFingers(IgnoreGuiFingers, RequiredFingerCount);
			float pinchRatio = LeanGesture.GetPinchRatio(fingers, WheelSensitivity);
			Zoom *= pinchRatio;
			if (ZoomClamp)
			{
				Zoom = Mathf.Clamp(Zoom, ZoomMin, ZoomMax);
			}
			SetZoom(Zoom);
		}

		protected void SetZoom(float current)
		{
			Camera camera = LeanTouch.GetCamera(Camera, base.gameObject);
			if (camera != null)
			{
				if (camera.orthographic)
				{
					camera.orthographicSize = current;
					float t = (current - ZoomMin) / (ZoomMax - ZoomMin);
					float y = Mathf.Lerp(m_zoomPositionLimitsY.x, m_zoomPositionLimitsY.y, t);
					base.transform.position = new Vector3(base.transform.position.x, y, base.transform.position.z);
				}
				else
				{
					camera.fieldOfView = current;
				}
			}
		}
	}
}
