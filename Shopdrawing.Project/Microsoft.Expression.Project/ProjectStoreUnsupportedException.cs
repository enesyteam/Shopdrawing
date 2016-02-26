using System;
using System.Runtime.Serialization;

namespace Microsoft.Expression.Project
{
	[Serializable]
	public sealed class ProjectStoreUnsupportedException : Exception
	{
		public ProjectStoreUnsupportedException() : base(ExceptionStringTable.UnsupportedProjectStore)
		{
		}

		public ProjectStoreUnsupportedException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public ProjectStoreUnsupportedException(string message) : base(message)
		{
		}

		private ProjectStoreUnsupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}