using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Documents;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Microsoft.Expression.Project.Build
{
	public sealed class BuildErrorTask : IErrorTask
	{
		private string file;

		private IProjectManager projectManager;

		private IProject projectReference;

		private string fullFileName;

		private string displayFileName;

		public int? Column
		{
			get
			{
				return JustDecompileGenerated_get_Column();
			}
			set
			{
				JustDecompileGenerated_set_Column(value);
			}
		}

		private int? JustDecompileGenerated_Column_k__BackingField;

		public int? JustDecompileGenerated_get_Column()
		{
			return this.JustDecompileGenerated_Column_k__BackingField;
		}

		private void JustDecompileGenerated_set_Column(int? value)
		{
			this.JustDecompileGenerated_Column_k__BackingField = value;
		}

		public string Description
		{
			get
			{
				return JustDecompileGenerated_get_Description();
			}
			set
			{
				JustDecompileGenerated_set_Description(value);
			}
		}

		private string JustDecompileGenerated_Description_k__BackingField;

		public string JustDecompileGenerated_get_Description()
		{
			return this.JustDecompileGenerated_Description_k__BackingField;
		}

		private void JustDecompileGenerated_set_Description(string value)
		{
			this.JustDecompileGenerated_Description_k__BackingField = value;
		}

		public string ExtendedDescription
		{
			get
			{
				return this.Description;
			}
		}

		public string File
		{
			get
			{
				if (this.displayFileName == null)
				{
					this.displayFileName = this.CreateDisplayFileName();
				}
				return this.displayFileName;
			}
		}

		public string FullFileName
		{
			get
			{
				if (this.fullFileName == null)
				{
					this.fullFileName = this.CreateFullFileName();
				}
				return this.fullFileName;
			}
		}

		public ICommand InvokeCommand
		{
			get
			{
				return new DelegateCommand(() => {
					if (this.projectReference != null)
					{
						string directoryNameOrRoot = PathHelper.GetDirectoryNameOrRoot(this.projectReference.DocumentReference.Path);
						DocumentReference documentReference = null;
						if (!PathHelper.IsPathRelative(this.file))
						{
							documentReference = DocumentReference.Create(this.file);
						}
						else
						{
							try
							{
								documentReference = DocumentReference.Create(PathHelper.ResolveCombinedPath(directoryNameOrRoot, this.file));
							}
							catch (ArgumentException argumentException)
							{
							}
						}
						IProjectItem projectItem = (documentReference != null ? this.projectReference.FindItem(documentReference) : null);
						if (projectItem != null)
						{
							IDocumentView documentView = projectItem.OpenView(true);
							if (documentView != null)
							{
								ISetCaretPosition setCaretPosition = documentView as ISetCaretPosition;
								if (setCaretPosition != null)
								{
									setCaretPosition.SetCaretPosition(this.Line.Value, this.Column.Value);
								}
							}
						}
					}
				});
			}
		}

		public int? Line
		{
			get
			{
				return JustDecompileGenerated_get_Line();
			}
			set
			{
				JustDecompileGenerated_set_Line(value);
			}
		}

		private int? JustDecompileGenerated_Line_k__BackingField;

		public int? JustDecompileGenerated_get_Line()
		{
			return this.JustDecompileGenerated_Line_k__BackingField;
		}

		private void JustDecompileGenerated_set_Line(int? value)
		{
			this.JustDecompileGenerated_Line_k__BackingField = value;
		}

		public string Project
		{
			get
			{
				if (this.projectReference == null)
				{
					return string.Empty;
				}
				return this.projectReference.Name;
			}
		}

		public ErrorSeverity Severity
		{
			get
			{
				return JustDecompileGenerated_get_Severity();
			}
			set
			{
				JustDecompileGenerated_set_Severity(value);
			}
		}

		private ErrorSeverity JustDecompileGenerated_Severity_k__BackingField;

		public ErrorSeverity JustDecompileGenerated_get_Severity()
		{
			return this.JustDecompileGenerated_Severity_k__BackingField;
		}

		private void JustDecompileGenerated_set_Severity(ErrorSeverity value)
		{
			this.JustDecompileGenerated_Severity_k__BackingField = value;
		}

		public BuildErrorTask(ErrorSeverity severity, string description, string projectPath, string file, int line, int column, IServiceProvider serviceProvider)
		{
			this.Severity = severity;
			this.Description = description;
			this.file = file;
			this.Line = new int?(line);
			this.Column = new int?(column);
			this.projectManager = (IProjectManager)serviceProvider.GetService(typeof(IProjectManager));
			this.projectReference = this.GetProjectReference(projectPath);
		}

		private string CreateDisplayFileName()
		{
			return PathHelper.GetFileOrDirectoryName(this.file);
		}

		private string CreateFullFileName()
		{
			if (!PathHelper.IsPathRelative(this.file) || this.projectReference == null)
			{
				return this.file;
			}
			return PathHelper.ResolveCombinedPath(PathHelper.GetDirectoryNameOrRoot(this.projectReference.DocumentReference.Path), this.file);
		}

		private IProject GetProjectReference(string projectPath)
		{
			if (this.projectManager != null && !string.IsNullOrEmpty(projectPath))
			{
				ISolution currentSolution = this.projectManager.CurrentSolution;
				if (currentSolution != null)
				{
					IProject project = currentSolution.Projects.FindMatchByUrl<IProject>(projectPath);
					if (project == null)
					{
						string directoryNameOrRoot = PathHelper.GetDirectoryNameOrRoot(projectPath);
						project = currentSolution.Projects.FirstOrDefault<IProject>((IProject potentialProject) => PathHelper.ArePathsEquivalent(directoryNameOrRoot, PathHelper.GetDirectoryNameOrRoot(potentialProject.DocumentReference.Path)));
					}
					return project;
				}
			}
			return null;
		}
	}
}