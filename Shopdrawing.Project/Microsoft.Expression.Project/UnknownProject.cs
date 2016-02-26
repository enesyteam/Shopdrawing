using System;

namespace Microsoft.Expression.Project
{
	public sealed class UnknownProject : ProjectBase
	{
		public UnknownProject(IProjectStore projectStore, IServiceProvider serviceProvider) : base(projectStore, serviceProvider)
		{
		}
	}
}