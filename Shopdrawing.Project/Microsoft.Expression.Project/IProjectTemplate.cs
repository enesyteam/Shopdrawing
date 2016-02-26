using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Project
{
	public interface IProjectTemplate : ITemplate
	{
		bool CreateNewFolder
		{
			get;
		}

		bool HasMultipleProjects
		{
			get;
		}

		bool HasProjectFile
		{
			get;
		}

		bool IsUserCreatable
		{
			get;
		}

		string ProjectFilename
		{
			get;
		}

		IEnumerable<INamedProject> CreateProjects(string name, string path, IEnumerable<TemplateArgument> templateArguments, IServiceProvider serviceProvider);

		bool IsPlatformSupported(out string warningMessage);

		ProjectPropertyValue PreferredPropertyValue(string property);

		List<ProjectPropertyValue> ValidPropertyValues(string property);
	}
}