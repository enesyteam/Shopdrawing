using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Project.UserInterface
{
	public class ProjectNameValidator : ProjectValidatorBase, IMessageBubbleValidator
	{
		private TextBox targetElement;

		private IMessageBubbleHelper helper;

		public ProjectNameValidator()
		{
		}

		private static char[] GetInvalidFileNameChars()
		{
			List<char> chrs = new List<char>(Path.GetInvalidFileNameChars())
			{
				';',
				'&',
				'#',
				'%'
			};
			return chrs.ToArray();
		}

		public void Initialize(UIElement targetElement, IMessageBubbleHelper helper)
		{
			this.targetElement = (TextBox)targetElement;
			this.targetElement.TextChanged += new TextChangedEventHandler(this.ProjectNameValidator_TextChanged);
			this.helper = helper;
		}

		private void ProjectNameValidator_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.helper.SetContent(this.Validate(this.targetElement.Text));
		}

		protected virtual MessageBubbleContent Validate(string name)
		{
			string str = ProjectNameValidator.ValidateWithErrorString(name);
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			return new MessageBubbleContent(str, MessageBubbleType.Error);
		}

		public static bool ValidateName(string name)
		{
			return ProjectNameValidator.ValidateWithErrorString(name) == null;
		}

		public static string ValidateWithErrorString(string projectName)
		{
			return ProjectValidatorBase.ValidateWithErrorString(projectName, ProjectNameValidator.GetInvalidFileNameChars());
		}
	}
}