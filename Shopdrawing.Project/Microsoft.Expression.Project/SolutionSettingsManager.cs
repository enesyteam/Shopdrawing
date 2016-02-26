using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public sealed class SolutionSettingsManager
	{
		private readonly static string ProjectSettingsObject;

		private readonly static string RelativeReference;

		private readonly static string SolutionSettingsObject;

		private ISolution solution;

		private IConfigurationObject configObject;

		private IConfigurationObjectCollection projectConfigs;

		public IConfigurationObject SolutionSettings
		{
			get
			{
				return this.configObject.GetOrCreateConfigurationObjectProperty(SolutionSettingsManager.SolutionSettingsObject);
			}
		}

		static SolutionSettingsManager()
		{
			SolutionSettingsManager.ProjectSettingsObject = "ProjectSettings";
			SolutionSettingsManager.RelativeReference = "RelativeReference";
			SolutionSettingsManager.SolutionSettingsObject = "SolutionSettings";
		}

		internal SolutionSettingsManager(ISolution solution, IConfigurationObject configObject)
		{
			this.solution = solution;
			this.configObject = configObject;
			this.projectConfigs = this.configObject.GetOrCreateConfigurationObjectCollectionProperty(SolutionSettingsManager.ProjectSettingsObject);
		}

		public void ClearProjectProperty(INamedProject project, string propertyName)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}
			this.GetConfigurationForProject(project).ClearProperty(propertyName);
		}

		public void FilterProjectSettings(IEnumerable<INamedProject> projects)
		{
			IEnumerable<IConfigurationObject> configurationObjects;
			if (projects != null)
			{
				configurationObjects = 
					from project in projects
					select this.GetConfigurationForProject(project);
			}
			else
			{
				configurationObjects = null;
			}
			IEnumerable<IConfigurationObject> configurationObjects1 = configurationObjects;
			IConfigurationObjectCollection configurationObjectCollections = this.configObject.CreateConfigurationObjectCollection();
			foreach (IConfigurationObject configurationObject in configurationObjects1)
			{
				configurationObjectCollections.Add(configurationObject);
			}
			this.configObject.SetProperty(SolutionSettingsManager.ProjectSettingsObject, configurationObjectCollections);
		}

		private IConfigurationObject GetConfigurationForProject(INamedProject project)
		{
			IConfigurationObject configurationObject;
			string relativeReference = this.GetRelativeReference(project);
			IEnumerator enumerator = this.projectConfigs.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					IConfigurationObject current = (IConfigurationObject)enumerator.Current;
					if (string.CompareOrdinal(relativeReference, current.GetPropertyOrDefault<string>(SolutionSettingsManager.RelativeReference).ToUpperInvariant()) != 0)
					{
						continue;
					}
					configurationObject = current;
					return configurationObject;
				}
				IConfigurationObject configurationObject1 = this.configObject.CreateConfigurationObject();
				configurationObject1.SetProperty(SolutionSettingsManager.RelativeReference, relativeReference);
				this.projectConfigs.Add(configurationObject1);
				return configurationObject1;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return configurationObject;
		}

		public object GetProjectProperty(INamedProject project, string propertyName)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}
			return this.GetConfigurationForProject(project).GetPropertyOrDefault<object>(propertyName);
		}

		private string GetRelativeReference(INamedProject project)
		{
			return this.solution.DocumentReference.GetRelativePath(project.DocumentReference).ToUpperInvariant();
		}

		public void SetProjectProperty(INamedProject project, string propertyName, object value)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName");
			}
			this.GetConfigurationForProject(project).SetProperty(propertyName, value);
		}
	}
}