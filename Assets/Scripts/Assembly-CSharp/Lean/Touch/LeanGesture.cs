using System.Collections.Generic;
using UnityEngine;

namespace Lean.Touch
{
	public static class LeanGesture
	{
		public static Vector2 GetScreenCenter()
		{
			return GetScreenCenter(LeanTouch.Fingers);
		}

		public static Vector2 GetScreenCenter(List<LeanFinger> fingers)
		{
			Vector2 center = default(Vector2);
			TryGetScreenCenter(fingers, ref center);
			return center;
		}

		public static bool TryGetScreenCenter(List<LeanFinger> fingers, ref Vector2 center)
		{
			if (fingers != null)
			{
				Vector2 zero = Vector2.zero;
				int num = 0;
				for (int num2 = fingers.Count - 1; num2 >= 0; num2--)
				{
					LeanFinger leanFinger = fingers[num2];
					if (leanFinger != null)
					{
						zero += leanFinger.ScreenPosition;
						num++;
					}
				}
				if (num > 0)
				{
					center = zero / num;
					return true;
				}
			}
			return false;
		}

		public static Vector2 GetLastScreenCenter()
		{
			return GetLastScreenCenter(LeanTouch.Fingers);
		}

		public static Vector2 GetLastScreenCenter(List<LeanFinger> fingers)
		{
			Vector2 center = default(Vector2);
			TryGetLastScreenCenter(fingers, ref center);
			return center;
		}

		public static bool TryGetLastScreenCenter(List<LeanFinger> fingers, ref Vector2 center)
		{
			if (fingers != null)
			{
				Vector2 zero = Vector2.zero;
				int num = 0;
				for (int num2 = fingers.Count - 1; num2 >= 0; num2--)
				{
					LeanFinger leanFinger = fingers[num2];
					if (leanFinger != null)
					{
						zero += leanFinger.LastScreenPosition;
						num++;
					}
				}
				if (num > 0)
				{
					center = zero / num;
					return true;
				}
			}
			return false;
		}

		public static Vector2 GetScreenDelta()
		{
			return GetScreenDelta(LeanTouch.Fingers);
		}

		public static Vector2 GetScreenDelta(List<LeanFinger> fingers)
		{
			Vector2 delta = default(Vector2);
			TryGetScreenDelta(fingers, ref delta);
			return delta;
		}

		public static bool TryGetScreenDelta(List<LeanFinger> fingers, ref Vector2 delta)
		{
			if (fingers != null)
			{
				Vector2 zero = Vector2.zero;
				int num = 0;
				for (int num2 = fingers.Count - 1; num2 >= 0; num2--)
				{
					LeanFinger leanFinger = fingers[num2];
					if (leanFinger != null)
					{
						zero += leanFinger.ScreenDelta;
						num++;
					}
				}
				if (num > 0)
				{
					delta = zero / num;
					return true;
				}
			}
			return false;
		}

		public static Vector2 GetScaledDelta()
		{
			return GetScreenDelta() * LeanTouch.ScalingFactor;
		}

		public static Vector2 GetScaledDelta(List<LeanFinger> fingers)
		{
			return GetScreenDelta(fingers) * LeanTouch.ScalingFactor;
		}

		public static bool TryGetScaledDelta(List<LeanFinger> fingers, ref Vector2 delta)
		{
			if (TryGetScreenDelta(fingers, ref delta))
			{
				delta *= LeanTouch.ScalingFactor;
				return true;
			}
			return false;
		}

		public static Vector3 GetWorldDelta(float distance, Camera camera = null)
		{
			return GetWorldDelta(LeanTouch.Fingers, distance, camera);
		}

		public static Vector3 GetWorldDelta(List<LeanFinger> fingers, float distance, Camera camera = null)
		{
			Vector3 delta = default(Vector3);
			TryGetWorldDelta(fingers, distance, ref delta, camera);
			return delta;
		}

		public static bool TryGetWorldDelta(List<LeanFinger> fingers, float distance, ref Vector3 delta, Camera camera = null)
		{
			if (fingers != null)
			{
				Vector3 zero = Vector3.zero;
				int num = 0;
				for (int num2 = fingers.Count - 1; num2 >= 0; num2--)
				{
					LeanFinger leanFinger = fingers[num2];
					if (leanFinger != null)
					{
						zero += leanFinger.GetWorldDelta(distance, camera);
						num++;
					}
				}
				if (num > 0)
				{
					delta = zero / num;
					return true;
				}
			}
			return false;
		}

		public static float GetScreenDistance()
		{
			return GetScreenDistance(LeanTouch.Fingers);
		}

		public static float GetScreenDistance(List<LeanFinger> fingers)
		{
			float distance = 0f;
			Vector2 center = default(Vector2);
			if (TryGetScreenCenter(fingers, ref center))
			{
				TryGetScreenDistance(fingers, center, ref distance);
			}
			return distance;
		}

		public static float GetScreenDistance(List<LeanFinger> fingers, Vector2 center)
		{
			float distance = 0f;
			TryGetScreenDistance(fingers, center, ref distance);
			return distance;
		}

		public static bool TryGetScreenDistance(List<LeanFinger> fingers, Vector2 center, ref float distance)
		{
			if (fingers != null)
			{
				float num = 0f;
				int num2 = 0;
				for (int num3 = fingers.Count - 1; num3 >= 0; num3--)
				{
					LeanFinger leanFinger = fingers[num3];
					if (leanFinger != null)
					{
						num += leanFinger.GetScreenDistance(center);
						num2++;
					}
				}
				if (num2 > 0)
				{
					distance = num / (float)num2;
					return true;
				}
			}
			return false;
		}

		public static float GetScaledDistance()
		{
			return GetScreenDistance() * LeanTouch.ScalingFactor;
		}

		public static float GetScaledDistance(List<LeanFinger> fingers)
		{
			return GetScreenDistance(fingers) * LeanTouch.ScalingFactor;
		}

		public static float GetScaledDistance(List<LeanFinger> fingers, Vector2 center)
		{
			return GetScreenDistance(fingers, center) * LeanTouch.ScalingFactor;
		}

		public static bool TryGetScaledDistance(List<LeanFinger> fingers, Vector2 center, ref float distance)
		{
			if (TryGetScreenDistance(fingers, center, ref distance))
			{
				distance *= LeanTouch.ScalingFactor;
				return true;
			}
			return false;
		}

		public static float GetLastScreenDistance()
		{
			return GetLastScreenDistance(LeanTouch.Fingers);
		}

		public static float GetLastScreenDistance(List<LeanFinger> fingers)
		{
			float distance = 0f;
			Vector2 center = default(Vector2);
			if (TryGetLastScreenCenter(fingers, ref center))
			{
				TryGetLastScreenDistance(fingers, center, ref distance);
			}
			return distance;
		}

		public static float GetLastScreenDistance(List<LeanFinger> fingers, Vector2 center)
		{
			float distance = 0f;
			TryGetLastScreenDistance(fingers, center, ref distance);
			return distance;
		}

		public static bool TryGetLastScreenDistance(List<LeanFinger> fingers, Vector2 center, ref float distance)
		{
			if (fingers != null)
			{
				float num = 0f;
				int num2 = 0;
				for (int num3 = fingers.Count - 1; num3 >= 0; num3--)
				{
					LeanFinger leanFinger = fingers[num3];
					if (leanFinger != null)
					{
						num += leanFinger.GetLastScreenDistance(center);
						num2++;
					}
				}
				if (num2 > 0)
				{
					distance = num / (float)num2;
					return true;
				}
			}
			return false;
		}

		public static float GetLastScaledDistance()
		{
			return GetLastScreenDistance() * LeanTouch.ScalingFactor;
		}

		public static float GetLastScaledDistance(List<LeanFinger> fingers)
		{
			return GetLastScreenDistance(fingers) * LeanTouch.ScalingFactor;
		}

		public static float GetLastScaledDistance(List<LeanFinger> fingers, Vector2 center)
		{
			return GetLastScreenDistance(fingers, center) * LeanTouch.ScalingFactor;
		}

		public static bool TryGetLastScaledDistance(List<LeanFinger> fingers, Vector2 center, ref float distance)
		{
			if (TryGetLastScreenDistance(fingers, center, ref distance))
			{
				distance *= LeanTouch.ScalingFactor;
				return true;
			}
			return false;
		}

		public static float GetPinchScale(float wheelSensitivity = 0f)
		{
			return GetPinchScale(LeanTouch.Fingers, wheelSensitivity);
		}

		public static float GetPinchScale(List<LeanFinger> fingers, float wheelSensitivity = 0f)
		{
			float scale = 1f;
			Vector2 screenCenter = GetScreenCenter(fingers);
			Vector2 lastScreenCenter = GetLastScreenCenter(fingers);
			TryGetPinchScale(fingers, screenCenter, lastScreenCenter, ref scale, wheelSensitivity);
			return scale;
		}

		public static bool TryGetPinchScale(List<LeanFinger> fingers, Vector2 center, Vector2 lastCenter, ref float scale, float wheelSensitivity = 0f)
		{
			float screenDistance = GetScreenDistance(fingers, center);
			float lastScreenDistance = GetLastScreenDistance(fingers, lastCenter);
			if (lastScreenDistance > 0f)
			{
				scale = screenDistance / lastScreenDistance;
				return true;
			}
			if (wheelSensitivity != 0f)
			{
				float y = Input.mouseScrollDelta.y;
				if (y > 0f)
				{
					scale = 1f - wheelSensitivity;
					return true;
				}
				if (y < 0f)
				{
					scale = 1f + wheelSensitivity;
					return true;
				}
			}
			return false;
		}

		public static float GetPinchRatio(float wheelSensitivity = 0f)
		{
			return GetPinchRatio(LeanTouch.Fingers, wheelSensitivity);
		}

		public static float GetPinchRatio(List<LeanFinger> fingers, float wheelSensitivity = 0f)
		{
			float ratio = 1f;
			Vector2 screenCenter = GetScreenCenter(fingers);
			Vector2 lastScreenCenter = GetLastScreenCenter(fingers);
			TryGetPinchRatio(fingers, screenCenter, lastScreenCenter, ref ratio, wheelSensitivity);
			return ratio;
		}

		public static bool TryGetPinchRatio(List<LeanFinger> fingers, Vector2 center, Vector2 lastCenter, ref float ratio, float wheelSensitivity = 0f)
		{
			float screenDistance = GetScreenDistance(fingers, center);
			float lastScreenDistance = GetLastScreenDistance(fingers, lastCenter);
			if (screenDistance > 0f)
			{
				ratio = lastScreenDistance / screenDistance;
				return true;
			}
			if (wheelSensitivity != 0f)
			{
				float y = Input.mouseScrollDelta.y;
				if (y > 0f)
				{
					ratio = 1f + wheelSensitivity;
					return true;
				}
				if (y < 0f)
				{
					ratio = 1f - wheelSensitivity;
					return true;
				}
			}
			return false;
		}

		public static float GetTwistDegrees()
		{
			return GetTwistDegrees(LeanTouch.Fingers);
		}

		public static float GetTwistDegrees(List<LeanFinger> fingers)
		{
			return GetTwistRadians(fingers) * 57.29578f;
		}

		public static float GetTwistDegrees(List<LeanFinger> fingers, Vector2 center, Vector2 lastCenter)
		{
			return GetTwistRadians(fingers, center, lastCenter) * 57.29578f;
		}

		public static bool TryGetTwistDegrees(List<LeanFinger> fingers, Vector2 center, Vector2 lastCenter, ref float degrees)
		{
			if (TryGetTwistRadians(fingers, center, lastCenter, ref degrees))
			{
				degrees *= 57.29578f;
				return true;
			}
			return false;
		}

		public static float GetTwistRadians()
		{
			return GetTwistRadians(LeanTouch.Fingers);
		}

		public static float GetTwistRadians(List<LeanFinger> fingers)
		{
			Vector2 screenCenter = GetScreenCenter(fingers);
			Vector2 lastScreenCenter = GetLastScreenCenter(fingers);
			return GetTwistRadians(fingers, screenCenter, lastScreenCenter);
		}

		public static float GetTwistRadians(List<LeanFinger> fingers, Vector2 center, Vector2 lastCenter)
		{
			float radians = 0f;
			TryGetTwistRadians(fingers, center, lastCenter, ref radians);
			return radians;
		}

		public static bool TryGetTwistRadians(List<LeanFinger> fingers, Vector2 center, Vector2 lastCenter, ref float radians)
		{
			if (fingers != null)
			{
				float num = 0f;
				int num2 = 0;
				for (int num3 = fingers.Count - 1; num3 >= 0; num3--)
				{
					LeanFinger leanFinger = fingers[num3];
					if (leanFinger != null)
					{
						num += leanFinger.GetDeltaRadians(center, lastCenter);
						num2++;
					}
				}
				if (num2 > 0)
				{
					radians = num / (float)num2;
					return true;
				}
			}
			return false;
		}
	}
}
