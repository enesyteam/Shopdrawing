using System;
using System.Diagnostics;

namespace Microsoft.Expression.Project.Build
{
	public interface IExecutable
	{
		bool CanExecute
		{
			get;
		}

		bool IsExecuting
		{
			get;
		}

		System.Diagnostics.ProcessStartInfo ProcessStartInfo
		{
			get;
		}

		string StartArguments
		{
			get;
		}

		string StartProgram
		{
			get;
		}

		string WorkingDirectory
		{
			get;
		}

		bool Execute();
	}
}