using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project
{
	public class ProjectReferenceDocumentType : AssemblyReferenceDocumentType
	{
		public const string BuildTask = "ProjectReference";

		protected override string DefaultBuildTask
		{
			get
			{
				return "ProjectReference";
			}
		}

		public override string[] FileExtensions
		{
			get
			{
				return new string[] { ".csproj", ".vbproj", ".vcproj", ".vjsproj" };
			}
		}

		public override string Name
		{
			get
			{
				return DocumentTypeNamesHelper.ProjectReference;
			}
		}

		public ProjectReferenceDocumentType()
		{
		}

		public override IProjectItem CreateProjectItem(IProject project, DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			return new ProjectReferenceProjectItem(project, documentReference, this, serviceProvider);
		}
	}
}