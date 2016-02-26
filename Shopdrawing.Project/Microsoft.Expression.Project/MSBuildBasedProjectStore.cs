using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.Expression.Project
{
	internal sealed class MSBuildBasedProjectStore : ProjectStoreBase
	{
		private const string ExpressionBlendVersionPropertyName = "ExpressionBlendVersion";

		private string blendVersion;

		private static string[] KnownMSBuildExtensions;

		internal Microsoft.Build.Evaluation.Project MsBuildProject
		{
			get;
			private set;
		}

		public override IEnumerable<string> ProjectImports
		{
			get
			{
				foreach (ResolvedImport resolvedImport in this.MsBuildProject.Imports)
				{
					yield return resolvedImport.ImportingElement.Project;
				}
			}
		}

		public override Version StoreVersion
		{
			get
			{
				Version version;
				if (Version.TryParse(this.MsBuildProject.ToolsVersion, out version))
				{
					return version;
				}
				return null;
			}
		}

		static MSBuildBasedProjectStore()
		{
			string[] strArrays = new string[] { ".CSPROJ", ".VBPROJ", ".VCXPROJ", ".FSPROJ" };
			MSBuildBasedProjectStore.KnownMSBuildExtensions = strArrays;
		}

		private MSBuildBasedProjectStore(Microsoft.Expression.Framework.Documents.DocumentReference documentReference) : base(documentReference)
		{
		}

		public override bool AddImport(string importValue)
		{
			bool flag = this.MsBuildProject.Xml.AddImport(importValue) != null;
			if (!flag)
			{
				string path = base.DocumentReference.Path;
				string unknownError = StringTable.UnknownError;
				string addImportAction = StringTable.AddImportAction;
				object[] objArray = new object[] { importValue };
				ProjectLog.LogError(path, unknownError, addImportAction, objArray);
			}
			else
			{
				ProjectLog.LogSuccess(base.DocumentReference.Path, StringTable.AddImportAction, new object[] { importValue });
			}
			return flag;
		}

		public override IProjectItemData AddItem(string itemType, string itemValue)
		{
			Microsoft.Build.Evaluation.ProjectItem projectItem = this.AddMsBuildItem(itemType, itemValue);
			if (projectItem == null)
			{
				string path = base.DocumentReference.Path;
				string unknownError = StringTable.UnknownError;
				string addProjectItemAction = StringTable.AddProjectItemAction;
				object[] objArray = new object[] { itemType, itemValue };
				ProjectLog.LogError(path, unknownError, addProjectItemAction, objArray);
			}
			else
			{
				string str = base.DocumentReference.Path;
				string addProjectItemAction1 = StringTable.AddProjectItemAction;
				object[] objArray1 = new object[] { itemType, itemValue };
				ProjectLog.LogSuccess(str, addProjectItemAction1, objArray1);
			}
			if (projectItem == null)
			{
				return null;
			}
			return new MSBuildProjectItemData(projectItem);
		}

		internal Microsoft.Build.Evaluation.ProjectItem AddMsBuildItem(string itemType, string value)
		{
			string str = ProjectCollection.Escape(value);
			IList<Microsoft.Build.Evaluation.ProjectItem> projectItems = this.MsBuildProject.AddItem(itemType, str);
			if (projectItems.Count == 0)
			{
				return null;
			}
			if (projectItems.Count > 1)
			{
				throw new InvalidOperationException();
			}
			return projectItems[0];
		}

		private Microsoft.Expression.Project.Build.BuildResult Build(ProjectInstance projectInstance, IEnumerable<ILogger> loggers, params string[] targetNames)
		{
			IDictionary<string, TargetResult> strs = new Dictionary<string, TargetResult>();
			if (projectInstance.Build(targetNames, loggers, null, out strs))
			{
				return Microsoft.Expression.Project.Build.BuildResult.Succeeded;
			}
			return Microsoft.Expression.Project.Build.BuildResult.Failed;
		}

		public override bool ChangeImport(string oldImportValue, string newImportValue)
		{
			bool flag;
			if (string.Equals(oldImportValue, newImportValue))
			{
				return true;
			}
			using (IEnumerator<ResolvedImport> enumerator = this.MsBuildProject.Imports.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ResolvedImport current = enumerator.Current;
					if (string.Compare(current.ImportingElement.Project, oldImportValue, StringComparison.OrdinalIgnoreCase) != 0)
					{
						continue;
					}
					current.ImportingElement.Project = newImportValue;
					string path = base.DocumentReference.Path;
					string updateImportAction = StringTable.UpdateImportAction;
					object[] objArray = new object[] { oldImportValue, newImportValue };
					ProjectLog.LogSuccess(path, updateImportAction, objArray);
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public static IProjectStore CreateInstance(Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			if (documentReference == null)
			{
				throw new ArgumentNullException("documentReference");
			}
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			if (!documentReference.IsValidPathFormat)
			{
				throw new ArgumentException("Document reference must be a valid path.", "documentReference");
			}
			if (!File.Exists(documentReference.Path))
			{
				throw new FileNotFoundException("File not found.", documentReference.Path);
			}
			string safeExtension = Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(documentReference.Path);
			if (string.IsNullOrEmpty(safeExtension))
			{
				return null;
			}
			if (!MSBuildBasedProjectStore.KnownMSBuildExtensions.Contains<string>(safeExtension.ToUpperInvariant()))
			{
				return null;
			}
			MSBuildBasedProjectStore mSBuildBasedProjectStore = new MSBuildBasedProjectStore(documentReference);
			mSBuildBasedProjectStore.LoadProject();
			mSBuildBasedProjectStore.blendVersion = (serviceProvider.ExpressionInformationService() == null ? "0.0.0.0" : serviceProvider.ExpressionInformationService().Version.ToString());
			return mSBuildBasedProjectStore;
		}

		private ProjectInstance CreateProjectInstance()
		{
			base.SetUnpersistedProperty(MSBuildBasedProject.BuildingInBlendPropertyName, "true");
			base.SetProperty("Utf8Output", "true");
			return this.MsBuildProject.CreateProjectInstance();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.MsBuildProject != null)
			{
				this.UnloadProject();
				this.MsBuildProject = null;
			}
		}

		internal ProjectBuildContext GenerateNewBuildContext(Action<Microsoft.Expression.Project.Build.BuildResult> buildCompleteCallback)
		{
			Func<ProjectInstance, IEnumerable<ILogger>, string[], Microsoft.Expression.Project.Build.BuildResult> func = new Func<ProjectInstance, IEnumerable<ILogger>, string[], Microsoft.Expression.Project.Build.BuildResult>(this.Build);
			Action<Microsoft.Expression.Project.Build.BuildResult> action = buildCompleteCallback;
			Func<ProjectInstance> func1 = new Func<ProjectInstance>(this.CreateProjectInstance);
			IEnumerable<ILogger> loggers = Enumerable.Empty<ILogger>();
			string fileOrDirectoryName = Microsoft.Expression.Framework.Documents.PathHelper.GetFileOrDirectoryName(base.DocumentReference.Path);
			IErrorTaskCollection errorTaskCollections = new ErrorTaskCollection();
			return ProjectBuildContext.CreateBuildContext(func, action, func1, loggers, fileOrDirectoryName, errorTaskCollections);
		}

		public override IEnumerable<IProjectItemData> GetItems(string itemType)
		{
			if (itemType == null)
			{
				throw new ArgumentNullException("itemType");
			}
			return this.ItemEnumerator(itemType);
		}

		public override string GetProperty(string name)
		{
			ProjectProperty property = this.MsBuildProject.GetProperty(name);
			if (property == null)
			{
				return null;
			}
			return property.EvaluatedValue;
		}

		private bool IsPropertyWritable(ProjectProperty projectProperty)
		{
			if (projectProperty != null && (projectProperty.IsReservedProperty || projectProperty.IsGlobalProperty))
			{
				return false;
			}
			return true;
		}

		public override bool IsPropertyWritable(string name)
		{
			return this.IsPropertyWritable(this.MsBuildProject.GetProperty(name));
		}

		private IEnumerable<IProjectItemData> ItemEnumerator(string itemType)
		{
			foreach (Microsoft.Build.Evaluation.ProjectItem projectItem in this.MsBuildProject.GetItems(itemType))
			{
				yield return new MSBuildProjectItemData(projectItem);
			}
		}

		private void LoadProject()
		{
			try
			{
				this.MsBuildProject = Microsoft.Expression.Project.Build.BuildManager.GetProject(base.DocumentReference);
			}
			catch (InvalidProjectFileException invalidProjectFileException)
			{
				base.Dispose();
				throw;
			}
		}

		public override bool RemoveItem(IProjectItemData item)
		{
			MSBuildProjectItemData mSBuildProjectItemDatum = item as MSBuildProjectItemData;
			if (mSBuildProjectItemDatum == null || mSBuildProjectItemDatum.ProjectItem.IsImported)
			{
				return false;
			}
			bool flag = this.MsBuildProject.RemoveItem(mSBuildProjectItemDatum.ProjectItem);
			if (!flag)
			{
				string path = base.DocumentReference.Path;
				string unknownError = StringTable.UnknownError;
				string removeProjectItemAction = StringTable.RemoveProjectItemAction;
				object[] itemType = new object[] { item.ItemType, item.Value };
				ProjectLog.LogError(path, unknownError, removeProjectItemAction, itemType);
			}
			else
			{
				string str = base.DocumentReference.Path;
				string removeProjectItemAction1 = StringTable.RemoveProjectItemAction;
				object[] objArray = new object[] { item.ItemType, item.Value };
				ProjectLog.LogSuccess(str, removeProjectItemAction1, objArray);
			}
			return flag;
		}

		public override void Save()
		{
			this.SetProperty("ExpressionBlendVersion", this.blendVersion, true);
			try
			{
				this.MsBuildProject.Save(base.DocumentReference.Path);
				ProjectLog.LogSuccess(base.DocumentReference.Path, StringTable.SaveAction, new object[0]);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				ProjectLog.LogError(base.DocumentReference.Path, exception, StringTable.SaveAction, new object[0]);
				throw;
			}
		}

		protected override bool SetProperty(string name, string value, bool persisted)
		{
			string str;
			string unevaluatedValue;
			if (!persisted)
			{
				return this.MsBuildProject.SetGlobalProperty(name, value);
			}
			ProjectProperty property = this.MsBuildProject.GetProperty(name);
			if (!this.IsPropertyWritable(property))
			{
				return false;
			}
			if (value == null)
			{
				if (property == null)
				{
					return true;
				}
				if (property.IsImported)
				{
					return false;
				}
				bool flag = this.MsBuildProject.RemoveProperty(property);
				if (!flag)
				{
					string path = base.DocumentReference.Path;
					string unknownError = StringTable.UnknownError;
					string removePropertyAction = StringTable.RemovePropertyAction;
					object[] objArray = new object[] { name };
					ProjectLog.LogError(path, unknownError, removePropertyAction, objArray);
				}
				else
				{
					ProjectLog.LogSuccess(base.DocumentReference.Path, StringTable.RemovePropertyAction, new object[] { name });
				}
				return flag;
			}
			if (property != null)
			{
				unevaluatedValue = property.UnevaluatedValue;
			}
			else
			{
				unevaluatedValue = null;
			}
			string str1 = unevaluatedValue;
			ProjectProperty projectProperty = this.MsBuildProject.SetProperty(name, value);
			if (!string.Equals(str1, value))
			{
				if (string.IsNullOrEmpty(str1))
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string setPropertyAction = StringTable.SetPropertyAction;
					object[] objArray1 = new object[] { name, value };
					str = string.Format(currentCulture, setPropertyAction, objArray1);
				}
				else
				{
					CultureInfo cultureInfo = CultureInfo.CurrentCulture;
					string updatePropertyAction = StringTable.UpdatePropertyAction;
					object[] objArray2 = new object[] { name, str1, value };
					str = string.Format(cultureInfo, updatePropertyAction, objArray2);
				}
				if (projectProperty == null)
				{
					ProjectLog.LogError(base.DocumentReference.Path, StringTable.UnknownError, str, new object[0]);
				}
				else
				{
					ProjectLog.LogSuccess(base.DocumentReference.Path, str, new object[0]);
				}
			}
			return projectProperty != null;
		}

		public override bool SetStoreVersion(Version version)
		{
			string toolsVersion = this.MsBuildProject.Xml.ToolsVersion;
			string str = version.ToString();
			this.MsBuildProject.Xml.ToolsVersion = str;
			if (!string.Equals(toolsVersion, str))
			{
				if (!Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(base.DocumentReference.Path))
				{
					return false;
				}
				this.Save();
				this.UnloadProject();
				this.LoadProject();
				if (string.IsNullOrEmpty(toolsVersion))
				{
					ProjectLog.LogSuccess(base.DocumentReference.Path, StringTable.SetToolVersionAction, new object[] { str });
				}
				else
				{
					string path = base.DocumentReference.Path;
					string updateToolversionAction = StringTable.UpdateToolversionAction;
					object[] objArray = new object[] { toolsVersion, str };
					ProjectLog.LogSuccess(path, updateToolversionAction, objArray);
				}
			}
			return true;
		}

		private void UnloadProject()
		{
			try
			{
				ProjectRootElement xml = this.MsBuildProject.Xml;
				Microsoft.Expression.Project.Build.BuildManager.ProjectCollection.UnloadProject(this.MsBuildProject);
				Microsoft.Expression.Project.Build.BuildManager.ProjectCollection.UnloadProject(xml);
			}
			catch (InvalidOperationException invalidOperationException)
			{
			}
		}
	}
}