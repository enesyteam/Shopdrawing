using Microsoft.Windows.Design.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal static class AttributeDataCache
	{
		private static Hashtable _baseMemberMap;

		private static Hashtable _attributeDataCache;

		private static Dictionary<Type, object[]> typeAttributeCache;

		private static object _noMemberInfo;

		private static object _syncObject;

		private static Dictionary<MemberTypes, AttributeDataCache.GetBaseMemberCallback> _baseMemberFinders;

		private readonly static BindingFlags _getInfoBindingFlags;

		static AttributeDataCache()
		{
			AttributeDataCache._baseMemberMap = new Hashtable();
			AttributeDataCache._attributeDataCache = new Hashtable();
			AttributeDataCache.typeAttributeCache = new Dictionary<Type, object[]>();
			AttributeDataCache._noMemberInfo = new object();
			AttributeDataCache._syncObject = new object();
			AttributeDataCache._getInfoBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			AttributeDataCache._baseMemberFinders = new Dictionary<MemberTypes, AttributeDataCache.GetBaseMemberCallback>();
			AttributeDataCache._baseMemberFinders[MemberTypes.Constructor] = new AttributeDataCache.GetBaseMemberCallback(AttributeDataCache.GetBaseConstructorInfo);
			AttributeDataCache._baseMemberFinders[MemberTypes.Method] = new AttributeDataCache.GetBaseMemberCallback(AttributeDataCache.GetBaseMethodInfo);
			AttributeDataCache._baseMemberFinders[MemberTypes.Property] = new AttributeDataCache.GetBaseMemberCallback(AttributeDataCache.GetBasePropertyInfo);
			AttributeDataCache._baseMemberFinders[MemberTypes.Event] = new AttributeDataCache.GetBaseMemberCallback(AttributeDataCache.GetBaseEventInfo);
		}

		private static MemberInfo CalculateBaseMemberInfo(MemberInfo member)
		{
			Type type = member as Type;
			if (type != null)
			{
				return type.BaseType;
			}
			Type baseType = member.DeclaringType.BaseType;
			MemberInfo item = null;
			while (baseType != null && item == null)
			{
				item = AttributeDataCache._baseMemberFinders[member.MemberType](member, baseType);
				baseType = baseType.BaseType;
			}
			return item;
		}

		internal static AttributeData GetAttributeData(Type attributeType)
		{
			AttributeData item = AttributeDataCache._attributeDataCache[attributeType] as AttributeData;
			if (item == null)
			{
				item = new AttributeData(attributeType);
				lock (AttributeDataCache._syncObject)
				{
					AttributeDataCache._attributeDataCache[attributeType] = item;
				}
			}
			return item;
		}

		private static MemberInfo GetBaseConstructorInfo(MemberInfo info, Type targetType)
		{
			return null;
		}

		private static MemberInfo GetBaseEventInfo(MemberInfo info, Type targetType)
		{
			return targetType.GetEvent((info as EventInfo).Name, AttributeDataCache._getInfoBindingFlags);
		}

		internal static MemberInfo GetBaseMemberInfo(MemberInfo member)
		{
			object item = AttributeDataCache._baseMemberMap[member];
			if (item == AttributeDataCache._noMemberInfo)
			{
				return null;
			}
			if (item == null)
			{
				item = AttributeDataCache.CalculateBaseMemberInfo(member);
				lock (AttributeDataCache._syncObject)
				{
					AttributeDataCache._baseMemberMap[member] = item ?? AttributeDataCache._noMemberInfo;
				}
			}
			return (MemberInfo)item;
		}

		private static MemberInfo GetBaseMethodInfo(MemberInfo info, Type targetType)
		{
			MethodInfo methodInfo = info as MethodInfo;
			return targetType.GetMethod(methodInfo.Name, AttributeDataCache._getInfoBindingFlags, null, AttributeDataCache.ToTypeArray(methodInfo.GetParameters()), null);
		}

		private static MemberInfo GetBasePropertyInfo(MemberInfo info, Type targetType)
		{
			PropertyInfo propertyInfo = info as PropertyInfo;
			return targetType.GetProperty(propertyInfo.Name, AttributeDataCache._getInfoBindingFlags, null, propertyInfo.PropertyType, AttributeDataCache.ToTypeArray(propertyInfo.GetIndexParameters()), null);
		}

		internal static IEnumerable<object> GetClrAttributes(MemberInfo member)
		{
			object[] objArray;
			IEnumerable<object> objs;
			Type type = member as Type;
			if (type != null && AttributeDataCache.typeAttributeCache.TryGetValue(type, out objArray))
			{
				return objArray;
			}
			try
			{
				object[] customAttributes = member.GetCustomAttributes(false);
				if (type != null)
				{
					AttributeDataCache.typeAttributeCache.Add(type, customAttributes);
				}
				objs = customAttributes;
			}
			catch
			{
				return new List<object>();
			}
			return objs;
		}

		internal static IEnumerable<object> GetMetadataStoreAttributes(Type type, string memberName, AttributeTable[] tables)
		{
			IEnumerable enumerable;
			if (tables != null && (int)tables.Length != 0)
			{
				AttributeTable[] attributeTableArray = tables;
				for (int i = 0; i < (int)attributeTableArray.Length; i++)
				{
					AttributeTable attributeTable = attributeTableArray[i];
					if (attributeTable.ContainsAttributes(type))
					{
						enumerable = (memberName != null ? attributeTable.GetCustomAttributes(type, memberName) : attributeTable.GetCustomAttributes(type));
						foreach (object obj in enumerable)
						{
							yield return obj;
						}
					}
				}
			}
		}

		private static Type[] ToTypeArray(ParameterInfo[] parameterInfo)
		{
			if (parameterInfo == null)
			{
				return null;
			}
			Type[] parameterType = new Type[(int)parameterInfo.Length];
			for (int i = 0; i < (int)parameterInfo.Length; i++)
			{
				parameterType[i] = parameterInfo[i].ParameterType;
			}
			return parameterType;
		}

		private delegate MemberInfo GetBaseMemberCallback(MemberInfo member, Type targetType);
	}
}