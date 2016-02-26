using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public static class TypeUtilities
	{
		public static bool CanCreateTypeInXaml(ITypeResolver typeResolver, Type type)
		{
			IType type1 = typeResolver.GetType(type);
			if (typeResolver.PlatformMetadata.IsNullType(type1) || !TypeHelper.IsAccessibleType(typeResolver, type1) || Nullable.GetUnderlyingType(type) != null)
			{
				return false;
			}
			if (type == typeof(string) || TypeUtilities.HasDefaultConstructor(type, typeResolver.InTargetAssembly(type1)))
			{
				return true;
			}
			return type1.TypeConverter.CanConvertFrom(typeof(string));
		}

		public static Attribute[] CullDuplicateAttributes(IList<Attribute> attributes)
		{
			HashSet<object> objs = new HashSet<object>();
			int count = attributes.Count;
			for (int i = 0; i < attributes.Count; i++)
			{
				Attribute item = attributes[i];
				if (objs.Contains(item.TypeId))
				{
					attributes[i] = null;
					count--;
				}
				else if (!AttributeDataCache.GetAttributeData(item.GetType()).AllowsMultiple)
				{
					objs.Add(item.TypeId);
				}
			}
			Attribute[] attributeArray = new Attribute[count];
			int num = 0;
			int num1 = 0;
			while (num < attributes.Count)
			{
				if (attributes[num] != null)
				{
					int num2 = num1;
					num1 = num2 + 1;
					attributeArray[num2] = attributes[num];
				}
				num++;
			}
			return attributeArray;
		}

		public static Attribute GetAttribute(Type type, Type attributeType)
		{
			return TypeUtilities.GetAttributes(type)[attributeType];
		}

		public static T GetAttribute<T>(Type type)
		where T : Attribute
		{
			return (T)(TypeUtilities.GetAttribute(type, typeof(T)) as T);
		}

		public static AttributeCollection GetAttributes(Type type)
		{
			if (type == null)
			{
				return AttributeCollection.Empty;
			}
			return new AttributeCollection(TypeUtilities.GetAttributes(type, null, true));
		}

		public static Attribute[] GetAttributes(MemberInfo memberInfo, Type attributeType, bool inherit)
		{
			bool flag;
			if (memberInfo == null)
			{
				return new Attribute[0];
			}
			List<Attribute> attributes = new List<Attribute>();
			Type declaringType = memberInfo as Type;
			Type valueType = null;
			if (declaringType == null)
			{
				flag = false;
				declaringType = memberInfo.DeclaringType;
				valueType = TypeUtilities.GetValueType(memberInfo);
			}
			else
			{
				flag = true;
			}
			bool flag1 = true;
			bool flag2 = (flag ? true : memberInfo.DeclaringType == declaringType);
			while (declaringType != null && memberInfo != null)
			{
				foreach (object obj in TypeUtilities.MergeAttributesIterator(declaringType, memberInfo, flag2))
				{
					if (attributeType != null && !attributeType.IsAssignableFrom(obj.GetType()))
					{
						continue;
					}
					AttributeData attributeData = AttributeDataCache.GetAttributeData(obj.GetType());
					if (!flag1 && !attributeData.IsInheritable)
					{
						continue;
					}
					Attribute attribute = obj as Attribute;
					if (attribute == null)
					{
						continue;
					}
					attributes.Add(attribute);
				}
				if (!inherit || memberInfo.MemberType == MemberTypes.Field)
				{
					break;
				}
				if (flag || memberInfo.DeclaringType == declaringType)
				{
					memberInfo = AttributeDataCache.GetBaseMemberInfo(memberInfo);
				}
				declaringType = declaringType.BaseType;
				flag2 = (flag || memberInfo == null || memberInfo.DeclaringType == declaringType ? true : false);
				flag1 = false;
			}
			if (valueType != null && inherit)
			{
				attributes.AddRange(TypeUtilities.GetAttributes(valueType, attributeType, true));
			}
			return TypeUtilities.CullDuplicateAttributes(attributes);
		}

		public static AttributeCollection GetAttributes(MemberDescriptor memberDescriptor)
		{
			AttributeCollection attributes;
			try
			{
				attributes = memberDescriptor.Attributes;
			}
			catch (Exception exception)
			{
				attributes = new AttributeCollection(new Attribute[0]);
			}
			return attributes;
		}

		public static EventDescriptorCollection GetEvents(Type type)
		{
			EventDescriptorCollection events;
			try
			{
				events = TypeDescriptor.GetEvents(type);
			}
			catch (Exception exception)
			{
				events = new EventDescriptorCollection(new EventDescriptor[0]);
			}
			return events;
		}

		private static string GetMemberName(MemberInfo member)
		{
			if (member is Type)
			{
				return null;
			}
			if (member.MemberType != MemberTypes.Method)
			{
				return member.Name;
			}
			return member.Name.Substring(3);
		}

		public static PropertyDescriptorCollection GetProperties(Type type)
		{
			PropertyDescriptorCollection properties;
			try
			{
				properties = TypeDescriptor.GetProperties(type);
			}
			catch (Exception exception)
			{
				properties = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			}
			return properties;
		}

		public static PropertyDescriptorCollection GetProperties(object component)
		{
			PropertyDescriptorCollection properties;
			try
			{
				properties = TypeDescriptor.GetProperties(component);
			}
			catch (Exception exception)
			{
				properties = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			}
			return properties;
		}

		public static PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties;
			try
			{
				properties = TypeDescriptor.GetProperties(component, attributes);
			}
			catch (Exception exception)
			{
				properties = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			}
			return properties;
		}

		public static Type GetValueType(MemberInfo memberInfo)
		{
			Type propertyType = null;
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			EventInfo eventInfo = memberInfo as EventInfo;
			if (propertyInfo != null)
			{
				propertyType = propertyInfo.PropertyType;
			}
			else if (fieldInfo != null)
			{
				propertyType = fieldInfo.FieldType;
			}
			else if (eventInfo != null)
			{
				propertyType = eventInfo.EventHandlerType;
			}
			return propertyType;
		}

		public static bool HasDefaultConstructor(Type type, bool supportInternal)
		{
			ConstructorAccessibility constructorAccessibility;
			PlatformTypeHelper.GetDefaultConstructor(type, supportInternal, out constructorAccessibility);
			ConstructorAccessibility constructorAccessibility1 = constructorAccessibility;
			if (constructorAccessibility1 != ConstructorAccessibility.Accessible && constructorAccessibility1 != ConstructorAccessibility.TypeIsValueType)
			{
				return false;
			}
			return true;
		}

		private static IEnumerable<object> MergeAttributesIterator(Type type, MemberInfo member, bool includeClrAttributes)
		{
			string str = TypeUtilities.GetMemberName(member);
			if (str == null)
			{
				foreach (object attribute in MetadataStore.GetAttributes(type))
				{
					yield return attribute;
				}
			}
			else
			{
				foreach (object obj in MetadataStore.GetAttributes(type, str))
				{
					yield return obj;
				}
			}
			if (includeClrAttributes)
			{
				foreach (object clrAttribute in AttributeDataCache.GetClrAttributes(member))
				{
					yield return clrAttribute;
				}
			}
		}
	}
}