using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;

namespace Microsoft.Expression.Project.UserInterface
{
	public class ProjectItemNameValidator : ProjectNameValidator
	{
		private IProject project;

		public ProjectItemNameValidator(IProject project)
		{
			this.project = project;
		}

		protected override MessageBubbleContent Validate(string name)
		{
			string str = ProjectItemNameValidator.ValidateWithErrorString(this.project, name);
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			return new MessageBubbleContent(str, MessageBubbleType.Error);
		}

		public static bool ValidateName(IProject project, string name)
		{
			return ProjectItemNameValidator.ValidateWithErrorString(project, name) == null;
		}

		public static string ValidateWithErrorString(IProject project, string projectName)
		{
			string projectItemDialogFileNameMatchesNamespace = ProjectNameValidator.ValidateWithErrorString(projectName);
			if (projectItemDialogFileNameMatchesNamespace == null && project != null)
			{
				string str = string.Concat(project.DefaultNamespaceName, ".xaml");
				if (projectName == project.DefaultNamespaceName || projectName == str)
				{
					projectItemDialogFileNameMatchesNamespace = StringTable.ProjectItemDialogFileNameMatchesNamespace;
				}
			}
			return projectItemDialogFileNameMatchesNamespace;
		}
	}
}