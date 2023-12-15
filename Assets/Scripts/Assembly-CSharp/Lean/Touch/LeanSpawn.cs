using UnityEngine;

namespace Lean.Touch
{
	public class LeanSpawn : MonoBehaviour
	{
		[Tooltip("The prefab that gets spawned")]
		public Transform Prefab;

		[Tooltip("The distance from the finger the prefab will be spawned in world space")]
		public float Distance = 10f;

		[Tooltip("If spawning with velocity, rotate to it?")]
		public bool RotateToVelocity;

		[Tooltip("If spawning with velocity, scale it?")]
		public float VelocityMultiplier = 1f;

		public void SpawnWithVelocity(Vector3 start, Vector3 end)
		{
			if (Prefab != null)
			{
				Vector3 vector = end - start;
				float num = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
				Transform transform = Object.Instantiate(Prefab);
				transform.position = start;
				transform.rotation = Quaternion.Euler(0f, 0f, 0f - num);
				Rigidbody component = transform.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.velocity = vector * VelocityMultiplier;
				}
				Rigidbody2D component2 = transform.GetComponent<Rigidbody2D>();
				if (component2 != null)
				{
					component2.velocity = vector * VelocityMultiplier;
				}
			}
		}

		public void Spawn()
		{
			if (Prefab != null)
			{
				Object.Instantiate(Prefab, base.transform.position, base.transform.rotation);
			}
		}

		public void Spawn(LeanFinger finger)
		{
			if (Prefab != null && finger != null)
			{
				Object.Instantiate(Prefab, finger.GetWorldPosition(Distance), base.transform.rotation);
			}
		}
	}
}
