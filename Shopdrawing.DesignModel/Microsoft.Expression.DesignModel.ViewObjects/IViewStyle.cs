using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewStyle : IViewObject
	{
		Type StyleTargetType
		{
			get;
		}
	}
}