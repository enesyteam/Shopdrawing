using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project.Templates
{
	public class TemplateItemHelper
	{
		private IProject project;

		private List<IProjectItemTemplate> templateItems;

		private IList<string> typeFilter;

		private IServiceProvider serviceProvider;

		public IEnumerable<IProjectItemTemplate> TemplateItems
		{
			get
			{
				if (this.templateItems == null)
				{
					this.templateItems = new List<IProjectItemTemplate>();
					foreach (IProjectItemTemplate projectItemTemplate in this.serviceProvider.ProjectManager().TemplateManager.ProjectItemTemplates)
					{
						if (this.typeFilter != null && !this.typeFilter.Contains(projectItemTemplate.DefaultName) || !this.project.IsValidItemTemplate(projectItemTemplate) || !this.serviceProvider.ProjectManager().TemplateManager.ShouldUseTemplate(this.project, projectItemTemplate))
						{
							continue;
						}
						this.templateItems.Add(projectItemTemplate);
					}
				}
				return this.templateItems;
			}
		}

		public TemplateItemHelper(IProject project, IList<string> typeFilter, IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
			this.project = project;
			this.typeFilter = typeFilter;
		}

		public IEnumerable<IProjectItem> AddProjectItemsForTemplateItem(IProjectItemTemplate templateItem, string fileName, string targetFolder, CreationOptions creationOptions, out List<IProjectItem> itemsToOpen)
		{
			IEnumerable<TemplateArgument> templateArguments;
			IProjectItemTemplate projectItemTemplate = templateItem;
			string str = fileName;
			string str1 = targetFolder;
			IProject project = this.project;
			if (this.project.TargetFramework == null)
			{
				templateArguments = Enumerable.Empty<TemplateArgument>();
			}
			else
			{
				TemplateArgument[] templateArgument = new TemplateArgument[] { new TemplateArgument("targetframeworkversion", this.project.TargetFramework.Version.ToString(2)) };
				templateArguments = templateArgument;
			}
			IEnumerable<TemplateArgument> templateArguments1 = templateArguments;
			CreationOptions creationOption = creationOptions;
			IServiceProvider serviceProvider = this.serviceProvider;
			return projectItemTemplate.CreateProjectItems(str, str1, project, templateArguments1, creationOption, out itemsToOpen, serviceProvider);
		}

		public IProjectItemTemplate FindTemplateItem(string name)
		{
			IProjectItemTemplate projectItemTemplate;
			using (IEnumerator<IProjectItemTemplate> enumerator = this.TemplateItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IProjectItemTemplate current = enumerator.Current;
					if (current.TemplateID != name)
					{
						continue;
					}
					projectItemTemplate = current;
					return projectItemTemplate;
				}
				return null;
			}
			return projectItemTemplate;
		}
	}
}