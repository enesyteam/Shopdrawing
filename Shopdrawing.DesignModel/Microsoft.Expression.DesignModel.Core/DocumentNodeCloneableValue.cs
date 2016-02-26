using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public abstract class DocumentNodeCloneableValue : IDocumentNodeValue
	{
		protected DocumentNodeCloneableValue()
		{
		}

		public abstract IDocumentNodeValue Clone(IDocumentContext documentContext);

		public abstract object CloneValue();
	}
}