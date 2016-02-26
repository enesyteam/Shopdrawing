using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.SourceControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.Expression.Project
{
	public static class SourceControlStatusCache
	{
		private static Dictionary<DocumentReference, SourceControlStatus> statusCache;

		static SourceControlStatusCache()
		{
			SourceControlStatusCache.statusCache = new Dictionary<DocumentReference, SourceControlStatus>(79);
		}

		internal static void ClearStatusCache()
		{
			SourceControlStatusCache.statusCache.Clear();
			SourceControlStatusCache.OnStatusUpdated();
		}

		public static SourceControlStatus GetCachedStatus(IDocumentItem item)
		{
			return SourceControlStatusCache.GetCachedStatus(item.DocumentReference);
		}

		public static SourceControlStatus GetCachedStatus(DocumentReference item)
		{
			SourceControlStatus sourceControlStatu;
			if (item == null)
			{
				return SourceControlStatus.None;
			}
			SourceControlStatusCache.statusCache.TryGetValue(item, out sourceControlStatu);
			return sourceControlStatu;
		}

		private static void OnStatusUpdated()
		{
			if (SourceControlStatusCache.StatusUpdated != null)
			{
				SourceControlStatusCache.StatusUpdated(null, new EventArgs());
			}
		}

		public static void SetCachedStatus(IDocumentItem item, SourceControlStatus status)
		{
			SourceControlStatusCache.SetCachedStatusInternal(item.DocumentReference, status);
			SourceControlStatusCache.OnStatusUpdated();
		}

		public static void SetCachedStatus(IEnumerable<IDocumentItem> items, SourceControlStatus status)
		{
			foreach (IDocumentItem item in items)
			{
				SourceControlStatusCache.SetCachedStatusInternal(item.DocumentReference, status);
			}
			SourceControlStatusCache.OnStatusUpdated();
		}

		public static void SetCachedStatus(DocumentReference item, SourceControlStatus status)
		{
			SourceControlStatusCache.SetCachedStatusInternal(item, status);
			SourceControlStatusCache.OnStatusUpdated();
		}

		public static void SetCachedStatus(IEnumerable<DocumentReference> items, SourceControlStatus status)
		{
			foreach (DocumentReference item in items)
			{
				SourceControlStatusCache.SetCachedStatusInternal(item, status);
			}
			SourceControlStatusCache.OnStatusUpdated();
		}

		private static void SetCachedStatusInternal(DocumentReference item, SourceControlStatus status)
		{
			if (SourceControlStatusCache.statusCache.ContainsKey(item))
			{
				SourceControlStatusCache.statusCache[item] = status;
				return;
			}
			SourceControlStatusCache.statusCache.Add(item, status);
		}

		internal static void UpdateStatus(IEnumerable<IDocumentItem> items, ISourceControlProvider sourceControlProvider)
		{
			if (sourceControlProvider == null)
			{
				return;
			}
			SourceControlStatusCache.UpdateStatus(
				from item in items
				where !item.IsReference
				select item.DocumentReference, sourceControlProvider);
		}

		internal static void UpdateStatus(IEnumerable<DocumentReference> items, ISourceControlProvider sourceControlProvider)
		{
			if (items.CountIs<DocumentReference>(0) || sourceControlProvider == null)
			{
				return;
			}
			DocumentReference[] array = (
				from item in items
				where PathHelper.FileExists(item.Path)
				select item).ToArray<DocumentReference>();
			SourceControlStatus[] sourceControlStatusArray = new SourceControlStatus[(int)array.Length];
			sourceControlProvider.QueryInfo((
				from path in (IEnumerable<DocumentReference>)array
				select path.Path).ToArray<string>(), sourceControlStatusArray);
			for (int i = 0; i < (int)array.Length; i++)
			{
				SourceControlStatusCache.SetCachedStatusInternal(array[i], sourceControlStatusArray[i]);
			}
			SourceControlStatusCache.OnStatusUpdated();
		}

		public static event EventHandler StatusUpdated;
	}
}