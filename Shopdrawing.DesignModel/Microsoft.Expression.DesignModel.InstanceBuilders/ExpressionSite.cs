using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	internal sealed class ExpressionSite
	{
		private readonly int childIndex;

		private readonly IProperty propertyKey;

		public int ChildIndex
		{
			get
			{
				return this.childIndex;
			}
		}

		public bool IsProperty
		{
			get
			{
				return this.propertyKey != null;
			}
		}

		public IProperty PropertyKey
		{
			get
			{
				return this.propertyKey;
			}
		}

		public ExpressionSite(IProperty propertyKey)
		{
			this.propertyKey = propertyKey;
			this.childIndex = -1;
		}

		public ExpressionSite(int childIndex)
		{
			this.childIndex = childIndex;
		}
	}
}