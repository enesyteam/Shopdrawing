using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewGrid : IViewPanel, IViewVisual, IViewObject
	{
		int ColumnDefinitionsCount
		{
			get;
		}

		int RowDefinitionsCount
		{
			get;
		}

		IViewColumnDefinition GetColumnDefinition(int index);

		IViewRowDefinition GetRowDefinition(int index);
	}
}