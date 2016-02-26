using Microsoft.Expression.DesignModel.DocumentModel;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	[Serializable]
	public sealed class InstanceBuilderException : Exception
	{
		private DocumentNode exceptionSource;

		private ViewNode exceptionTarget;

		public DocumentNode ExceptionSource
		{
			get
			{
				return this.exceptionSource;
			}
		}

		public ViewNode ExceptionTarget
		{
			get
			{
				return this.exceptionTarget;
			}
		}

		private InstanceBuilderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public InstanceBuilderException() : this(null, null, null)
		{
		}

		public InstanceBuilderException(string message) : this(message, null, null)
		{
		}

		public InstanceBuilderException(string message, Exception innerException) : this(message, innerException, null)
		{
		}

		public InstanceBuilderException(string message, DocumentNode exceptionSource) : this(message, null, exceptionSource)
		{
		}

		public InstanceBuilderException(string message, Exception innerException, DocumentNode exceptionSource) : this(message, innerException, exceptionSource, null)
		{
		}

		public InstanceBuilderException(string message, Exception innerException, DocumentNode exceptionSource, ViewNode exceptionTarget) : base(message, innerException)
		{
			this.exceptionSource = exceptionSource;
			this.exceptionTarget = exceptionTarget;
		}
	}
}