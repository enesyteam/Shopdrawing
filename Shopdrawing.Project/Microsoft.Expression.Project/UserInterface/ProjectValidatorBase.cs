using System;
using System.Globalization;

namespace Microsoft.Expression.Project.UserInterface
{
	public abstract class ProjectValidatorBase
	{
		protected ProjectValidatorBase()
		{
		}

		protected static bool IsAllDots(string fileName)
		{
			string str = fileName;
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] != '.')
				{
					return false;
				}
			}
			return true;
		}

		internal static string ValidateWithErrorString(string projectName, char[] invalidCharacters)
		{
			string projectDialogEmptyNameError = null;
			int num = projectName.IndexOfAny(invalidCharacters);
			if (num < 0)
			{
				projectName = projectName.Trim();
				if (projectName.Length == 0)
				{
					projectDialogEmptyNameError = StringTable.ProjectDialogEmptyNameError;
				}
				else if (!ProjectValidatorBase.IsAllDots(projectName))
				{
					int num1 = projectName.IndexOf('.');
					if (num1 >= 0)
					{
						projectName = projectName.Substring(0, num1);
						projectName = projectName.Trim();
					}
					if (projectName.Length == 0 || Array.IndexOf<string>(ProjectDialog.ReservedNames, projectName.ToUpperInvariant()) >= 0)
					{
						projectDialogEmptyNameError = StringTable.ProjectDialogFileNameIsReservedNameError;
					}
				}
				else
				{
					projectDialogEmptyNameError = StringTable.ProjectDialogFileNameIsReservedNameError;
				}
			}
			else
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string projectDialogFileNameContainsInvalidCharacterError = StringTable.ProjectDialogFileNameContainsInvalidCharacterError;
				object[] objArray = new object[] { projectName[num] };
				projectDialogEmptyNameError = string.Format(currentCulture, projectDialogFileNameContainsInvalidCharacterError, objArray);
			}
			return projectDialogEmptyNameError;
		}
	}
}