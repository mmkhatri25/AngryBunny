using System.Collections.Generic;
using Localization.Exceptions;
using UnityEngine;

namespace Localization
{
	public static class LanguagePair
	{
		private static Dictionary<SystemLanguage, string> languageTags;

		static LanguagePair()
		{
			languageTags = new Dictionary<SystemLanguage, string>();
			Debug.LogWarning("LANGUAGE MANAGER: Some languages tags are not added");
			languageTags.Add(SystemLanguage.English, string.Empty);
			languageTags.Add(SystemLanguage.Romanian, "_ro");
			languageTags.Add(SystemLanguage.German, "_de");
			languageTags.Add(SystemLanguage.French, "_fr");
			languageTags.Add(SystemLanguage.Spanish, "_es");
			languageTags.Add(SystemLanguage.Portuguese, "_pt");
			languageTags.Add(SystemLanguage.Turkish, "_tr");
			languageTags.Add(SystemLanguage.Russian, "_ru");
			languageTags.Add(SystemLanguage.Chinese, "_cn_tr");
			languageTags.Add(SystemLanguage.ChineseTraditional, "_cn_tr");
			languageTags.Add(SystemLanguage.ChineseSimplified, "_zh_cn");
			languageTags.Add(SystemLanguage.Japanese, "_ja");
			languageTags.Add(SystemLanguage.Korean, "_kr");
			languageTags.Add(SystemLanguage.Italian, "_it");
			languageTags.Add(SystemLanguage.Dutch, "_nl");
			languageTags.Add(SystemLanguage.Unknown, string.Empty);
			languageTags.Add(SystemLanguage.Arabic, "_ar");
			languageTags.Add(SystemLanguage.Norwegian, "_no");
		}

		public static string GetTag(SystemLanguage language)
		{
			try
			{
				return languageTags[language];
			}
			catch (KeyNotFoundException)
			{
				throw new LanguageFileMissing(language.ToString() + ". Consider adding it to the languages dictionary in LanguageManagerUtilities.cs");
			}
		}
	}
}
