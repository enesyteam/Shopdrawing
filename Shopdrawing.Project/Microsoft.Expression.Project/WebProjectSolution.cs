using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project
{
	internal class WebProjectSolution : SingleProjectSolution
	{
		public override bool CanAddProjects
		{
			get
			{
				return false;
			}
		}

		internal WebProjectSolution(IServiceProvider serviceProvider, Microsoft.Expression.Framework.Documents.DocumentReference documentReference) : base(serviceProvider, documentReference)
		{
		}
	}
}