using System;

namespace Microsoft.Expression.Project
{
	internal sealed class CursorDocumentType : DocumentType
	{
		private const string BuildTask = "Resource";

		protected override string DefaultBuildTask
		{
			get
			{
				return "Resource";
			}
		}

		public override string Description
		{
			get
			{
				return StringTable.CursorDocumentTypeDescription;
			}
		}

		public override string[] FileExtensions
		{
			get
			{
				return new string[] { ".cur" };
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
				return "Resources\\Cursor.png";
			}
		}

		public override string Name
		{
			get
			{
				return "Cursor";
			}
		}

		public CursorDocumentType()
		{
		}
	}
}