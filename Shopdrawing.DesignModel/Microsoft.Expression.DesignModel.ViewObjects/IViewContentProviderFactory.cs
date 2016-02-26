using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewContentProviderFactory
	{
		ViewContentProvider GetProvider(object instance);

		void Register(ViewContentProvider value);

		void Unregister(ViewContentProvider value);
	}
}