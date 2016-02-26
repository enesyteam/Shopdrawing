using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public class ProjectItemRenameException : Exception
	{
		public string AttemptedName
		{
			get;
			private set;
		}

		public ProjectItemRenameError Error
		{
			get;
			private set;
		}

		public IProjectItem ProjectItem
		{
			get;
			private set;
		}

		public ProjectItemRenameException(ProjectItemRenameError error, IProjectItem projectItem, string attemptedName) : this(null, error, projectItem, attemptedName)
		{
		}

		public ProjectItemRenameException(Exception innerException, ProjectItemRenameError error, IProjectItem projectItem, string attemptedName) : base(ProjectItemRenameException.GetMessage(error, innerException), innerException)
		{
			this.Init(error, projectItem, attemptedName);
		}

		private static string GetMessage(ProjectItemRenameError error, Exception innerException)
		{
			switch (error)
			{
				case ProjectItemRenameError.EmptyString:
				{
					return StringTable.RenameProjectItemFileNameEmptyError;
				}
				case ProjectItemRenameError.StartsWithPeriod:
				{
					return StringTable.RenameProjectItemFileNameStartsWithPeriodError;
				}
				case ProjectItemRenameError.ContainsInvalidCharacters:
				{
					return StringTable.RenameProjectItemFileNameContainsInvalidCharactersError;
				}
				case ProjectItemRenameError.IsReservedName:
				{
					return StringTable.RenameProjectItemFileNameIsReservedNameError;
				}
				case ProjectItemRenameError.DuplicateName:
				{
					return StringTable.RenameProjectItemDuplicateName;
				}
				case ProjectItemRenameError.Exception:
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string renameProjectItemRenameFailed = StringTable.RenameProjectItemRenameFailed;
					object[] message = new object[] { innerException.Message };
					return string.Format(currentCulture, renameProjectItemRenameFailed, message);
				}
			}
			return string.Empty;
		}

		private void Init(ProjectItemRenameError error, IProjectItem projectItem, string attemptedName)
		{
			this.Error = error;
			this.ProjectItem = projectItem;
			this.AttemptedName = attemptedName;
		}
	}
}