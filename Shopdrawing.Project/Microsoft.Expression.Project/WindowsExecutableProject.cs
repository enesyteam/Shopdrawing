using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public class WindowsExecutableProject : ExecutableProjectBase
	{
		private bool? isControlLibrary;

		protected override bool IsControlLibrary
		{
			get
			{
				if (!this.isControlLibrary.HasValue)
				{
					string property = base.ProjectStore.GetProperty("OutputType");
					this.isControlLibrary = new bool?(property.Equals("Library", StringComparison.OrdinalIgnoreCase));
				}
				return this.isControlLibrary.Value;
			}
		}

		public override string StartArguments
		{
			get
			{
				if (StringComparer.OrdinalIgnoreCase.Compare(base.GetEvaluatedPropertyValue("StartAction"), "Program") == 0)
				{
					return base.GetEvaluatedPropertyValue("StartArguments");
				}
				string fullTargetPath = this.FullTargetPath;
				if (string.IsNullOrEmpty(fullTargetPath) || StringComparer.OrdinalIgnoreCase.Compare(Path.GetExtension(fullTargetPath), ".xbap") != 0)
				{
					return null;
				}
				return string.Concat("-debug \"", fullTargetPath, "\"");
			}
		}

		public override string StartProgram
		{
			get
			{
				if (this.IsControlLibrary)
				{
					return null;
				}
				if (StringComparer.OrdinalIgnoreCase.Compare(base.GetEvaluatedPropertyValue("StartAction"), "Program") == 0)
				{
					return base.GetEvaluatedPropertyValue("StartProgram");
				}
				string fullTargetPath = this.FullTargetPath;
				if (string.IsNullOrEmpty(fullTargetPath) || StringComparer.OrdinalIgnoreCase.Compare(Path.GetExtension(fullTargetPath), ".xbap") != 0)
				{
					return fullTargetPath;
				}
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "PresentationHost.exe");
			}
		}

		protected WindowsExecutableProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider) : base(projectStore, codeDocumentType, projectType, serviceProvider)
		{
		}

		public static IProject Create(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider)
		{
			return KnownProjectBase.TryCreate(() => new WindowsExecutableProject(projectStore, codeDocumentType, projectType, serviceProvider));
		}
	}
}