using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal interface IEventImplementation
	{
		EventImplementationBase Implementation
		{
			get;
			set;
		}

		void Invalidate();
	}
}