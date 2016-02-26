using System;
using System.IO;

namespace Microsoft.Expression.DesignModel.Core
{
	public static class FontFamilyHelper
	{
		public static string EnsureFamilyName(string familyName)
		{
			try
			{
				Uri.UnescapeDataString(familyName);
			}
			catch (UriFormatException uriFormatException)
			{
				familyName = Uri.EscapeUriString(familyName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			}
			return familyName;
		}
	}
}