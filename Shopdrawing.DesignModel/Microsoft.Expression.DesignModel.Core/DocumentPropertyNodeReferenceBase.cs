using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public abstract class DocumentPropertyNodeReferenceBase : DocumentNodeReference
	{
		public abstract DocumentCompositeNode Parent
		{
			get;
		}

		public abstract IPropertyId PropertyKey
		{
			get;
		}

		protected DocumentPropertyNodeReferenceBase()
		{
		}
	}
}