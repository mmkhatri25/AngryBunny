using UnityEngine;

namespace Lean.Touch
{
	public class LeanAddForce : MonoBehaviour
	{
		[Tooltip("The strength of the force")]
		public float ForceMultiplier = 1f;

		[Tooltip("Should the force be based on recorded finger positions?")]
		public bool NoRelease;

		public void ApplyForce(Vector3 force)
		{
			TryApplyForce(base.transform, force);
		}

		public void ApplyForce(LeanFinger finger)
		{
			for (int i = 0; i < LeanSelectable.Instances.Count; i++)
			{
				LeanSelectable leanSelectable = LeanSelectable.Instances[i];
				if (leanSelectable.IsSelected || leanSelectable.SelectingFinger == finger)
				{
					Vector2 vector = finger.SwipeScaledDelta;
					if (NoRelease)
					{
						float tapThreshold = LeanTouch.Instance.TapThreshold;
						vector = finger.GetSnapshotScaledDelta(tapThreshold);
					}
					TryApplyForce(leanSelectable, vector);
				}
			}
		}

		private void TryApplyForce(Component component, Vector3 force)
		{
			Rigidbody componentInParent = component.GetComponentInParent<Rigidbody>();
			if (componentInParent != null)
			{
				componentInParent.AddForce(force * ForceMultiplier);
			}
			Rigidbody2D componentInParent2 = component.GetComponentInParent<Rigidbody2D>();
			if (componentInParent2 != null)
			{
				componentInParent2.AddForce(force * ForceMultiplier);
			}
		}
	}
}
