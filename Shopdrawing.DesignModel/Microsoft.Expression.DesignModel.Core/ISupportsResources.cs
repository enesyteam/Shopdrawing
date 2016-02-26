using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public interface ISupportsResources
	{
		DocumentCompositeNode HostNode
		{
			get;
		}

		DocumentCompositeNode Resources
		{
			get;
			set;
		}
	}
}