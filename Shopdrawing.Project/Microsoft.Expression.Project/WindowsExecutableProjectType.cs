using System;

namespace Microsoft.Expression.Project
{
	public class WindowsExecutableProjectType : MSBuildBasedProjectType
	{
		public override string Identifier
		{
			get
			{
				return ProjectTypeNamesHelper.WindowsExecutable;
			}
		}

		public WindowsExecutableProjectType()
		{
		}

		public override INamedProject CreateProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IServiceProvider serviceProvider)
		{
			return WindowsExecutableProject.Create(projectStore, codeDocumentType, this, serviceProvider);
		}

		private static bool HasValidGuids(IProjectStore projectStore)
		{
			string property = projectStore.GetProperty("ProjectTypeGuids");
			if (string.IsNullOrEmpty(property))
			{
				return true;
			}
			return property.IndexOf("{349c5851-65df-11da-9384-00065b846f21}", StringComparison.OrdinalIgnoreCase) == -1;
		}

		internal static bool IsSupportedOutputType(IProjectStore projectStore)
		{
			string property = projectStore.GetProperty("OutputType");
			if (string.IsNullOrEmpty(property))
			{
				return false;
			}
			if (property.Equals("WinExe", StringComparison.OrdinalIgnoreCase) || property.Equals("Library", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (!property.Equals("StaticLibrary", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			return ProjectStoreHelper.GetProjectLanguage(projectStore) == ProjectLanguage.CPlusPlus;
		}

		public override bool IsValidTypeForProject(IProjectStore projectStore)
		{
			if (!WindowsExecutableProjectType.IsSupportedOutputType(projectStore) || !WindowsExecutableProjectType.HasValidGuids(projectStore))
			{
				return false;
			}
			return base.IsValidTypeForProject(projectStore);
		}
	}
}