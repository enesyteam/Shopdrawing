using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public static class KnownProjectExtensions
	{
		public static IProject FindMatchByGuid(this IEnumerable<IProject> source, Guid projectGuid)
		{
			return source.FirstOrDefault<IProject>((IProject p) => p.ProjectGuid == projectGuid);
		}
	}
}