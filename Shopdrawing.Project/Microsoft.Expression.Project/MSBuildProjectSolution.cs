using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project
{
	internal class MSBuildProjectSolution : SingleProjectSolution
	{
		public override bool CanAddProjects
		{
			get
			{
				return true;
			}
		}

		internal MSBuildProjectSolution(IServiceProvider serviceProvider, Microsoft.Expression.Framework.Documents.DocumentReference documentReference) : base(serviceProvider, documentReference)
		{
		}
	}
}