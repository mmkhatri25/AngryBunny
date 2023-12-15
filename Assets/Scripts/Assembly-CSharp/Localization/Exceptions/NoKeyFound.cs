using System;

namespace Localization.Exceptions
{
	public class NoKeyFound : Exception
	{
		public NoKeyFound(string key)
			: base("Missing key: " + key)
		{
		}
	}
}
