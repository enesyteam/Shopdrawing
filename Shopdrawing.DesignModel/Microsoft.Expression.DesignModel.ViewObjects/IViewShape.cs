using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewShape : IViewVisual, IViewObject
	{
		Geometry RenderedGeometry
		{
			get;
		}

		Point? StartPointOfOnePointPath
		{
			get;
		}
	}
}