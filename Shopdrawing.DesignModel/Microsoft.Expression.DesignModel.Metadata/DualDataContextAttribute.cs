using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DualDataContextAttribute : Attribute
	{
		public bool UseDefaultDataContextWhenValueSet
		{
			get;
			private set;
		}

		public DualDataContextAttribute(bool useDefaultDataContextWhenValueSet)
		{
			this.UseDefaultDataContextWhenValueSet = useDefaultDataContextWhenValueSet;
		}
	}
}