using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Windows;

namespace Microsoft.Expression.Project.PerformanceTests
{
	public class ShutdownOperation : PerformanceTestProjectOperation
	{
		public ShutdownOperation(PerformanceTestProjectUtilities utilities) : base(utilities)
		{
		}

		public override void Execute()
		{
			base.PerformanceTestUtilities.CloseAll();
			Application.Current.Shutdown();
			base.FinishExecution();
		}
	}
}