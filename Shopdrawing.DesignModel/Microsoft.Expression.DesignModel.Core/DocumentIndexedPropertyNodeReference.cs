using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Core
{
	public sealed class DocumentIndexedPropertyNodeReference : DocumentPropertyNodeReferenceBase
	{
		private DocumentCompositeNode parent;

		private IndexedClrPropertyReferenceStep referenceStep;

		public int Index
		{
			get
			{
				return this.referenceStep.Index;
			}
		}

		public override DocumentNode Node
		{
			get
			{
				return this.parent.Children[this.Index];
			}
			set
			{
				if (value == null)
				{
					throw new InvalidOperationException(ExceptionStringTable.DocumentNodeReferenceCannotClearIndexedNode);
				}
				this.parent.Children[this.Index] = value;
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
				return this.referenceStep;
			}
		}

		public DocumentIndexedPropertyNodeReference(DocumentCompositeNode parent, IndexedClrPropertyReferenceStep referenceStep)
		{
			this.parent = parent;
			this.referenceStep = referenceStep;
		}

		public override bool Equals(object obj)
		{
			DocumentIndexedPropertyNodeReference documentIndexedPropertyNodeReference = obj as DocumentIndexedPropertyNodeReference;
			if (documentIndexedPropertyNodeReference == null || !this.parent.Equals(documentIndexedPropertyNodeReference.parent))
			{
				return false;
			}
			return this.referenceStep.Equals(documentIndexedPropertyNodeReference.referenceStep);
		}

		public override int GetHashCode()
		{
			return this.parent.GetHashCode() ^ this.referenceStep.GetHashCode();
		}

		public override string ToString()
		{
			return string.Concat(this.parent.ToString(), "[", this.referenceStep.Name, "]");
		}
	}
}