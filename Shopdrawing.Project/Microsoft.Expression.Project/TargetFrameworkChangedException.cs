using System;
using System.Runtime.Serialization;

namespace Microsoft.Expression.Project
{
	[Serializable]
	public sealed class TargetFrameworkChangedException : Exception
	{
		public TargetFrameworkChangedException() : base(ExceptionStringTable.TargetFrameworkChanged)
		{
		}

		public TargetFrameworkChangedException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public TargetFrameworkChangedException(string message) : base(message)
		{
		}

		private TargetFrameworkChangedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}