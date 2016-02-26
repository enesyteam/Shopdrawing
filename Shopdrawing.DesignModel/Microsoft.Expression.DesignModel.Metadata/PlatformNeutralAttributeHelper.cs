using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public static class PlatformNeutralAttributeHelper
	{
		private static Dictionary<Type, HashSet<string>> attributeTypes;

		static PlatformNeutralAttributeHelper()
		{
			PlatformNeutralAttributeHelper.attributeTypes = new Dictionary<Type, HashSet<string>>();
		}

		public static bool AttributeExists(IEnumerable attributes, ITypeId attributeType)
		{
			bool flag;
			if (attributes != null)
			{
				IEnumerator enumerator = attributes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (!PlatformNeutralAttributeHelper.IsAssignableFrom(attributeType, enumerator.Current))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				return flag;
			}
			return false;
		}

		private static T GetValue<T>(object attribute, string propertyName)
		{
			object value;
			PropertyInfo property = attribute.GetType().GetProperty(propertyName);
			if (property != null)
			{
				try
				{
					value = property.GetValue(attribute, null);
				}
				catch (Exception exception)
				{
					value = property.GetValue(attribute, null);
				}
				if (value is T)
				{
					return (T)value;
				}
			}
			return default(T);
		}

		private static bool IsAssignableFrom(ITypeId attributeType, object attribute)
		{
			HashSet<string> strs;
			Type type = attribute.GetType();
			if (!PlatformNeutralAttributeHelper.attributeTypes.TryGetValue(type, out strs))
			{
				strs = new HashSet<string>(StringComparer.Ordinal);
				PlatformNeutralAttributeHelper.attributeTypes.Add(type, strs);
				while (type != null)
				{
					strs.Add(type.FullName);
					type = type.BaseType;
				}
			}
			return strs.Contains(attributeType.FullName);
		}

		public static bool TryGetAttributeValue<T>(IEnumerable attributes, ITypeId attributeType, string propertyName, out T result)
		{
			bool flag;
			if (attributes != null)
			{
				IEnumerator enumerator = attributes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.Current;
						if (!PlatformNeutralAttributeHelper.IsAssignableFrom(attributeType, current))
						{
							continue;
						}
						result = PlatformNeutralAttributeHelper.GetValue<T>(current, propertyName);
						flag = true;
						return flag;
					}
					result = default(T);
					return false;
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				return flag;
			}
			result = default(T);
			return false;
		}

		public static bool TryGetAttributeValue<T>(Type type, ITypeId attributeType, string propertyName, out T result)
		{
			return PlatformNeutralAttributeHelper.TryGetAttributeValue<T>(TypeUtilities.GetAttributes(type), attributeType, propertyName, out result);
		}

		public static bool TryGetAttributeValues<T>(IEnumerable attributes, ITypeId attributeType, string propertyName, out List<T> results)
		{
			results = new List<T>();
			if (attributes != null)
			{
				foreach (object attribute in attributes)
				{
					if (!PlatformNeutralAttributeHelper.IsAssignableFrom(attributeType, attribute))
					{
						continue;
					}
					results.Add(PlatformNeutralAttributeHelper.GetValue<T>(attribute, propertyName));
				}
			}
			return results.Count > 0;
		}

		public static bool TryGetAttributeValues<T>(Type type, ITypeId attributeType, string propertyName, out List<T> results)
		{
			return PlatformNeutralAttributeHelper.TryGetAttributeValues<T>(TypeUtilities.GetAttributes(type), attributeType, propertyName, out results);
		}
	}
}