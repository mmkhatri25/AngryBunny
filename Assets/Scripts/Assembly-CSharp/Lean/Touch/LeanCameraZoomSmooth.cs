using UnityEngine;

namespace Lean.Touch
{
	public class LeanCameraZoomSmooth : LeanCameraZoom
	{
		[Tooltip("How quickly the zoom reaches the target value")]
		public float Dampening = 10f;

		private float currentZoom;

		protected virtual void OnEnable()
		{
			currentZoom = Zoom;
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			float dampenFactor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
			currentZoom = Mathf.Lerp(currentZoom, Zoom, dampenFactor);
			SetZoom(currentZoom);
		}
	}
}
