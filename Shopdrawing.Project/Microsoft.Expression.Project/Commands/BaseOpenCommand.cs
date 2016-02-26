using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Project.Commands
{
	internal abstract class BaseOpenCommand : ProjectCommand
	{
		public override bool IsEnabled
		{
			get
			{
				if (!base.IsEnabled)
				{
					return false;
				}
				return !BuildManager.Building;
			}
		}

		protected virtual IEnumerable<string> ProjectExtensions
		{
			get
			{
				List<string> strs = new List<string>();
				foreach (IDocumentType documentType in base.Services.DocumentTypes())
				{
					ICodeDocumentType codeDocumentType = documentType as ICodeDocumentType;
					if (codeDocumentType == null || string.IsNullOrEmpty(codeDocumentType.ProjectFileExtension))
					{
						continue;
					}
					string str = string.Concat("*", codeDocumentType.ProjectFileExtension);
					if (strs.Contains(str))
					{
						continue;
					}
					strs.Add(str);
				}
				return strs;
			}
		}

		public BaseOpenCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		private string GenerateFilterString(IEnumerable<string> extensions)
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			foreach (string extension in extensions)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("; ");
				}
				stringBuilder.Append(extension);
			}
			StringBuilder stringBuilder1 = new StringBuilder(128);
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			string interactiveProjectDocumentTypeDescription = StringTable.InteractiveProjectDocumentTypeDescription;
			object[] str = new object[] { stringBuilder.ToString() };
			stringBuilder1.AppendFormat(currentCulture, interactiveProjectDocumentTypeDescription, str);
			stringBuilder1.Append('|');
			stringBuilder1.Append(stringBuilder.ToString());
			return stringBuilder1.ToString();
		}

		protected string SelectProject(string dialogTitle)
		{
			string str;
			string defaultOpenProjectPath = this.ProjectManager().DefaultOpenProjectPath;
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(defaultOpenProjectPath))
			{
				defaultOpenProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			}
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(defaultOpenProjectPath))
			{
				defaultOpenProjectPath = Path.GetPathRoot(Environment.CurrentDirectory);
			}
			ExpressionOpenFileDialog expressionOpenFileDialog = new ExpressionOpenFileDialog()
			{
				Title = dialogTitle,
				RestoreDirectory = true,
				InitialDirectory = defaultOpenProjectPath
			};
			IEnumerable<string> projectExtensions = this.ProjectExtensions;
			expressionOpenFileDialog.Filter = this.GenerateFilterString(projectExtensions);
			bool? nullable = expressionOpenFileDialog.ShowDialog();
			Application.Current.MainWindow.Dispatcher.Invoke(DispatcherPriority.Input, new TimeSpan(0, 0, 3), new DispatcherOperationCallback((object arg) => null), null);
			if (nullable.HasValue && nullable.Value)
			{
				string fileName = expressionOpenFileDialog.FileName;
				string extension = Path.GetExtension(fileName);
				if (extension.Length > 0)
				{
					using (IEnumerator<string> enumerator = projectExtensions.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!enumerator.Current.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
							{
								continue;
							}
							str = fileName;
							return str;
						}
						goto Label0;
					}
					return str;
				}
			Label0:
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string extensionNotOpenableMessage = StringTable.ExtensionNotOpenableMessage;
				object[] objArray = new object[] { extension };
				this.DisplayCommandFailedMessage(string.Format(currentCulture, extensionNotOpenableMessage, objArray));
			}
			return null;
		}
	}
}