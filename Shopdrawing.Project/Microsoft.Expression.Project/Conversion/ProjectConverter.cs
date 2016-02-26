using Microsoft.Expression.Project;
using System;

namespace Microsoft.Expression.Project.Conversion
{
	internal abstract class ProjectConverter : ProjectConverterBase
	{
		public ProjectConverter(ISolutionManagement solution, IServiceProvider serviceProvider) : base(solution, serviceProvider)
		{
		}

		protected sealed override bool InternalConvert(ConversionTarget file, ConversionType initialState, ConversionType targetState)
		{
			if (this.GetVersion(file) == targetState)
			{
				return true;
			}
			if (!ProjectPathHelper.AttemptToMakeWritable(file.ProjectStore.DocumentReference, base.Context))
			{
				return false;
			}
			if (!this.UpgradeProject(file.ProjectStore, initialState, targetState))
			{
				return false;
			}
			file.ProjectStore.Save();
			return true;
		}

		protected abstract bool UpgradeProject(IProjectStore projectStore, ConversionType initialVersion, ConversionType targetVersion);
	}
}