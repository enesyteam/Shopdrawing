using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.Core
{
	public interface IPlatformSpecificView
	{
		string BaseUri
		{
			get;
			set;
		}

		ImageHost CreateImageHost();

		void Dispose();

		Rect GetActualBounds(IViewObject element);

		Rect GetActualBoundsInParent(IViewObject element);

		double GetDefinitionActualSize(object obj);

		Rect GetDescendantBounds(IViewObject element);

		Geometry GetShapeRenderedGeometry(object shape, Rect bounds);

		bool IsAncestorOf(IViewObject potentialAncestor, IViewObject target);

		bool IsMatrixTransform(IViewObject from, IViewObject to);

		void SetName(object target, string name);

		Rect TransformBounds(IViewObject from, IViewObject to, Rect bounds);

		Point TransformPoint(IViewObject from, IViewObject to, Point point);

		Matrix TransformToVisual(IViewObject from, IViewObject to);

		Matrix TransformToVisualVerified(IViewObject from, IViewObject to);

		event UnhandledExceptionEventHandler UnhandledException;
	}
}