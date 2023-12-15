using Localization;
using UnityEngine;
using UnityEngine.UI;

public class TranslatedImage : MonoBehaviour
{
	[Tooltip("Resources path to the default language file (without extension)")]
	[SerializeField]
	private string file;

	private Image image;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	private void Start()
	{
		string languageTag = LanguageManager.Instance.LanguageTag;
		image.sprite = Resources.Load<Sprite>(file + languageTag);
	}
}
