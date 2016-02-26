using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewResourceDictionary : IViewObject
	{
		void Clear();

		object FindResource(object key);
	}
}