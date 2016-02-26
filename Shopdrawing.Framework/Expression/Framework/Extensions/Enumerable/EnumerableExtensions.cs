using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.Expression.Framework.Extensions.Enumerable
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

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
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
            if (object.ReferenceEquals(left, right)) //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
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

        
    }
}