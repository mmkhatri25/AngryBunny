using System.Collections.Generic;
using UnityEngine;

namespace Lean.Touch
{
	[ExecuteInEditMode]
	public class LeanPitchYaw : MonoBehaviour
	{
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = true;

		[Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("Does turning require an object to be selected?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("If you want the rotation to be scaled by the camera FOV, then set that here")]
		public Camera Camera;

		[Tooltip("Pitch of the rotation in degrees")]
		[Space(10f)]
		public float Pitch;

		[Tooltip("The strength of the pitch changes with vertical finger movement")]
		public float PitchSensitivity = 0.25f;

		[Tooltip("Limit the pitch to min/max?")]
		public bool PitchClamp = true;

		[Tooltip("The minimum pitch angle in degrees")]
		public float PitchMin = -90f;

		[Tooltip("The maximum pitch angle in degrees")]
		public float PitchMax = 90f;

		[Tooltip("Yaw of the rotation in degrees")]
		[Space(10f)]
		public float Yaw;

		[Tooltip("The strength of the yaw changes with horizontal finger movement")]
		public float YawSensitivity = 0.25f;

		[Tooltip("Limit the yaw to min/max?")]
		public bool YawClamp;

		[Tooltip("The minimum yaw angle in degrees")]
		public float YawMin = -45f;

		[Tooltip("The maximum yaw angle in degrees")]
		public float YawMax = 45f;

		protected virtual void Start()
		{
			if (RequiredSelectable == null)
			{
				RequiredSelectable = GetComponent<LeanSelectable>();
			}
			if (Camera == null)
			{
				Camera = GetComponent<Camera>();
			}
		}

		protected virtual void LateUpdate()
		{
			if (RequiredSelectable != null && !RequiredSelectable.IsSelected)
			{
				UpdateRotation();
				return;
			}
			List<LeanFinger> fingers = LeanTouch.GetFingers(IgnoreGuiFingers, RequiredFingerCount);
			Vector2 scaledDelta = LeanGesture.GetScaledDelta(fingers);
			float sensitivity = GetSensitivity();
			Pitch += scaledDelta.y * PitchSensitivity * sensitivity;
			if (PitchClamp)
			{
				Pitch = Mathf.Clamp(Pitch, PitchMin, PitchMax);
			}
			Yaw -= scaledDelta.x * YawSensitivity * sensitivity;
			if (YawClamp)
			{
				Yaw = Mathf.Clamp(Yaw, YawMin, YawMax);
			}
			UpdateRotation();
		}

		private float GetSensitivity()
		{
			if (Camera != null && !Camera.orthographic)
			{
				return Camera.fieldOfView / 90f;
			}
			return 1f;
		}

		private void UpdateRotation()
		{
			base.transform.localRotation = Quaternion.Euler(Pitch, Yaw, 0f);
		}
	}
}
