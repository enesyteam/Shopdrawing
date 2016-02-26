using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public interface IAttachViewRoot
	{
		ViewNode ViewRoot
		{
			get;
		}

		IDisposable EnsureCrossDocumentUpdateContext();

		void EnsureRootSited();

		void UpdateLayout();
	}
}