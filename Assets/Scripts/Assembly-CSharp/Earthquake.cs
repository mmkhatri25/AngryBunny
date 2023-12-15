using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : MonoBehaviour
{
	public delegate void EarthquakeEvent();

	public float force = 25f;

	public float rate = 0.75f;

	public float duration = 3f;

	public IAudioEvent audioEvent;

	private bool m_active;

	private CameraController m_camera;

	private int m_hits;

	public bool Active
	{
		get
		{
			return m_active;
		}
	}

	public void Reset()
	{
		m_active = false;
	}

	private void Awake()
	{
		m_camera = Object.FindObjectOfType<CameraController>();
	}

	public void Init()
	{
		List<Breakable> breakables = GetBreakables();
		m_hits = (int)Mathf.Ceil(rate * (float)breakables.Count);
	}

	public List<Breakable> GetBreakables()
	{
		Breakable[] array = Object.FindObjectsOfType<Breakable>();
		List<Breakable> list = new List<Breakable>(array);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Objective)
			{
				list.Remove(array[i]);
			}
		}
		return list;
	}

	public void Apply(EarthquakeEvent onFinished)
	{
		m_active = true;
		List<Breakable> breakables = GetBreakables();
		audioEvent.Play(GetComponent<AudioSource>(), AudioController.Volume);
		StartCoroutine(Shake(breakables, onFinished));
		m_camera.Shake(duration, force * 0.05f);
	}

	private IEnumerator Shake(List<Breakable> list, EarthquakeEvent onFinished)
	{
		float interval = duration / (float)m_hits;
		for (int i = 0; i < m_hits; i++)
		{
			while (true)
			{
				int index = Random.Range(0, list.Count);
				int num = 0;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].Active)
					{
						num++;
					}
				}
				if (num == 0)
				{
					break;
				}
				if (list[index].Active)
				{
					list[index].ApplyForce(force);
					break;
				}
			}
			GameController.Vibrate();
			float timer = 0f;
			while (timer < interval)
			{
				timer += Time.deltaTime;
				yield return null;
			}
		}
		m_active = false;
		onFinished();
	}
}
