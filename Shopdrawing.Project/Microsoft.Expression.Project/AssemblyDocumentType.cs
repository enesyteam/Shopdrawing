using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project
{
	public class AssemblyDocumentType : AssemblyReferenceDocumentType
	{
		public const string BuildTask = "Reference";

		protected override string DefaultBuildTask
		{
			get
			{
				return "Reference";
			}
		}

		public override string[] FileExtensions
		{
			get
			{
				return new string[] { ".dll", ".exe" };
			}
		}

		public override string Name
		{
			get
			{
				return DocumentTypeNamesHelper.Assembly;
			}
		}

		public AssemblyDocumentType()
		{
		}

		public override IProjectItem CreateProjectItem(IProject project, DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			return new AssemblyProjectItem(project, documentReference, this, serviceProvider);
		}
	}
}