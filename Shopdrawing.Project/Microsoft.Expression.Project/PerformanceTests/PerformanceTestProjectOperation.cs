using Microsoft.Expression.Framework.Diagnostics;
using System;

namespace Microsoft.Expression.Project.PerformanceTests
{
	public abstract class PerformanceTestProjectOperation : PerformanceTestOperation
	{
		private PerformanceTestProjectUtilities performanceTestProjectUtilities;

		protected PerformanceTestProjectUtilities PerformanceTestUtilities
		{
			get
			{
				return this.performanceTestProjectUtilities;
			}
		}

		protected PerformanceTestProjectOperation(PerformanceTestProjectUtilities performanceTestProjectUtilities)
		{
			this.performanceTestProjectUtilities = performanceTestProjectUtilities;
		}
	}
}