using Microsoft.Expression.Framework.Controls;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Project
{
	public interface ITemplateManager
	{
		IEnumerable<IProjectItemTemplate> ProjectItemTemplates
		{
			get;
		}

		IEnumerable<IProjectTemplate> ProjectTemplates
		{
			get;
		}

		IEnumerable<IProjectTemplate> SampleProjectTemplates
		{
			get;
		}

		Icon FindCategoryIcon(string name);

		bool ShouldUseTemplate(IProject project, ITemplate templateToTest);
	}
}