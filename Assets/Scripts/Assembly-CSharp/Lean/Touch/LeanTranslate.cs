using System.Collections.Generic;
using UnityEngine;

namespace Lean.Touch
{
	public class LeanTranslate : MonoBehaviour
	{
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = true;

		[Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("Does translation require an object to be selected?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("The camera the translation will be calculated using (None = MainCamera)")]
		public Camera Camera;

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
				List<LeanFinger> fingers = LeanTouch.GetFingers(IgnoreGuiFingers, RequiredFingerCount, RequiredSelectable);
				Vector2 screenDelta = LeanGesture.GetScreenDelta(fingers);
				Translate(screenDelta);
			}
		}

		protected virtual void Translate(Vector2 screenDelta)
		{
			Camera camera = LeanTouch.GetCamera(Camera, base.gameObject);
			if (camera != null)
			{
				Vector3 position = camera.WorldToScreenPoint(base.transform.position);
				position += (Vector3)screenDelta;
				base.transform.position = camera.ScreenToWorldPoint(position);
			}
		}
	}
}
