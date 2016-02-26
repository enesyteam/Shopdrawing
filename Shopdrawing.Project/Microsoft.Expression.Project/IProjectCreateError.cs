using System;

namespace Microsoft.Expression.Project
{
	public interface IProjectCreateError
	{
		string Identifier
		{
			get;
		}

		string Message
		{
			get;
		}
	}
}