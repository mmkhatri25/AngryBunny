using Lean.Touch;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio Events/Audio Event")]
public class AudioEvent : IAudioEvent
{
	public AudioClip[] clips;

	public Vector2 volume;

	public Vector2 pitch;

	public bool muteIfHidden = true;

	public float hiddenDistance = 10f;

	private AudioSource m_source;

	private static LeanCameraZoomSmooth m_zoom;

	private float HiddenDistance
	{
		get
		{
			return hiddenDistance * Zoom.Zoom;
		}
	}

	private static LeanCameraZoomSmooth Zoom
	{
		get
		{
			if (!m_zoom)
			{
				m_zoom = Object.FindObjectOfType<LeanCameraZoomSmooth>();
			}
			return m_zoom;
		}
	}

	public override void Play(AudioSource source, float volumeMultiplier = 1f)
	{
		if (clips.Length != 0 && (bool)source)
		{
			m_source = source;
			Vector3 position = source.transform.position;
			position.z = Camera.main.transform.position.z;
			float num = Vector3.Distance(position, Camera.main.transform.position);
			if (!(num > HiddenDistance) || !muteIfHidden)
			{
				source.clip = clips[Random.Range(0, clips.Length)];
				source.volume = Random.Range(volume.x, volume.y) * volumeMultiplier;
				source.pitch = Random.Range(pitch.x, pitch.y);
				source.Play();
			}
		}
	}

	public override void Play(AudioSource source)
	{
		Play(source, 1f);
	}

	public override void Stop()
	{
		if ((bool)m_source)
		{
			m_source.Stop();
		}
	}
}
