using System;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	public static class ProjectAdapterHelper
	{
		public static bool IsTargetFrameworkSupported(IServiceProvider serviceProvider, FrameworkName targetFramework)
		{
			if (targetFramework == null)
			{
				return false;
			}
			IProjectAdapterService service = (IProjectAdapterService)serviceProvider.GetService(typeof(IProjectAdapterService));
			if (service == null)
			{
				return false;
			}
			return service.IsTargetFrameworkSupported(targetFramework);
		}
	}
}