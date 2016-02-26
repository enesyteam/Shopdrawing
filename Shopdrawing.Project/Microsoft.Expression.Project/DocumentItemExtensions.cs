using Microsoft.Expression.Framework.Collections;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public static class DocumentItemExtensions
	{
		public static TDocumentItem FindMatchByFileName<TDocumentItem>(this IEnumerable<TDocumentItem> source, string fileName)
		where TDocumentItem : class, IDocumentItem
		{
			TDocumentItem tDocumentItem;
			if (string.IsNullOrEmpty(fileName))
			{
				return default(TDocumentItem);
			}
			using (IEnumerator<TDocumentItem> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TDocumentItem current = enumerator.Current;
					if (!fileName.Equals(Path.GetFileName(current.DocumentReference.Path), StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					tDocumentItem = current;
					return tDocumentItem;
				}
				return default(TDocumentItem);
			}
			return tDocumentItem;
		}

		public static TDocumentItem FindMatchByUrl<TDocumentItem>(this IEnumerable<TDocumentItem> source, string url)
		where TDocumentItem : class, IDocumentItem
		{
			DocumentReference documentReference = DocumentReference.Create(url);
			IIndexedHashSet<TDocumentItem> tDocumentItems = source as IIndexedHashSet<TDocumentItem>;
			if (tDocumentItems != null)
			{
				return tDocumentItems[documentReference.GetHashCode()];
			}
			return source.FirstOrDefault<TDocumentItem>((TDocumentItem d) => d.DocumentReference.GetHashCode() == documentReference.GetHashCode());
		}
	}
}