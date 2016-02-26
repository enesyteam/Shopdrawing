using Microsoft.Expression.Framework.Commands;
using System;

namespace Microsoft.Expression.Project.Commands
{
	public interface IProjectCommand : ICommand
	{
		string DisplayName
		{
			get;
		}

		IServiceProvider Services
		{
			get;
		}
	}
}