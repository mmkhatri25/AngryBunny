using UnityEngine;

public abstract class IAudioEvent : ScriptableObject
{
	public abstract void Play(AudioSource source);

	public abstract void Play(AudioSource source, float volumeMultiplier = 1f);

	public abstract void Stop();
}
