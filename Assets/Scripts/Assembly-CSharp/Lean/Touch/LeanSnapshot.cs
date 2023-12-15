using System.Collections.Generic;
using UnityEngine;

namespace Lean.Touch
{
	public class LeanSnapshot
	{
		public float Age;

		public Vector2 ScreenPosition;

		public static List<LeanSnapshot> InactiveSnapshots = new List<LeanSnapshot>(1000);

		public Vector3 GetWorldPosition(float distance, Camera camera = null)
		{
			camera = LeanTouch.GetCamera(camera);
			if (camera != null)
			{
				Vector3 position = new Vector3(ScreenPosition.x, ScreenPosition.y, distance);
				return camera.ScreenToWorldPoint(position);
			}
			return default(Vector3);
		}

		public static LeanSnapshot Pop()
		{
			if (InactiveSnapshots.Count > 0)
			{
				int index = InactiveSnapshots.Count - 1;
				LeanSnapshot result = InactiveSnapshots[index];
				InactiveSnapshots.RemoveAt(index);
				return result;
			}
			return new LeanSnapshot();
		}

		public static bool TryGetScreenPosition(List<LeanSnapshot> snapshots, float targetAge, ref Vector2 screenPosition)
		{
			if (snapshots != null && snapshots.Count > 0)
			{
				LeanSnapshot leanSnapshot = snapshots[0];
				if (targetAge <= leanSnapshot.Age)
				{
					screenPosition = leanSnapshot.ScreenPosition;
					return true;
				}
				LeanSnapshot leanSnapshot2 = snapshots[snapshots.Count - 1];
				if (targetAge >= leanSnapshot2.Age)
				{
					screenPosition = leanSnapshot2.ScreenPosition;
					return true;
				}
				int lowerIndex = GetLowerIndex(snapshots, targetAge);
				int num = lowerIndex + 1;
				LeanSnapshot leanSnapshot3 = snapshots[lowerIndex];
				LeanSnapshot leanSnapshot4 = ((num >= snapshots.Count) ? leanSnapshot3 : snapshots[num]);
				float t = Mathf.InverseLerp(leanSnapshot3.Age, leanSnapshot4.Age, targetAge);
				screenPosition = Vector2.Lerp(leanSnapshot3.ScreenPosition, leanSnapshot4.ScreenPosition, t);
				return true;
			}
			return false;
		}

		public static bool TryGetSnapshot(List<LeanSnapshot> snapshots, int index, ref float age, ref Vector2 screenPosition)
		{
			if (index >= 0 && index < snapshots.Count)
			{
				LeanSnapshot leanSnapshot = snapshots[index];
				age = leanSnapshot.Age;
				screenPosition = leanSnapshot.ScreenPosition;
				return true;
			}
			return true;
		}

		public static int GetLowerIndex(List<LeanSnapshot> snapshots, float targetAge)
		{
			if (snapshots != null)
			{
				int count = snapshots.Count;
				if (count > 0)
				{
					for (int num = count - 1; num >= 0; num--)
					{
						if (snapshots[num].Age <= targetAge)
						{
							return num;
						}
					}
				}
				return 0;
			}
			return -1;
		}
	}
}
