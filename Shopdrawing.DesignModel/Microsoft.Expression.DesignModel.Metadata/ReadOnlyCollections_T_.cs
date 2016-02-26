using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public static class ReadOnlyCollections<T>
	{
		public readonly static ReadOnlyCollection<T> Empty;

		static ReadOnlyCollections()
		{
			ReadOnlyCollections<T>.Empty = new ReadOnlyCollection<T>(new List<T>());
		}
	}
}