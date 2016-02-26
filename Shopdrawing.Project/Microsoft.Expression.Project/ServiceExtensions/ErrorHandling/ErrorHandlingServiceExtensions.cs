using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.ServiceExtensions.ErrorHandling
{
	internal static class ErrorHandlingServiceExtensions
	{
		public static bool ExceptionHandler(this IServiceProvider source, Action action, Func<string> errorMessage)
		{
			return Microsoft.Expression.Project.ErrorHandling.HandleBasicExceptions(action, (Exception exception) => source.ShowErrorMessage(errorMessage(), exception));
		}

		public static void ShowErrorMessage(this IServiceProvider source, string message, Exception exception)
		{
			ErrorArgs errorArg = new ErrorArgs()
			{
				Message = message,
				Exception = exception
			};
			source.MessageDisplayService().ShowError(errorArg);
		}
	}
}