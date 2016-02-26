using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Project
{
	public sealed class DocumentItemUrlComparer<TDocumentItem> : IEqualityComparer<TDocumentItem>
	where TDocumentItem : class, IDocumentItem
	{
		public DocumentItemUrlComparer()
		{
		}

		public bool Equals(TDocumentItem x, TDocumentItem y)
		{
			if (object.ReferenceEquals(x, y))
			{
				return true;
			}
			if (x == null || y == null || !(x.DocumentReference != null) || !(y.DocumentReference != null))
			{
				return false;
			}
			return x.DocumentReference.GetHashCode() == y.DocumentReference.GetHashCode();
		}

		public int GetHashCode(TDocumentItem obj)
		{
			if (obj == null || obj.DocumentReference == null)
			{
				throw new ArgumentNullException("obj");
			}
			return obj.DocumentReference.GetHashCode();
		}
	}
}