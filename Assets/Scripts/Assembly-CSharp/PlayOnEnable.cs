using UnityEngine;

public class PlayOnEnable : MonoBehaviour
{
	public IAudioEvent audioEvent;

	private void OnEnable()
	{
		audioEvent.Play(GetComponentInChildren<AudioSource>(), AudioController.Volume);
	}
}
