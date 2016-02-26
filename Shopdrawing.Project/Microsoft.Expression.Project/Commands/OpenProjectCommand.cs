using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class OpenProjectCommand : BaseOpenCommand
	{
		public override string DisplayName
		{
			get
			{
				return StringTable.CommandOpenProjectName;
			}
		}

		protected override IEnumerable<string> ProjectExtensions
		{
			get
			{
				List<string> strs = new List<string>()
				{
					"*.sln"
				};
				strs.AddRange(base.ProjectExtensions);
				return strs;
			}
		}

		public OpenProjectCommand(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				string str = base.SelectProject(StringTable.OpenProjectDialogTitle);
				if (!string.IsNullOrEmpty(str))
				{
					PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.OpenProjectInner);
					ISolution solution = this.ProjectManager().OpenSolution(DocumentReference.Create(str), true, true);
					PerformanceUtility.MarkInterimStep(PerformanceEvent.OpenProjectInner, "Finished loading, now closing all projects.");
					if (solution != null)
					{
						this.ProjectManager().DefaultOpenProjectPath = Path.GetDirectoryName(Path.GetDirectoryName(solution.DocumentReference.Path));
					}
				}
			});
		}
	}
}