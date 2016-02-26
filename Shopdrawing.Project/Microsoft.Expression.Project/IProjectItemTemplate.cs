using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Project
{
	public interface IProjectItemTemplate : ITemplate
	{
		IEnumerable<IProjectItem> CreateProjectItems(string name, string targetFolder, IProject project, IEnumerable<TemplateArgument> templateArguments, CreationOptions creationOptions, out List<IProjectItem> itemsToOpen, IServiceProvider serviceProvider);

		string FindAvailableDefaultName(string targetFolder, IExpressionInformationService expressionInformationService);
	}
}