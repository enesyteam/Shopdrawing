using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Utility.Extensions.Enumerable
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<TSource> AppendItem<TSource>(this IEnumerable<TSource> source, TSource item)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			return EnumerableExtensions.AppendItemIterator<TSource>(source, item);
		}

		private static IEnumerable<TSource> AppendItemIterator<TSource>(IEnumerable<TSource> source, TSource item)
		{
			foreach (TSource tSource in source)
			{
				yield return tSource;
			}
			yield return item;
		}

		public static IEnumerable<TSource> AppendItems<TSource>(this IEnumerable<TSource> source, params TSource[] items)
		{
			return source.Concat<TSource>(items);
		}

		public static bool AreContinuous<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> original)
		{
			bool flag;
			int num = -1;
			List<TSource> list = original.ToList<TSource>();
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int num1 = list.IndexOf(enumerator.Current);
					if (num <= -1 || num1 == num + 1)
					{
						num = num1;
					}
					else
					{
						flag = false;
						return flag;
					}
				}
				return true;
			}
			return flag;
		}

		public static IEnumerable<TSource> AsEnumerable<TSource>(TSource item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			return EnumerableExtensions.SingleItemIterator<TSource>(item);
		}

		private static object[] ConvertToArray(IEnumerable source)
		{
			ArrayList arrayLists = source as ArrayList;
			if (arrayLists == null)
			{
				arrayLists = new ArrayList();
				foreach (object obj in source)
				{
					arrayLists.Add(obj);
				}
			}
			return arrayLists.ToArray();
		}

		public static bool CountIs<TSource>(this IEnumerable<TSource> source, int count)
		{
			bool flag;
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			ICollection<TSource> tSources = source as ICollection<TSource>;
			if (tSources != null)
			{
				return tSources.Count == count;
			}
			int num = 0;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					num++;
					if (num <= count)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return num == count;
			}
			return flag;
		}

		public static bool CountIsLessThan<TSource>(this IEnumerable<TSource> source, int count)
		{
			bool flag;
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (count < 1)
			{
				return false;
			}
			ICollection<TSource> tSources = source as ICollection<TSource>;
			if (tSources != null)
			{
				return tSources.Count < count;
			}
			int num = 0;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					num++;
					if (num <= count)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return num < count;
			}
			return flag;
		}

		public static bool CountIsMoreThan<TSource>(this IEnumerable<TSource> source, int count)
		{
			bool flag;
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			ICollection<TSource> tSources = source as ICollection<TSource>;
			if (tSources != null)
			{
				return tSources.Count > count;
			}
			int num = 0;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					num++;
					if (num <= count)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return num > count;
			}
			return flag;
		}

		public static IEnumerable<TSource> DistinctOnOrdered<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer)
		{
			TSource tSource = default(TSource);
			bool flag = true;
			foreach (TSource tSource1 in source)
			{
				if (flag)
				{
					yield return tSource1;
					flag = false;
				}
				else if (comparer.Compare(tSource, tSource1) != 0)
				{
					yield return tSource1;
				}
				tSource = tSource1;
			}
		}

		public static void EnqueueMany<TBase, TDerived>(this Queue<TBase> queue, IEnumerable<TDerived> items)
		where TDerived : TBase
		{
			foreach (TDerived item in items)
			{
				queue.Enqueue((TBase)(object)item);
			}
		}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> items, T itemToExclude)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			return items.Except<T>(new T[] { itemToExclude });
		}

		public static int FindIndex<T>(this IEnumerable<T> items, Predicate<T> criterion)
		{
			int num;
			int num1 = 0;
			using (IEnumerator<T> enumerator = items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!criterion(enumerator.Current))
					{
						num1++;
					}
					else
					{
						num = num1;
						return num;
					}
				}
				return -1;
			}
			return num;
		}

		public static int FindMaxValueIndex<T>(this IEnumerable<T> sequence)
		where T : IComparable<T>
		{
			int num = -1;
			T t = default(T);
			int num1 = 0;
			foreach (T t1 in sequence)
			{
				if (t1.CompareTo(t) > 0 || num == -1)
				{
					num = num1;
					t = t1;
				}
				num1++;
			}
			return num;
		}

		public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (T item in items)
			{
				action(item);
			}
		}

		public static void ForEach(this IEnumerable items, Action<object> action)
		{
			foreach (object item in items)
			{
				action(item);
			}
		}

		public static void ForEach<T>(this IEnumerable<T> items, Action<T, int> action)
		{
			int num = 0;
			foreach (T item in items)
			{
				action(item, num);
				num++;
			}
		}

		public static void Merge<T>(this ICollection<T> oldItems, IEnumerable<T> newItems)
		{
			if (oldItems == null)
			{
				throw new ArgumentNullException("oldItems");
			}
			if (newItems == null)
			{
				newItems = System.Linq.Enumerable.Empty<T>();
			}
			IEnumerable<T> array = oldItems.Intersect<T>(newItems).ToArray<T>();
			IEnumerable<T> ts = newItems.Except<T>(array).ToArray<T>();
			foreach (T t in oldItems.Except<T>(array).ToArray<T>())
			{
				oldItems.Remove(t);
			}
			foreach (T t1 in ts)
			{
				oldItems.Add(t1);
			}
		}

		public static void PushMany<TBase, TDerived>(this Stack<TBase> stack, IEnumerable<TDerived> items)
		where TDerived : TBase
		{
			foreach (TDerived item in items)
			{
				stack.Push((TBase)(object)item);
			}
		}

		public static bool SetEquals<T>(this IEnumerable<T> left, IEnumerable<T> right)
		{
			return left.SetEquals<T>(right, EqualityComparer<T>.Default);
		}

		public static bool SetEquals<T>(this IEnumerable<T> left, IEnumerable<T> right, IEqualityComparer<T> comparer)
		{
			return !left.SetSymmetricDifference<T>(right, comparer).Any<T>();
		}

		public static IEnumerable<T> SetSymmetricDifference<T>(this IEnumerable<T> left, IEnumerable<T> right)
		{
			return left.SetSymmetricDifference<T>(right, EqualityComparer<T>.Default);
		}

		public static IEnumerable<T> SetSymmetricDifference<T>(this IEnumerable<T> left, IEnumerable<T> right, IEqualityComparer<T> comparer)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			if (object.ReferenceEquals(left, right))
			{
				return System.Linq.Enumerable.Empty<T>();
			}
			HashSet<T> ts = new HashSet<T>(left, comparer);
			ts.SymmetricExceptWith(right);
			return ts;
		}

		private static IEnumerable<TSource> SingleItemIterator<TSource>(TSource item)
		{
			yield return item;
		}

		public static TSource SingleOrNull<TSource>(this IEnumerable<TSource> source)
		where TSource : class
		{
			TSource tSource;
			TSource tSource1;
			if (source == null)
			{
				return default(TSource);
			}
			IList<TSource> tSources = source as IList<TSource>;
			if (tSources == null)
			{
				using (IEnumerator<TSource> enumerator = source.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						TSource current = enumerator.Current;
						if (enumerator.MoveNext())
						{
							tSource1 = default(TSource);
							return tSource1;
						}
						else
						{
							tSource = current;
						}
					}
					else
					{
						tSource = default(TSource);
					}
				}
				return tSource;
			}
			else if (tSources.Count == 1)
			{
				return tSources[0];
			}
			tSource1 = default(TSource);
			return tSource1;
		}

		public static object[] ToArray(this IEnumerable source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return EnumerableExtensions.ConvertToArray(source);
		}

		public static IEnumerable<Tuple<T, T>> ToConsecutivePairs<T>(this IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			using (IEnumerator<T> enumerator = items.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					while (enumerator.MoveNext())
					{
						yield return Tuple.Create<T, T>(current, enumerator.Current);
						current = enumerator.Current;
					}
				}
				else
				{
				}
			}
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			return new HashSet<T>(items);
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items, IEqualityComparer<T> comparer)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			return new HashSet<T>(items, comparer);
		}

		public static IList ToList(this IEnumerable source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			IList lists = source as IList;
			if (lists != null)
			{
				return lists;
			}
			return EnumerableExtensions.ConvertToArray(source);
		}
	}
}