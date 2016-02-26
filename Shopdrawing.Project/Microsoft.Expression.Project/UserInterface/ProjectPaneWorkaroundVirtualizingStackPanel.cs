using Microsoft.Expression.Framework.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Project.UserInterface
{
	public class ProjectPaneWorkaroundVirtualizingStackPanel : WorkaroundVirtualizingStackPanel
	{
		public ProjectPaneWorkaroundVirtualizingStackPanel()
		{
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			base.MeasureOverride(arrangeSize);
			return base.ArrangeOverride(arrangeSize);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			return base.DesiredSize;
		}
	}
}