using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public sealed class InstantiatedElementList : IEnumerable
	{
		private List<WeakReference> entries;

		public object First
		{
			get
			{
				IEnumerator enumerator = this.GetEnumerator();
				if (!enumerator.MoveNext())
				{
					return null;
				}
				return enumerator.Current;
			}
		}

		public InstantiatedElementList()
		{
			this.entries = new List<WeakReference>();
		}

		public bool Add(object instance)
		{
			if (this.entries == null || -1 != this.entries.FindIndex((WeakReference i) => {
				if (!i.IsAlive)
				{
					return false;
				}
				return i.Target == instance;
			}))
			{
				return false;
			}
			this.entries.Add(new WeakReference(instance));
			return true;
		}

		public IEnumerator GetEnumerator()
		{
			int num = 0;
			while (num < this.entries.Count)
			{
				if (!this.entries[num].IsAlive)
				{
					this.ScavengeRemaining(num);
				}
				else
				{
					List<WeakReference> weakReferences = this.entries;
					int num1 = num;
					int num2 = num1;
					num = num1 + 1;
					yield return weakReferences[num2].Target;
				}
			}
		}

		private void ScavengeRemaining(int startIndex)
		{
			int item = startIndex;
			for (int i = startIndex; i < this.entries.Count; i++)
			{
				if (this.entries[i].IsAlive)
				{
					if (item < i)
					{
						this.entries[item] = this.entries[i];
					}
					item++;
				}
			}
			this.entries.RemoveRange(item, this.entries.Count - item);
		}
	}
}