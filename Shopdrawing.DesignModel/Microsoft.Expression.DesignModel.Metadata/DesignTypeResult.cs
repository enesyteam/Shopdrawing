using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class DesignTypeResult
	{
		public string Context
		{
			get;
			private set;
		}

		public Type DesignType
		{
			get;
			private set;
		}

		public bool IsFailed
		{
			get
			{
				return this.TypeGenerationException != null;
			}
		}

		public Type SourceType
		{
			get;
			private set;
		}

		public Exception TypeGenerationException
		{
			get;
			private set;
		}

		public DesignTypeResult(Type sourceType, Type designType)
		{
			this.SourceType = sourceType;
			this.DesignType = designType;
		}

		public DesignTypeResult(Type sourceType, Exception exception, string context)
		{
			this.SourceType = sourceType;
			this.TypeGenerationException = exception;
			this.Context = context;
		}

		public override string ToString()
		{
			if (this.IsFailed)
			{
				return string.Concat("Failed ", this.SourceType.FullName);
			}
			return string.Concat(this.SourceType.FullName, " -> ", this.DesignType.FullName);
		}
	}
}