using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	internal interface IExpressionCache
	{
		void CacheExpressionValue(ViewNode target, DocumentNode source);

		List<ViewNode> GetExpressionValue(DocumentNode target);

		List<ViewNode> Validate(IInstanceBuilderContext context, out List<ExpressionSite> invalidProperties);
	}
}