using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IUserControlInstanceBuilder
	{
		Type PreviewControlType
		{
			get;
		}

		bool NeedsRebuild(IInstanceBuilderContext context, ViewNode viewNode, string closedDocumentPath);
	}
}