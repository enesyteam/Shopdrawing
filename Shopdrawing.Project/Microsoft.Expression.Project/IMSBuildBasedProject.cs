using System;

namespace Microsoft.Expression.Project
{
	internal interface IMSBuildBasedProject
	{
		void OnBuildItemChanged();
	}
}