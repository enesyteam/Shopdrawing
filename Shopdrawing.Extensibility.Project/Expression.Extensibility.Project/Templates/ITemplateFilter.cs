using Microsoft.Expression.Extensibility.Project;
using System;

namespace Microsoft.Expression.Extensibility.Project.Templates
{
	public interface ITemplateFilter
	{
		int Version
		{
			get;
		}

		bool ShouldLoad(VSTemplate vsTemplate);

		bool ShouldUseTemplate(IProject project, VSTemplate vsTemplate);
	}
}