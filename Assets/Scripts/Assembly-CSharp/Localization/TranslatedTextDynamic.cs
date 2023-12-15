using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
	[RequireComponent(typeof(Text))]
	public class TranslatedTextDynamic : MonoBehaviour
	{
		[SerializeField]
		private string key;

		public string[] toReplace;

		public string[] valuesToReplaceWith;

		[SerializeField]
		private TextFormatting format;

		[SerializeField]
		[Tooltip("Use the Text component font instead of the LanguageManager font")]
		private bool useTextFont;

		private Text text;

		private bool skipEnable = true;

		private void Start()
		{
			text = GetComponent<Text>();
			UpdateText();
			skipEnable = false;
		}

		private void OnEnable()
		{
			if (!skipEnable)
			{
				UpdateText();
			}
		}

		public void UpdateText()
		{
			if (!useTextFont)
			{
				this.text.font = LanguageManager.GetFont();
			}
			string text = LanguageManager.Get(key);
			for (int i = 0; i < toReplace.Length; i++)
			{
				text = text.Replace(toReplace[i], valuesToReplaceWith[i]);
			}
			TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
			switch (format)
			{
			case TextFormatting.Unchanged:
				this.text.text = text;
				break;
			case TextFormatting.UpperCase:
				this.text.text = text.ToUpper();
				break;
			case TextFormatting.LowerCase:
				this.text.text = text.ToLower();
				break;
			case TextFormatting.TitleCase:
				this.text.text = textInfo.ToTitleCase(text);
				break;
			case TextFormatting.SentenceCase:
				if (text.Length > 0)
				{
					this.text.text = textInfo.ToUpper(text[0]).ToString();
				}
				if (text.Length >= 2)
				{
					this.text.text += text.Substring(1);
				}
				break;
			}
		}
	}
}
