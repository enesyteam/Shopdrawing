using System;

namespace Microsoft.Expression.Project
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class VisualStudioSolutionAttribute : Attribute
	{
		public VisualStudioSolutionAttribute()
		{
		}
	}
}