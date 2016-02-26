using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DataContextPathExtensionAttribute : Attribute
	{
		public bool IsCollectionItem
		{
			get;
			private set;
		}

		public string Property
		{
			get;
			private set;
		}

		public DataContextPathExtensionAttribute(string property, bool isCollectionItem)
		{
			this.Property = property;
			this.IsCollectionItem = isCollectionItem;
		}
	}
}