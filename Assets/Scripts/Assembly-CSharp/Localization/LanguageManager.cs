using System;
using System.Collections.Generic;
using Localization.Exceptions;
using UnityEngine;

namespace Localization
{
	public class LanguageManager : MonoBehaviour
	{
		[Header("Options")]
		[SerializeField]
		[Tooltip("Used if <<UseDeviceLanguage>> is unchecked")]
		private SystemLanguage defaultLanguage = SystemLanguage.English;

		private SystemLanguage language = SystemLanguage.English;

		private string languageTag = string.Empty;

		[SerializeField]
		private bool useDeviceLanguage;

		[SerializeField]
		private SystemLanguage[] supportedLanguages = new SystemLanguage[1] { SystemLanguage.English };

		private Dictionary<string, string> fields;

		private Dictionary<SystemLanguage, string> languageTags;

		[Header("Fonts")]
		[SerializeField]
		private Font defaultFont;

		[SerializeField]
		private List<FontCategory> specialFonts;

		[Header("Resources")]
		[SerializeField]
		private string fileLocation = "Languages/values";

		[Header("Debugging")]
		[SerializeField]
		private bool debugging;

		[SerializeField]
		private string debuggingTag = "_dbg";

		public static LanguageManager Instance { get; private set; }

		public string LanguageTag
		{
			get
			{
				return languageTag;
			}
		}

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			else if (Instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			language = defaultLanguage;
			fields = new Dictionary<string, string>();
			languageTags = new Dictionary<SystemLanguage, string>();
			for (int i = 0; i < supportedLanguages.Length; i++)
			{
				languageTags[supportedLanguages[i]] = LanguagePair.GetTag(supportedLanguages[i]);
			}
			if (useDeviceLanguage)
			{
				language = Application.systemLanguage;
			}
			if (languageTags.ContainsKey(language))
			{
				languageTag = languageTags[language];
			}
			else
			{
				languageTag = LanguagePair.GetTag(defaultLanguage);
			}
			if (debugging)
			{
				languageTag = debuggingTag;
			}
			LoadLanguage(languageTag);
		}

		public void SetLanguage(SystemLanguage language)
		{
			if (!debugging)
			{
				this.language = language;
				languageTag = languageTags[language];
				LoadLanguage(languageTag);
			}
		}

		private void LoadLanguage(string lang)
		{
			fields.Clear();
			TextAsset textAsset = (TextAsset)Resources.Load(fileLocation + languageTag);
			if (textAsset == null)
			{
				throw new LanguageFileMissing(lang);
			}
			string empty = string.Empty;
			empty = textAsset.text;
			string[] array = empty.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].StartsWith("#") && array[i].IndexOf("=") >= 0 && !array[i].StartsWith("#"))
				{
					string key = array[i].Substring(0, array[i].IndexOf("="));
					string value = array[i].Substring(array[i].IndexOf("=") + 1, array[i].Length - array[i].IndexOf("=") - 1).Replace("\\n", Environment.NewLine);
					fields.Add(key, value);
				}
			}
		}

		public static string Get(string key)
		{
			if (Instance == null)
			{
				throw new NoInstance();
			}
			string value;
			if (Instance.fields.TryGetValue(key, out value))
			{
				return value;
			}
			throw new NoKeyFound(key);
		}

		public static Font GetFont()
		{
			if (Instance == null)
			{
				for (int i = 0; i < Instance.specialFonts.Count; i++)
				{
					if (Instance.specialFonts[i].languages.Contains(Instance.defaultLanguage))
					{
						return Instance.specialFonts[i].font;
					}
				}
			}
			return Instance.defaultFont;
		}
	}
}
