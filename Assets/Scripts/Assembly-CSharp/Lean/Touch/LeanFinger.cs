using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lean.Touch
{
	public class LeanFinger
	{
		public int Index;

		public float Age;

		public bool Set;

		public bool LastSet;

		public bool Tap;

		public int TapCount;

		public bool Swipe;

		public Vector2 StartScreenPosition;

		public Vector2 LastScreenPosition;

		public Vector2 ScreenPosition;

		public bool StartedOverGui;

		public List<LeanSnapshot> Snapshots = new List<LeanSnapshot>(1000);

		public bool IsActive
		{
			get
			{
				return LeanTouch.Fingers.Contains(this);
			}
		}

		public float SnapshotDuration
		{
			get
			{
				if (Snapshots.Count > 0)
				{
					return Age - Snapshots[0].Age;
				}
				return 0f;
			}
		}

		public bool IsOverGui
		{
			get
			{
				return LeanTouch.PointOverGui(ScreenPosition);
			}
		}

		public bool Down
		{
			get
			{
				return Set && !LastSet;
			}
		}

		public bool Up
		{
			get
			{
				return !Set && LastSet;
			}
		}

		public Vector2 LastSnapshotScreenDelta
		{
			get
			{
				int count = Snapshots.Count;
				if (count > 0)
				{
					LeanSnapshot leanSnapshot = Snapshots[count - 1];
					if (leanSnapshot != null)
					{
						return ScreenPosition - leanSnapshot.ScreenPosition;
					}
				}
				return Vector2.zero;
			}
		}

		public Vector2 LastSnapshotScaledDelta
		{
			get
			{
				return LastSnapshotScreenDelta * LeanTouch.ScalingFactor;
			}
		}

		public Vector2 ScreenDelta
		{
			get
			{
				return ScreenPosition - LastScreenPosition;
			}
		}

		public Vector2 ScaledDelta
		{
			get
			{
				return ScreenDelta * LeanTouch.ScalingFactor;
			}
		}

		public Vector2 SwipeScreenDelta
		{
			get
			{
				return ScreenPosition - StartScreenPosition;
			}
		}

		public Vector2 SwipeScaledDelta
		{
			get
			{
				return SwipeScreenDelta * LeanTouch.ScalingFactor;
			}
		}

		public Ray GetRay(Camera camera = null)
		{
			camera = LeanTouch.GetCamera(camera);
			if (camera != null)
			{
				return camera.ScreenPointToRay(ScreenPosition);
			}
			return default(Ray);
		}

		public Ray GetStartRay(Camera camera = null)
		{
			camera = LeanTouch.GetCamera(camera);
			if (camera != null)
			{
				return camera.ScreenPointToRay(StartScreenPosition);
			}
			return default(Ray);
		}

		public Vector2 GetSnapshotScreenDelta(float deltaTime)
		{
			return ScreenPosition - GetSnapshotScreenPosition(Age - deltaTime);
		}

		public Vector2 GetSnapshotScaledDelta(float deltaTime)
		{
			return GetSnapshotScreenDelta(deltaTime) * LeanTouch.ScalingFactor;
		}

		public Vector2 GetSnapshotScreenPosition(float targetAge)
		{
			Vector2 screenPosition = ScreenPosition;
			LeanSnapshot.TryGetScreenPosition(Snapshots, targetAge, ref screenPosition);
			return screenPosition;
		}

		public Vector3 GetSnapshotWorldPosition(float targetAge, float distance, Camera camera = null)
		{
			camera = LeanTouch.GetCamera(camera);
			if (camera != null)
			{
				Vector2 snapshotScreenPosition = GetSnapshotScreenPosition(targetAge);
				Vector3 position = new Vector3(snapshotScreenPosition.x, snapshotScreenPosition.y, distance);
				return camera.ScreenToWorldPoint(position);
			}
			return default(Vector3);
		}

		public float GetRadians(Vector2 referencePoint)
		{
			return Mathf.Atan2(ScreenPosition.x - referencePoint.x, ScreenPosition.y - referencePoint.y);
		}

		public float GetDegrees(Vector2 referencePoint)
		{
			return GetRadians(referencePoint) * 57.29578f;
		}

		public float GetLastRadians(Vector2 referencePoint)
		{
			return Mathf.Atan2(LastScreenPosition.x - referencePoint.x, LastScreenPosition.y - referencePoint.y);
		}

		public float GetLastDegrees(Vector2 referencePoint)
		{
			return GetLastRadians(referencePoint) * 57.29578f;
		}

		public float GetDeltaRadians(Vector2 referencePoint)
		{
			return GetDeltaRadians(referencePoint, referencePoint);
		}

		public float GetDeltaRadians(Vector2 referencePoint, Vector2 lastReferencePoint)
		{
			float lastRadians = GetLastRadians(lastReferencePoint);
			float radians = GetRadians(referencePoint);
			float num = Mathf.Repeat(lastRadians - radians, (float)Math.PI * 2f);
			if (num > (float)Math.PI)
			{
				num -= (float)Math.PI * 2f;
			}
			return num;
		}

		public float GetDeltaDegrees(Vector2 referencePoint)
		{
			return GetDeltaRadians(referencePoint, referencePoint) * 57.29578f;
		}

		public float GetDeltaDegrees(Vector2 referencePoint, Vector2 lastReferencePoint)
		{
			return GetDeltaRadians(referencePoint, lastReferencePoint) * 57.29578f;
		}

		public float GetScreenDistance(Vector2 point)
		{
			return Vector2.Distance(ScreenPosition, point);
		}

		public float GetScaledDistance(Vector2 point)
		{
			return GetScreenDistance(point) * LeanTouch.ScalingFactor;
		}

		public float GetLastScreenDistance(Vector2 point)
		{
			return Vector2.Distance(LastScreenPosition, point);
		}

		public float GetLastScaledDistance(Vector2 point)
		{
			return GetLastScreenDistance(point) * LeanTouch.ScalingFactor;
		}

		public Vector3 GetStartWorldPosition(float distance, Camera camera = null)
		{
			camera = LeanTouch.GetCamera(camera);
			if (camera != null)
			{
				Vector3 position = new Vector3(StartScreenPosition.x, StartScreenPosition.y, distance);
				return camera.ScreenToWorldPoint(position);
			}
			return default(Vector3);
		}

		public Vector3 GetLastWorldPosition(float distance, Camera camera = null)
		{
			camera = LeanTouch.GetCamera(camera);
			if (camera != null)
			{
				Vector3 position = new Vector3(LastScreenPosition.x, LastScreenPosition.y, distance);
				return camera.ScreenToWorldPoint(position);
			}
			return default(Vector3);
		}

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

		public Vector3 GetWorldDelta(float distance, Camera camera = null)
		{
			return GetWorldDelta(distance, distance, camera);
		}

		public Vector3 GetWorldDelta(float lastDistance, float distance, Camera camera = null)
		{
			camera = LeanTouch.GetCamera(camera);
			if (camera != null)
			{
				return GetWorldPosition(distance, camera) - GetLastWorldPosition(lastDistance, camera);
			}
			return default(Vector3);
		}

		public void ClearSnapshots(int count = -1)
		{
			if (count > 0 && count <= Snapshots.Count)
			{
				for (int i = 0; i < count; i++)
				{
					LeanSnapshot.InactiveSnapshots.Add(Snapshots[i]);
				}
				Snapshots.RemoveRange(0, count);
			}
			else if (count < 0)
			{
				LeanSnapshot.InactiveSnapshots.AddRange(Snapshots);
				Snapshots.Clear();
			}
		}

		public void RecordSnapshot()
		{
			LeanSnapshot leanSnapshot = LeanSnapshot.Pop();
			leanSnapshot.Age = Age;
			leanSnapshot.ScreenPosition = ScreenPosition;
			Snapshots.Add(leanSnapshot);
		}
	}
}
