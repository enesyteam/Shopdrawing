using Microsoft.Expression.Utility.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Expression.Utility.Globalization
{
	public static class LocalizationHelper
	{
		internal static string FindFolderForCulture(CultureInfo culture, string rootDirectory, bool useLcidFormat)
		{
			string str;
			while (culture != null && !culture.Equals(CultureInfo.InvariantCulture))
			{
				if (useLcidFormat)
				{
					int lCID = culture.LCID;
					str = Path.Combine(rootDirectory, lCID.ToString(CultureInfo.InvariantCulture));
				}
				else
				{
					str = Path.Combine(rootDirectory, culture.Name);
				}
				string str1 = str;
				if (Microsoft.Expression.Utility.IO.PathHelper.DirectoryExists(str1))
				{
					return str1;
				}
				culture = culture.Parent;
			}
			return null;
		}

		public static string FindLocalizedSubdirectory(string rootDirectory, bool useLcidFormat = false)
		{
			string str;
			using (IEnumerator<CultureInfo> enumerator = CultureManager.PreferredCulturesExtended.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string str1 = LocalizationHelper.FindFolderForCulture(enumerator.Current, rootDirectory, useLcidFormat);
					if (string.IsNullOrEmpty(str1))
					{
						continue;
					}
					str = str1;
					return str;
				}
				return null;
			}
			return str;
		}

		public static string TranslatedFolder(string rootFolderName)
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetAssembly(typeof(LocalizationHelper)).Location);
			return LocalizationHelper.FindLocalizedSubdirectory(Path.Combine(directoryName, rootFolderName), false);
		}
	}
}