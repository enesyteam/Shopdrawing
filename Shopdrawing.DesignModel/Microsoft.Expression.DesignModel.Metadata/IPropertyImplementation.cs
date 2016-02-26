using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal interface IPropertyImplementation
	{
		PropertyImplementationBase Implementation
		{
			get;
			set;
		}

		void Invalidate();
	}
}