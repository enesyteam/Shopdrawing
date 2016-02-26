using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Utility.Extensions.Dictionary
{
	public static class DictionaryExtensions
	{
		public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (source.ContainsKey(key))
			{
				source[key] = value;
				return;
			}
			source.Add(key, value);
		}

		public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (updateValueFactory == null)
			{
				throw new ArgumentNullException("updateValueFactory");
			}
			if (!source.ContainsKey(key))
			{
				source.Add(key, addValue);
				return addValue;
			}
			TValue tValue = updateValueFactory(key, source[key]);
			source[key] = tValue;
			return tValue;
		}

		public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (updateValueFactory == null)
			{
				throw new ArgumentNullException("addValueFactory");
			}
			if (updateValueFactory == null)
			{
				throw new ArgumentNullException("updateValueFactory");
			}
			if (!source.ContainsKey(key))
			{
				TValue tValue = addValueFactory(key);
				source.Add(key, tValue);
				return tValue;
			}
			TValue tValue1 = updateValueFactory(key, source[key]);
			source[key] = tValue1;
			return tValue1;
		}

		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (source.ContainsKey(key))
			{
				return source[key];
			}
			source.Add(key, value);
			return value;
		}

		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TKey, TValue> addValueFactory)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (addValueFactory == null)
			{
				throw new ArgumentNullException("addValueFactory");
			}
			if (source.ContainsKey(key))
			{
				return source[key];
			}
			TValue tValue = addValueFactory(key);
			source.Add(key, tValue);
			return tValue;
		}

		public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
		{
			return source.Remove(key);
		}

		public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, out TValue value)
		{
			if (!source.TryGetValue(key, out value))
			{
				return false;
			}
			return source.Remove(key);
		}
	}
}