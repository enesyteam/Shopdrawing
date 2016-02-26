using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DesignTimeMediaElementAttribute : Attribute
	{
		public DesignTimeMediaElementAttribute()
		{
		}
	}
}