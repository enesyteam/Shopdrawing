using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Extensions;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Project.Conversion
{
	internal abstract class ProjectConverterBase : IProjectConverter
	{
		private ISolutionManagement solution;

		private IConfigurationObject configurationObject;

		private bool attemptedToGetConfigurationObject;

		private readonly static string SuoUpgraderCollection;

		private readonly static string SuoUpgraderIdentifier;

		private readonly static string SuoUpgraderDisabledProperty;

		public virtual bool AllowDisabling
		{
			get
			{
				return true;
			}
		}

		private IConfigurationObject ConfigurationObject
		{
			get
			{
				if (!this.attemptedToGetConfigurationObject)
				{
					this.configurationObject = this.GetConfigurationObject();
					this.attemptedToGetConfigurationObject = true;
				}
				return this.configurationObject;
			}
		}

		protected IProjectActionContext Context
		{
			get;
			private set;
		}

		public virtual string ConversionPrompt
		{
			get
			{
				return string.Empty;
			}
		}

		public abstract string Identifier
		{
			get;
		}

		public bool IsEnabled
		{
			get
			{
				if (this.ConfigurationObject == null)
				{
					return true;
				}
				return !this.ConfigurationObject.GetPropertyOrDefault<bool>(ProjectConverterBase.SuoUpgraderDisabledProperty);
			}
			set
			{
				if (this.ConfigurationObject != null && this.AllowDisabling)
				{
					this.ConfigurationObject.SetProperty(ProjectConverterBase.SuoUpgraderDisabledProperty, !value);
				}
			}
		}

		protected IServiceProvider Services
		{
			get
			{
				return this.Context.ServiceProvider;
			}
		}

		protected ISolutionManagement Solution
		{
			get
			{
				if (this.solution != null)
				{
					return this.solution;
				}
				return this.Services.ProjectManager().CurrentSolution as ISolutionManagement;
			}
		}

		static ProjectConverterBase()
		{
			ProjectConverterBase.SuoUpgraderCollection = "UpgraderCollection";
			ProjectConverterBase.SuoUpgraderIdentifier = "UpgraderIdentifier";
			ProjectConverterBase.SuoUpgraderDisabledProperty = "UpgraderDisabled";
		}

		public ProjectConverterBase(ISolutionManagement solution, IServiceProvider serviceProvider)
		{
			this.solution = solution;
			this.Context = new ProjectUpgradeContext(serviceProvider);
		}

		protected bool AttemptToMakeWriteable(DocumentReference documentReference)
		{
			return ProjectPathHelper.AttemptToMakeWritable(documentReference, this.Context);
		}

		public bool Convert(ConversionTarget file, ConversionType initialState, ConversionType targetState)
		{
			return this.InternalConvert(file, initialState, targetState);
		}

		internal static AssemblyName GetAssemblyName(string assemblyReference)
		{
			AssemblyName assemblyName;
			try
			{
				assemblyName = new AssemblyName(assemblyReference);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (!(exception is FileLoadException) && !(exception is FileNotFoundException) && !(exception is COMException) && !(exception is ArgumentException))
				{
					throw;
				}
				return null;
			}
			return assemblyName;
		}

		private IConfigurationObject GetConfigurationObject()
		{
			IConfigurationObject solutionSettings = this.Solution.SolutionSettingsManager.SolutionSettings;
			IConfigurationObjectCollection orCreateConfigurationObjectCollectionProperty = solutionSettings.GetOrCreateConfigurationObjectCollectionProperty(ProjectConverterBase.SuoUpgraderCollection);
			IConfigurationObject configurationObject = null;
			foreach (IConfigurationObject configurationObject1 in orCreateConfigurationObjectCollectionProperty)
			{
				if (string.CompareOrdinal(this.Identifier, configurationObject1.GetPropertyOrDefault<string>(ProjectConverterBase.SuoUpgraderIdentifier)) != 0)
				{
					continue;
				}
				configurationObject = configurationObject1;
				break;
			}
			if (configurationObject == null)
			{
				configurationObject = solutionSettings.CreateConfigurationObject();
				configurationObject.SetProperty(ProjectConverterBase.SuoUpgraderIdentifier, this.Identifier);
				orCreateConfigurationObjectCollectionProperty.Add(configurationObject);
			}
			return configurationObject;
		}

		public abstract ConversionType GetVersion(ConversionTarget project);

		protected abstract bool InternalConvert(ConversionTarget file, ConversionType initialState, ConversionType targetState);
	}
}