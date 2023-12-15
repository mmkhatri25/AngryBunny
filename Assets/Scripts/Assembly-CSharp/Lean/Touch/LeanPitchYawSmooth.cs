using UnityEngine;

namespace Lean.Touch
{
	public class LeanPitchYawSmooth : LeanPitchYaw
	{
		[Tooltip("How sharp the rotation value changes update")]
		[Space(10f)]
		public float Dampening = 3f;

		private float currentPitch;

		private float currentYaw;

		protected virtual void OnEnable()
		{
			currentPitch = Pitch;
			currentYaw = Yaw;
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			float dampenFactor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
			currentPitch = Mathf.Lerp(currentPitch, Pitch, dampenFactor);
			currentYaw = Mathf.Lerp(currentYaw, Yaw, dampenFactor);
			base.transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
		}
	}
}
