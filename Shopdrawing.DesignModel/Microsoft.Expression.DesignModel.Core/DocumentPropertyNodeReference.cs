using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public sealed class DocumentPropertyNodeReference : DocumentPropertyNodeReferenceBase
	{
		private DocumentCompositeNode parent;

		private IPropertyId propertyKey;

		public override DocumentNode Node
		{
			get
			{
				return this.parent.Properties[this.propertyKey];
			}
			set
			{
				this.parent.Properties[this.propertyKey] = value;
			}
		}

		public override DocumentCompositeNode Parent
		{
			get
			{
				return this.parent;
			}
		}

		public override IPropertyId PropertyKey
		{
			get
			{
				return this.propertyKey;
			}
		}

		public DocumentPropertyNodeReference(DocumentCompositeNode parent, IPropertyId propertyKey)
		{
			this.parent = parent;
			this.propertyKey = propertyKey;
		}

		public override bool Equals(object obj)
		{
			DocumentPropertyNodeReference documentPropertyNodeReference = obj as DocumentPropertyNodeReference;
			if (documentPropertyNodeReference == null || !this.parent.Equals(documentPropertyNodeReference.parent))
			{
				return false;
			}
			return this.propertyKey.Equals(documentPropertyNodeReference.propertyKey);
		}

		public override int GetHashCode()
		{
			return this.parent.GetHashCode() ^ this.propertyKey.GetHashCode();
		}

		public override string ToString()
		{
			return string.Concat(this.parent.ToString(), ".", this.propertyKey.Name);
		}
	}
}