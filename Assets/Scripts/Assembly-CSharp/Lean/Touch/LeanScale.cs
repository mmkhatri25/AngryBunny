using System.Collections.Generic;
using UnityEngine;

namespace Lean.Touch
{
	public class LeanScale : MonoBehaviour
	{
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers;

		[Tooltip("Allows you to force rotation with a specific amount of fingers (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("Does scaling require an object to be selected?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("If you want the mouse wheel to simulate pinching then set the strength of it here")]
		[Range(-1f, 1f)]
		public float WheelSensitivity;

		[Tooltip("The camera that will be used to calculate the zoom (None = MainCamera)")]
		public Camera Camera;

		[Tooltip("Should the scaling be performanced relative to the finger center?")]
		public bool Relative;

		[Tooltip("Should the scale value be clamped?")]
		public bool ScaleClamp;

		[Tooltip("The minimum scale value on all axes")]
		public Vector3 ScaleMin;

		[Tooltip("The maximum scale value on all axes")]
		public Vector3 ScaleMax;

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
				float pinchScale = LeanGesture.GetPinchScale(fingers, WheelSensitivity);
				Vector2 screenCenter = LeanGesture.GetScreenCenter(fingers);
				Scale(pinchScale, screenCenter);
			}
		}

		private void Scale(float pinchScale, Vector2 screenCenter)
		{
			if (!(pinchScale > 0f))
			{
				return;
			}
			Vector3 localScale = base.transform.localScale;
			if (Relative)
			{
				Camera camera = LeanTouch.GetCamera(Camera, base.gameObject);
				if (camera != null)
				{
					Vector3 position = camera.WorldToScreenPoint(base.transform.position);
					position.x = screenCenter.x + (position.x - screenCenter.x) * pinchScale;
					position.y = screenCenter.y + (position.y - screenCenter.y) * pinchScale;
					base.transform.position = camera.ScreenToWorldPoint(position);
					localScale *= pinchScale;
				}
			}
			else
			{
				localScale *= pinchScale;
			}
			if (ScaleClamp)
			{
				localScale.x = Mathf.Clamp(localScale.x, ScaleMin.x, ScaleMax.x);
				localScale.y = Mathf.Clamp(localScale.y, ScaleMin.y, ScaleMax.y);
				localScale.z = Mathf.Clamp(localScale.z, ScaleMin.z, ScaleMax.z);
			}
			base.transform.localScale = localScale;
		}
	}
}
