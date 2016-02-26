using Microsoft.Expression.DesignModel.ViewObjects;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewInlineContainer : IViewObject
	{
		ICollection<IViewInline> Inlines
		{
			get;
		}
	}
}