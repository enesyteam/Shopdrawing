using System;

namespace Microsoft.Expression.Project
{
	public abstract class AssemblyReferenceDocumentType : DocumentType
	{
		public override string Description
		{
			get
			{
				return StringTable.AssemblyDocumentTypeDescription;
			}
		}

		protected override string FileNameBase
		{
			get
			{
				return StringTable.AssemblyDocumentTypeFileNameBase;
			}
		}

		protected override string ImagePath
		{
			get
			{
				return "Resources\\Assembly.png";
			}
		}

		public override Microsoft.Expression.Project.PreferredExternalEditCommand PreferredExternalEditCommand
		{
			get
			{
				return Microsoft.Expression.Project.PreferredExternalEditCommand.None;
			}
		}

		protected AssemblyReferenceDocumentType()
		{
		}

		public override bool CanAddToProject(IProject project)
		{
			return false;
		}

		public override bool IsDocumentTypeOf(string fileName)
		{
			return false;
		}
	}
}