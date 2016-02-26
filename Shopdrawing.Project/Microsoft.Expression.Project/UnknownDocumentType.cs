using System;

namespace Microsoft.Expression.Project
{
	internal class UnknownDocumentType : DocumentType
	{
		public override string Description
		{
			get
			{
				return StringTable.UnknownDocumentTypeDescription;
			}
		}

		public override string[] FileExtensions
		{
			get
			{
				return new string[] { ".*" };
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
				return "Resources\\Unknown.png";
			}
		}

		public override string Name
		{
			get
			{
				return "Document";
			}
		}

		public UnknownDocumentType()
		{
		}
	}
}