using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public abstract class DocumentNodeReference
	{
		public abstract DocumentNode Node
		{
			get;
			set;
		}

		protected DocumentNodeReference()
		{
		}
	}
}