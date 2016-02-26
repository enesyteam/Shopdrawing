using System;

namespace Microsoft.Expression.Project
{
	public static class ProjectTypeNamesHelper
	{
		public readonly static string Application;

		public readonly static string Silverlight;

		public readonly static string WebApplication;

		public readonly static string Executable;

		public readonly static string Unknown;

		public readonly static string Website;

		public readonly static string WindowsExecutable;

		static ProjectTypeNamesHelper()
		{
			ProjectTypeNamesHelper.Application = "Application Executable";
			ProjectTypeNamesHelper.Silverlight = "Silverlight-based Project";
			ProjectTypeNamesHelper.WebApplication = "Web Application Project";
			ProjectTypeNamesHelper.Executable = "Application";
			ProjectTypeNamesHelper.Unknown = "Unknown Project";
			ProjectTypeNamesHelper.Website = "Web Site";
			ProjectTypeNamesHelper.WindowsExecutable = "Windows Application";
		}
	}
}