using UnityEngine;

namespace Lean.Touch
{
	public class LeanTranslateSmooth : LeanTranslate
	{
		[Tooltip("How smoothly this object moves to its target position")]
		public float Dampening = 10f;

		[HideInInspector]
		public Vector3 RemainingDelta;

		protected virtual void LateUpdate()
		{
			float dampenFactor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
			Vector3 vector = Vector3.Lerp(RemainingDelta, Vector3.zero, dampenFactor);
			base.transform.position += RemainingDelta - vector;
			RemainingDelta = vector;
		}

		protected override void Translate(Vector2 screenDelta)
		{
			Camera camera = LeanTouch.GetCamera(Camera, base.gameObject);
			if (camera != null)
			{
				Vector3 position = base.transform.position;
				Vector3 position2 = camera.WorldToScreenPoint(position);
				position2 += (Vector3)screenDelta;
				Vector3 vector = camera.ScreenToWorldPoint(position2);
				RemainingDelta += vector - position;
			}
		}
	}
}
