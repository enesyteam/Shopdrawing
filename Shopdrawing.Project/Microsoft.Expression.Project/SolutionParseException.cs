using System;
using System.Runtime.Serialization;

namespace Microsoft.Expression.Project
{
	[Serializable]
	public sealed class SolutionParseException : Exception
	{
		public SolutionParseException()
		{
		}

		public SolutionParseException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public SolutionParseException(string message) : base(message)
		{
		}

		private SolutionParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}