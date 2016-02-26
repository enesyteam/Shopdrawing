using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface ICrossDocumentUpdateContext
	{
		bool IsDelaying
		{
			get;
		}

		void BeginUpdate(bool shouldDelay);

		void DelayInstanceBuilding(IInstanceBuilderContext context, ViewNode viewNode);

		void EndUpdate();

		IInstanceBuilderContext GetViewContext(IDocumentRoot documentRoot);

		void OnViewNodeRemoving(IInstanceBuilderContext context, ViewNode viewNode);
	}
}