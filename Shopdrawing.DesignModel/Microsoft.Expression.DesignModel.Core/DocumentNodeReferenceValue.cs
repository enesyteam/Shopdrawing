using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public sealed class DocumentNodeReferenceValue : IDocumentNodeValue
	{
		private DocumentNode @value;

		public DocumentNode Value
		{
			get
			{
				return this.@value;
			}
		}

		public DocumentNodeReferenceValue(DocumentNode value)
		{
			this.@value = value;
		}

		public DocumentNodeReferenceValue Clone(IDocumentContext documentContext)
		{
			return new DocumentNodeReferenceValue(this.@value);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			DocumentNodeReferenceValue documentNodeReferenceValue = obj as DocumentNodeReferenceValue;
			if (documentNodeReferenceValue == null)
			{
				return false;
			}
			return documentNodeReferenceValue.@value == this.@value;
		}

		public override int GetHashCode()
		{
			return this.@value.GetHashCode();
		}

		IDocumentNodeValue Microsoft.Expression.DesignModel.DocumentModel.IDocumentNodeValue.Clone(IDocumentContext documentContext)
		{
			return this.Clone(documentContext);
		}

		public override string ToString()
		{
			return this.@value.ToString();
		}
	}
}