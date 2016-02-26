using Microsoft.Expression.DesignModel.DocumentModel;
using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public sealed class DocumentNodeStructValue<T> : DocumentNodeCloneableValue
	where T : struct
	{
		private T @value;

		public DocumentNodeStructValue(T value)
		{
			this.@value = value;
		}

		public override IDocumentNodeValue Clone(IDocumentContext documentContext)
		{
			return new DocumentNodeStructValue<T>(this.@value);
		}

		public override object CloneValue()
		{
			return this.@value;
		}

		public override string ToString()
		{
			return this.@value.ToString();
		}
	}
}