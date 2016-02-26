using System;

namespace Microsoft.Expression.Project
{
	internal class UnlicensedProject : ProjectBase
	{
		public UnlicensedProject(IProjectStore projectStore, IServiceProvider serviceProvider) : base(projectStore, serviceProvider)
		{
		}
	}
}