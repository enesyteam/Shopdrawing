using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Project.Build
{
	public interface IBuildWorker
	{
		BuildResult Build(IEnumerable<ILogger> loggers, params string[] targetNames);
	}
}