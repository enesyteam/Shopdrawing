using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IEasingFunctionDefinition : IViewObject
	{
		Microsoft.Expression.DesignModel.ViewObjects.EasingMode EasingMode
		{
			get;
			set;
		}

		string GroupName
		{
			get;
		}

		string Name
		{
			get;
		}

		object PlatformSpecificEasingMode
		{
			get;
		}

		double Ease(double normalizedTime);
	}
}