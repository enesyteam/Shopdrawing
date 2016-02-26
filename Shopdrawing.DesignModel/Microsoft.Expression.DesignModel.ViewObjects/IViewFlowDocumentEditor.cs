using Microsoft.Expression.DesignModel.ViewObjects.TextObjects;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewFlowDocumentEditor : IViewRichTextBox, IViewTextBoxBase, IViewControl, IViewVisual, IViewObject
	{
		Thickness BorderThickness
		{
			get;
			set;
		}

		IViewFlowDocument Document
		{
			get;
			set;
		}

		event RoutedEventHandler Loaded;
	}
}