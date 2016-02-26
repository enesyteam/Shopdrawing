using Microsoft.Expression.Project;
using System;
using System.Globalization;

namespace Microsoft.Expression.Project.UserInterface
{
	internal static class AlphabeticThenNumericComparer
	{
		public static int Compare(string first, string second, CultureInfo cultureInfo)
		{
			AlphabeticThenNumericComparer.SeparatedString separatedString = AlphabeticThenNumericComparer.TrimDigits(first, cultureInfo);
			AlphabeticThenNumericComparer.SeparatedString separatedString1 = AlphabeticThenNumericComparer.TrimDigits(second, cultureInfo);
			int num = ProjectPathHelper.CompareForDisplay(separatedString.BaseString, separatedString1.BaseString, cultureInfo);
			if (num == 0)
			{
				num = (separatedString.BaseString == first || separatedString1.BaseString == second ? first.CompareTo(second) : separatedString.Number - separatedString1.Number);
			}
			return num;
		}

		private static AlphabeticThenNumericComparer.SeparatedString TrimDigits(string input, CultureInfo cultureInfo)
		{
			AlphabeticThenNumericComparer.SeparatedString separatedString;
			int num;
			int length = input.Length - 1;
			while (length >= 0 && char.IsDigit(input[length]))
			{
				length--;
			}
			separatedString = (!int.TryParse(input.Substring(length + 1), NumberStyles.Integer, cultureInfo, out num) ? new AlphabeticThenNumericComparer.SeparatedString(input, 0) : new AlphabeticThenNumericComparer.SeparatedString(input.Substring(0, length + 1), num));
			return separatedString;
		}

		private struct SeparatedString
		{
			private string baseString;

			private int number;

			public string BaseString
			{
				get
				{
					return this.baseString;
				}
			}

			public int Number
			{
				get
				{
					return this.number;
				}
			}

			public SeparatedString(string baseString, int number)
			{
				this.baseString = baseString;
				this.number = number;
			}
		}
	}
}