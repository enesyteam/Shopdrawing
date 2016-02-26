using System;

namespace Microsoft.Expression.Project
{
	[Flags]
	public enum UpdateSourceControlActions
	{
		PendAdd = 1,
		Checkout = 2
	}
}