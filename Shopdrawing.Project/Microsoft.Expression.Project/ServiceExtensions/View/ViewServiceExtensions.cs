using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.ServiceExtensions.View
{
	public static class ViewServiceExtensions
	{
		public static IView ActiveView(this IServiceProvider source)
		{
			IViewService viewService = source.ViewService();
			if (viewService == null)
			{
				return null;
			}
			return viewService.ActiveView;
		}

		public static void CloseAllViews(this IServiceProvider source, bool closeActiveDocument)
		{
			IViewService viewService = source.ViewService();
			if (viewService == null)
			{
				return;
			}
			for (int i = viewService.Views.Count - 1; i >= 0; i--)
			{
				bool item = true;
				if (!closeActiveDocument)
				{
					item = viewService.Views[i] != viewService.ActiveView;
				}
				if (i < viewService.Views.Count && item)
				{
					viewService.CloseView(viewService.Views[i]);
				}
			}
		}

		public static IViewCollection Views(this IServiceProvider source)
		{
			IViewService viewService = source.ViewService();
			if (viewService == null)
			{
				return null;
			}
			return viewService.Views;
		}
	}
}