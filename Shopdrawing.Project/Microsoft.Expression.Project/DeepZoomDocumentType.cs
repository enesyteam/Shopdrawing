using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project
{
	internal sealed class DeepZoomDocumentType : DocumentType
	{
		public override string Description
		{
			get
			{
				return null;
			}
		}

		public override string[] FileExtensions
		{
			get
			{
				return new string[] { ".xml" };
			}
		}

		protected override string FileNameBase
		{
			get
			{
				return string.Empty;
			}
		}

		protected override string ImagePath
		{
			get
			{
				return "Resources\\DeepZoom.png";
			}
		}

		public override bool IsDefaultTypeForExtension
		{
			get
			{
				return false;
			}
		}

		public override string Name
		{
			get
			{
				return DocumentTypeNamesHelper.DeepZoom;
			}
		}

		public override Microsoft.Expression.Project.PreferredExternalEditCommand PreferredExternalEditCommand
		{
			get
			{
				return Microsoft.Expression.Project.PreferredExternalEditCommand.None;
			}
		}

		public DeepZoomDocumentType()
		{
		}

		public override IProjectItem CreateProjectItem(IProject project, DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			return new DeepZoomProjectItem(project, documentReference, this, serviceProvider);
		}

		public override bool IsDocumentTypeOf(string fileName)
		{
			if (!base.IsDocumentTypeOf(fileName))
			{
				return false;
			}
			return DeepZoomHelper.IsDeepZoomDocument(fileName);
		}
	}
}