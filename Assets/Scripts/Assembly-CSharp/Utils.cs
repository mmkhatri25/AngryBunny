using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Utils
{
	public static Vector3 GetCenter(Transform parent)
	{
		Vector3 position = parent.position;
		for (int i = 0; i < parent.childCount; i++)
		{
			position += parent.GetChild(i).position;
		}
		return position / parent.childCount;
	}

	public static void SmoothTransoformMove(MonoBehaviour Effector, Transform target, Vector3 startPos, Vector3 endPos, float duration, AnimationCurve curve = null, UnityEvent<Transform> OnMoveEnd = null)
	{
		Effector.StartCoroutine(SmoothTransformMoveCoroutine(target, startPos, endPos, duration, curve, OnMoveEnd));
	}

	private static IEnumerator SmoothTransformMoveCoroutine(Transform target, Vector3 startPos, Vector3 endPos, float duration, AnimationCurve curve, UnityEvent<Transform> OnMoveEnd = null)
	{
		float _timer = 0f;
		bool _quit = false;
		while (_timer < duration)
		{
			_timer += Time.deltaTime;
			float rate = ((curve == null) ? (_timer / duration) : curve.Evaluate(_timer / duration));
			if (!target)
			{
				_quit = true;
				break;
			}
			target.transform.position = Vector3.Lerp(startPos, endPos, rate);
			yield return null;
		}
		if (!_quit && OnMoveEnd != null)
		{
			OnMoveEnd.Invoke(target);
		}
	}

	public static string RandomString(int length)
	{
		string text = string.Empty;
		for (int i = 0; i < length; i++)
		{
			text += Random.Range(0, 256);
		}
		return text;
	}

	public static void ClearChildren(Transform parent)
	{
		int childCount = parent.childCount;
		for (int num = childCount - 1; num >= 0; num--)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(parent.GetChild(num).gameObject);
			}
			else
			{
				Object.DestroyImmediate(parent.GetChild(num).gameObject);
			}
		}
	}

	public static bool IsPointerOverUIObject()
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, list);
		return list.Count > 0;
	}

	public static float ScreenScaleMultiplier()
	{
		Vector2 vector = new Vector2(1600f, 900f);
		return new Vector2(vector.x / (float)Screen.width, vector.y / (float)Screen.height).magnitude;
	}
}
