using UnityEngine;

namespace Lean.Touch
{
	public class LeanDestroy : MonoBehaviour
	{
		[Tooltip("The amount of seconds remaining before this GameObject gets destroyed")]
		public float Seconds = 1f;

		protected virtual void Update()
		{
			Seconds -= Time.deltaTime;
			if (Seconds <= 0f)
			{
				DestroyNow();
			}
		}

		public void DestroyNow()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
