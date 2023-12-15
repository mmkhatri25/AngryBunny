using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
	[Serializable]
	public class FontCategory
	{
		public Font font;

		public List<SystemLanguage> languages;
	}
}
