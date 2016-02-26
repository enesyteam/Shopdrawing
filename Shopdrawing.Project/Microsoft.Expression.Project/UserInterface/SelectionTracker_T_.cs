using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.Project.UserInterface
{
	internal class SelectionTracker<T> : LinkedList<T>
	{
		public T SelectedItem
		{
			set
			{
				if (value != null)
				{
					base.Remove(value);
					base.AddFirst(value);
				}
			}
		}

		public SelectionTracker()
		{
		}

		public T GetValidSelectedItem(ICollection<T> validItems)
		{
			if (validItems == null || validItems.Count == 0)
			{
				return default(T);
			}
			return this.GetValidSelectedItem(validItems, validItems.First<T>());
		}

		public T GetValidSelectedItem(ICollection<T> validItems, T defaultItem)
		{
			T t = this.InternalGetValidSelectedItem(validItems);
			if (t != null)
			{
				if (!t.Equals(default(T)))
				{
					return t;
				}
			}
			return defaultItem;
		}

		private T InternalGetValidSelectedItem(ICollection<T> validItems)
		{
			T t;
			if (validItems == null || validItems.Count == 0)
			{
				return default(T);
			}
			LinkedList<T>.Enumerator enumerator = base.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					if (!validItems.Contains(current))
					{
						continue;
					}
					t = current;
					return t;
				}
				return default(T);
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return t;
		}
	}
}