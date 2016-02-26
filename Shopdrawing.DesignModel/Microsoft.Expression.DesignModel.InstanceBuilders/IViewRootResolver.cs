using Microsoft.Expression.DesignModel.DocumentModel;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IViewRootResolver
	{
		IInstanceBuilderContext GetViewContext(IDocumentRoot documentRoot);
	}
}