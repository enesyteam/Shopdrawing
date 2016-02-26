using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Project
{
	public abstract class DocumentItemBase : IDocumentItem
	{
		private Microsoft.Expression.Framework.Documents.DocumentReference documentReference;

		public virtual IEnumerable<IDocumentItem> Descendants
		{
			get
			{
				return Enumerable.Empty<IDocumentItem>();
			}
		}

		public Microsoft.Expression.Framework.Documents.DocumentReference DocumentReference
		{
			get
			{
				return this.documentReference;
			}
		}

		public virtual bool IsDirectory
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsReference
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsVirtual
		{
			get
			{
				return false;
			}
		}

		protected virtual bool SafeToRename
		{
			get
			{
				return false;
			}
		}

		public virtual Version VersionNumber
		{
			get
			{
				return new Version(0, 0);
			}
		}

		protected DocumentItemBase(Microsoft.Expression.Framework.Documents.DocumentReference documentReference)
		{
			if (documentReference == null)
			{
				throw new ArgumentNullException("documentReference");
			}
			this.documentReference = documentReference;
		}

		public void Rename(Microsoft.Expression.Framework.Documents.DocumentReference newDocumentReference)
		{
			if (!this.SafeToRename)
			{
				throw new InvalidOperationException("Rename not supported on this type of document item.");
			}
			this.documentReference = newDocumentReference;
		}
	}
}