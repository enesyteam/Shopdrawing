using Microsoft.Build.Evaluation;
using System;

namespace Microsoft.Expression.Project
{
	internal interface IMSBuildItemInternal
	{
		Microsoft.Build.Evaluation.ProjectItem BuildItem
		{
			get;
			set;
		}
	}
}