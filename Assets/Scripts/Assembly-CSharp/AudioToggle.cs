using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
	[SerializeField]
	private Sprite m_spriteOn;

	[SerializeField]
	private Sprite m_spriteOff;

	[Range(0f, 1f)]
	[SerializeField]
	private float m_maxVolume = 1f;

	private const string m_saveKey = "setting_audio_off";

	private Image m_image;

	private bool IsOn
	{
		get
		{
			return SaveManager.LoadInt("setting_audio_off") == 0;
		}
		set
		{
			SaveManager.SaveInt("setting_audio_off", (!value) ? 1 : 0);
		}
	}

	private void Start()
	{
		m_image = GetComponentInChildren<Image>();
		Set(IsOn);
	}
    private void OnEnable()
    {
        m_image = GetComponentInChildren<Image>();
        Set(IsOn);
    }

    private void Set(bool state)
	{
		m_image.sprite = ((!state) ? m_spriteOff : m_spriteOn);
		AudioController.SetVolume((!state) ? 0f : m_maxVolume);
		IsOn = state;
	}

	public void Toggle()
	{
		Set(!IsOn);
	}
}
