using System;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal static class TypeNameQuoter
	{
		private static char[] specialCharacters;

		static TypeNameQuoter()
		{
			TypeNameQuoter.specialCharacters = new char[] { '\\', '[', ']', ',', '+', '&', '*' };
		}

		private static bool Contains(char[] This, char ch)
		{
			char[] @this = This;
			for (int i = 0; i < (int)@this.Length; i++)
			{
				if (@this[i] == ch)
				{
					return true;
				}
			}
			return false;
		}

		internal static string GetQuotedTypeName(string name)
		{
			if (name.IndexOfAny(TypeNameQuoter.specialCharacters) == -1)
			{
				return name;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < name.Length; i++)
			{
				if (TypeNameQuoter.Contains(TypeNameQuoter.specialCharacters, name[i]))
				{
					stringBuilder.Append('\\');
				}
				stringBuilder.Append(name[i]);
			}
			return stringBuilder.ToString();
		}
	}
}