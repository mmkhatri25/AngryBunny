using System.Collections;
using Lean.Touch;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public float magnitude = 1f;

	public AnimationCurve curve;

	private LeanCameraZoomSmooth m_zoom;

	private void Awake()
	{
		m_zoom = Object.FindObjectOfType<LeanCameraZoomSmooth>();
	}

	public void Stop()
	{
		StopAllCoroutines();
	}

	public void Shake(float duration, float force)
	{
		StopAllCoroutines();
		StartCoroutine(ShakeCorotuine(duration, force));
	}

	private IEnumerator ShakeCorotuine(float duration, float force)
	{
		Vector3 startPos = base.transform.position;
		float timer = 0f;
		float m_currentZoom = m_zoom.Zoom;
		float m_maxZoom = Mathf.Lerp(m_currentZoom, m_zoom.ZoomMax, 0.5f);
		float m_minZoom = Mathf.Lerp(m_currentZoom, m_zoom.ZoomMin, 0.5f);
		float _magnitude2 = magnitude;
		while (timer < duration)
		{
			if (Time.timeScale != 0f)
			{
				timer += Time.deltaTime;
				_magnitude2 = Mathf.Lerp(magnitude, 0f, curve.Evaluate(timer / duration));
				base.transform.position = startPos + Random.insideUnitSphere * force * _magnitude2;
				m_zoom.Zoom = Random.Range(m_minZoom, m_maxZoom);
			}
			yield return null;
		}
		base.transform.position = startPos;
		m_zoom.Zoom = m_currentZoom;
	}
}
