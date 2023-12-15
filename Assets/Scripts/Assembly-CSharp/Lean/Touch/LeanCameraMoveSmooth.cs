using UnityEngine;

namespace Lean.Touch
{
	public class LeanCameraMoveSmooth : LeanCameraMove
	{
		[Tooltip("How quickly the zoom reaches the target value")]
		public float Dampening = 10f;

		public Vector2 Clamp;

		private float remainingDelta;

		protected override void LateUpdate()
		{
			float x = base.transform.localPosition.x;
			base.LateUpdate();
			remainingDelta += base.transform.localPosition.x - x;
			float dampenFactor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
			float num = Mathf.Lerp(remainingDelta, 0f, dampenFactor);
			Vector3 localPosition = base.transform.localPosition;
			localPosition.x = x + remainingDelta - num;
			if (localPosition.x < Clamp.x)
			{
				localPosition.x = Clamp.x;
			}
			if (localPosition.x > Clamp.y)
			{
				localPosition.x = Clamp.y;
			}
			base.transform.localPosition = localPosition;
			remainingDelta = num;
		}
	}
}
