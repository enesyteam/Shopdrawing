using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewControl : IViewVisual, IViewObject
	{
		object VisualStateManagerHost
		{
			get;
		}
	}
}