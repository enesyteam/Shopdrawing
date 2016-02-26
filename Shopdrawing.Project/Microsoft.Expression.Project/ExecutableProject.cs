using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public sealed class ExecutableProject : ExecutableProjectBase
	{
		public override string StartArguments
		{
			get
			{
				if (StringComparer.OrdinalIgnoreCase.Compare(base.GetEvaluatedPropertyValue("StartAction"), "Program") != 0)
				{
					return null;
				}
				return base.GetEvaluatedPropertyValue("StartArguments");
			}
		}

		public override string StartProgram
		{
			get
			{
				if (StringComparer.OrdinalIgnoreCase.Compare(base.GetEvaluatedPropertyValue("StartAction"), "Program") != 0)
				{
					return this.FullTargetPath;
				}
				return base.GetEvaluatedPropertyValue("StartProgram");
			}
		}

		private ExecutableProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider) : base(projectStore, codeDocumentType, projectType, serviceProvider)
		{
		}

		public static IProject Create(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider)
		{
			return KnownProjectBase.TryCreate(() => new ExecutableProject(projectStore, codeDocumentType, projectType, serviceProvider));
		}
	}
}