using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project
{
	public class ComReferenceDocumentType : AssemblyReferenceDocumentType
	{
		public const string BuildTask = "COMReference";

		protected override string DefaultBuildTask
		{
			get
			{
				return "COMReference";
			}
		}

		public override string[] FileExtensions
		{
			get
			{
				return new string[0];
			}
		}

		public override string Name
		{
			get
			{
				return DocumentTypeNamesHelper.ComReference;
			}
		}

		public ComReferenceDocumentType()
		{
		}

		public override IProjectItem CreateProjectItem(IProject project, DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			return new ComReferenceProjectItem(project, documentReference, this, serviceProvider);
		}
	}
}