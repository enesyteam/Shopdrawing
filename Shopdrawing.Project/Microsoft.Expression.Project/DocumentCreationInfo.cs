using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public struct DocumentCreationInfo
	{
		public Microsoft.Expression.Project.BuildTaskInfo BuildTaskInfo
		{
			get;
			set;
		}

		public Microsoft.Expression.Project.CreationOptions CreationOptions
		{
			get;
			set;
		}

		public IDocumentType DocumentType
		{
			get;
			set;
		}

		public string SourcePath
		{
			get;
			set;
		}

		public string TargetFolder
		{
			get;
			set;
		}

		public string TargetPath
		{
			get;
			set;
		}
	}
}