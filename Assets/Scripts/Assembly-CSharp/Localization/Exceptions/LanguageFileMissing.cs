using System;

namespace Localization.Exceptions
{
	public class LanguageFileMissing : Exception
	{
		public LanguageFileMissing(string tag)
			: base("Missing language: " + tag)
		{
		}
	}
}
