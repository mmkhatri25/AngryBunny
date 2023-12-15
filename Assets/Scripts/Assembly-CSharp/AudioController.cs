using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	[SerializeField]
	private List<AudioSource> m_audioSources;

	public const int UI_BUTTON = 0;

	public const int POP = 1;

	public const int BACKGROUND = 2;

	public const int AMBIENT = 3;

	private static AudioController m_instance;

	private List<float> m_initVolumes;

	private float m_volume;

	private bool[] m_startingSources;

	private static AudioController Instance
	{
		get
		{
			if (!m_instance)
			{
				m_instance = Object.FindObjectOfType<AudioController>();
			}
			return m_instance;
		}
	}

	public static float Volume
	{
		get
		{
			return Instance.m_volume;
		}
	}

	private void Awake()
	{
		m_initVolumes = new List<float>();
		m_startingSources = new bool[4];
		for (int i = 0; i < m_audioSources.Count; i++)
		{
			m_initVolumes.Add(m_audioSources[i].volume);
		}
	}

	public static void SetVolume(float volume)
	{
		Instance.m_volume = volume;
		for (int i = 0; i < Instance.m_audioSources.Count; i++)
		{
			Instance.m_audioSources[i].volume = volume * Instance.m_initVolumes[i];
		}
	}

	public static void PlaySFX(int id)
	{
		Instance.m_audioSources[id].Play();
	}

	private IEnumerator FadeOutSource(int sourceID)
	{
		float timer = 0f;
		float start = m_audioSources[sourceID].volume;
		while (timer < 0.8f && !m_startingSources[sourceID])
		{
			timer += Time.unscaledDeltaTime;
			m_audioSources[sourceID].volume = Mathf.Lerp(start, 0f, timer / 0.8f);
			yield return null;
		}
		m_audioSources[sourceID].Stop();
	}

	private IEnumerator FadeInSource(int sourceID)
	{
		float timer = 0f;
		float start = m_audioSources[sourceID].volume;
		m_startingSources[sourceID] = true;
		m_audioSources[sourceID].Play();
		while (timer < 0.8f)
		{
			timer += Time.unscaledDeltaTime;
			m_audioSources[sourceID].volume = Mathf.Lerp(start, m_initVolumes[sourceID] * Instance.m_volume, timer / 0.8f);
			yield return null;
		}
		m_startingSources[sourceID] = false;
	}

	public static void StopBackgroundSource()
	{
		Instance.StartCoroutine(Instance.FadeOutSource(2));
	}

	public static void StartBackgroundSource()
	{
		Instance.StopCoroutine(Instance.FadeOutSource(2));
		Instance.StartCoroutine(Instance.FadeInSource(2));
	}

	public static void StopAmbientSource()
	{
		Debug.Log("Stop");
		Instance.StartCoroutine(Instance.FadeOutSource(3));
	}

	public static void StartAmbientSource()
	{
		Debug.Log("Start");
		Instance.StartCoroutine(Instance.FadeInSource(3));
	}

	public static void SetSourceClip(int sourceID, AudioClip clip)
	{
		Instance.m_audioSources[sourceID].clip = clip;
	}

	public static void SetSourceVolume(int sourceID, float volume)
	{
		Instance.m_audioSources[sourceID].volume = volume;
	}
}
