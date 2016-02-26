using Microsoft.Build.Exceptions;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal class MigratingMSBuildStore : IProjectStore, IDisposable
	{
		private IServiceProvider serviceProvider;

		private Func<bool, bool> AttemptToMigrate;

		public Microsoft.Expression.Framework.Documents.DocumentReference DocumentReference
		{
			get
			{
				return JustDecompileGenerated_get_DocumentReference();
			}
			set
			{
				JustDecompileGenerated_set_DocumentReference(value);
			}
		}

		private Microsoft.Expression.Framework.Documents.DocumentReference JustDecompileGenerated_DocumentReference_k__BackingField;

		public Microsoft.Expression.Framework.Documents.DocumentReference JustDecompileGenerated_get_DocumentReference()
		{
			return this.JustDecompileGenerated_DocumentReference_k__BackingField;
		}

		private void JustDecompileGenerated_set_DocumentReference(Microsoft.Expression.Framework.Documents.DocumentReference value)
		{
			this.JustDecompileGenerated_DocumentReference_k__BackingField = value;
		}

		internal Exception LastError
		{
			get;
			private set;
		}

		internal IProjectStore NestedStore
		{
			get;
			private set;
		}

		public IEnumerable<string> ProjectImports
		{
			get
			{
				return this.NestedStore.ProjectImports;
			}
		}

		public Version StoreVersion
		{
			get
			{
				return this.NestedStore.StoreVersion;
			}
		}

		private MigratingMSBuildStore(Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			this.DocumentReference = documentReference;
			this.serviceProvider = serviceProvider;
			this.AttemptToMigrate = (bool callerSuccess) => {
				if (!callerSuccess)
				{
					return callerSuccess;
				}
				try
				{
					ProjectPathHelper.AttemptToMakeWritable(this.DocumentReference, this.serviceProvider);
					this.Save();
					IProjectStore projectStore = MSBuildBasedProjectStore.CreateInstance(this.DocumentReference, this.serviceProvider);
					this.NestedStore.Dispose();
					this.NestedStore = projectStore;
					this.AttemptToMigrate = (bool value) => value;
				}
				catch (InvalidProjectFileException invalidProjectFileException)
				{
					this.LastError = invalidProjectFileException;
				}
				return callerSuccess;
			};
		}

		public bool AddImport(string value)
		{
			return this.AttemptToMigrate(this.NestedStore.AddImport(value));
		}

		public IProjectItemData AddItem(string itemType, string itemValue)
		{
			return this.NestedStore.AddItem(itemType, itemValue);
		}

		public bool ChangeImport(string oldImportValue, string newImportValue)
		{
			return this.AttemptToMigrate(this.NestedStore.ChangeImport(oldImportValue, newImportValue));
		}

		public static IProjectStore CreateInstance(Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			IProjectStore projectStore;
			try
			{
				projectStore = MSBuildBasedProjectStore.CreateInstance(documentReference, serviceProvider);
			}
			catch (InvalidProjectFileException invalidProjectFileException1)
			{
				InvalidProjectFileException invalidProjectFileException = invalidProjectFileException1;
				MigratingMSBuildStore migratingMSBuildStore = new MigratingMSBuildStore(documentReference, serviceProvider)
				{
					LastError = invalidProjectFileException,
					NestedStore = XmlMSBuildProjectStore.CreateInstance(documentReference, serviceProvider)
				};
				projectStore = migratingMSBuildStore;
			}
			return projectStore;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.NestedStore != null)
				{
					this.NestedStore.Dispose();
				}
				this.NestedStore = null;
			}
		}

		public IEnumerable<IProjectItemData> GetItems(string itemType)
		{
			return this.NestedStore.GetItems(itemType);
		}

		public string GetProperty(string name)
		{
			return this.NestedStore.GetProperty(name);
		}

		public bool IsPropertyWritable(string name)
		{
			return this.NestedStore.IsPropertyWritable(name);
		}

		public bool RemoveItem(IProjectItemData item)
		{
			return this.NestedStore.RemoveItem(item);
		}

		public void Save()
		{
			this.NestedStore.Save();
		}

		public bool SetProperty(string name, string value)
		{
			return this.AttemptToMigrate(this.NestedStore.SetProperty(name, value));
		}

		public bool SetStoreVersion(Version version)
		{
			return this.AttemptToMigrate(this.NestedStore.SetStoreVersion(version));
		}

		public bool SetUnpersistedProperty(string name, string value)
		{
			return this.AttemptToMigrate(this.NestedStore.SetUnpersistedProperty(name, value));
		}
	}
}