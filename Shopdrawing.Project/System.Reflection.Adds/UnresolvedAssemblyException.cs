using System;
using System.Runtime.Serialization;

namespace System.Reflection.Adds
{
	[Serializable]
	internal class UnresolvedAssemblyException : Exception
	{
		public UnresolvedAssemblyException(string message) : base(message)
		{
		}

		public UnresolvedAssemblyException()
		{
		}

		public UnresolvedAssemblyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UnresolvedAssemblyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}