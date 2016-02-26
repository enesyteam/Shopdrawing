using Microsoft.Build.Exceptions;
using System;
using System.IO;
using System.Security;
using System.Xml;

namespace Microsoft.Expression.Project
{
	public static class ErrorHandling
	{
		public static bool HandleBasicExceptions(Action action, Action<Exception> handledExceptionAction)
		{
			bool flag;
			try
			{
				action();
				flag = true;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (!ErrorHandling.ShouldHandleExceptions(exception))
				{
					throw;
				}
				else
				{
					if (handledExceptionAction != null)
					{
						handledExceptionAction(exception);
					}
					flag = false;
				}
			}
			return flag;
		}

		public static bool ShouldHandleExceptions(Exception e)
		{
			if (e.GetType() == typeof(ArgumentException))
			{
				if (!e.StackTrace.Contains("System.IO.Path.CheckInvalidPathChars") && !e.StackTrace.Contains("System.IO.Path.NormalizePath"))
				{
					return false;
				}
				return true;
			}
			if (!(e is IOException) && !(e is SecurityException) && !(e is UnauthorizedAccessException) && !(e is InvalidProjectFileException) && !(e is InvalidToolsetDefinitionException) && !(e.GetType().FullName == "Microsoft.Build.BuildEngine.InvalidProjectFileException") && !(e.GetType().FullName == "Microsoft.Build.BuildEngine.Shared.InternalErrorException") && !(e is XmlException) && !(e is TargetFrameworkChangedException) && !(e is ProjectStoreUnsupportedException))
			{
				return false;
			}
			return true;
		}
	}
}