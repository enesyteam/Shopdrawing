using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal abstract class ProjectStoreBase : IProjectStore, IDisposable
	{
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

		protected bool IsDisposed
		{
			get;
			private set;
		}

		public virtual IEnumerable<string> ProjectImports
		{
			get
			{
				return Enumerable.Empty<string>();
			}
		}

		public virtual Version StoreVersion
		{
			get
			{
				return new Version();
			}
		}

		protected ProjectStoreBase(Microsoft.Expression.Framework.Documents.DocumentReference documentReference)
		{
			if (documentReference == null)
			{
				throw new ArgumentNullException("documentReference");
			}
			this.DocumentReference = documentReference;
		}

		public virtual bool AddImport(string value)
		{
			return false;
		}

		public virtual IProjectItemData AddItem(string itemType, string itemValue)
		{
			return null;
		}

		public virtual bool ChangeImport(string oldImportValue, string newImportValue)
		{
			return false;
		}

		public void Dispose()
		{
			try
			{
				if (!this.IsDisposed)
				{
					this.Dispose(true);
				}
			}
			finally
			{
				this.IsDisposed = true;
			}
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public virtual IEnumerable<IProjectItemData> GetItems(string itemType)
		{
			return Enumerable.Empty<IProjectItemData>();
		}

		public virtual string GetProperty(string name)
		{
			return null;
		}

		public virtual bool IsPropertyWritable(string name)
		{
			return false;
		}

		public virtual bool RemoveItem(IProjectItemData item)
		{
			return false;
		}

		public virtual void Save()
		{
		}

		public bool SetProperty(string name, string value)
		{
			return this.SetProperty(name, value, true);
		}

		protected virtual bool SetProperty(string name, string value, bool persisted)
		{
			return false;
		}

		public virtual bool SetStoreVersion(Version version)
		{
			return false;
		}

		public bool SetUnpersistedProperty(string name, string value)
		{
			return this.SetProperty(name, value, false);
		}
	}
}