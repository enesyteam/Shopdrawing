using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.Expression.DesignModel.Metadata
{
	[DebuggerDisplay("{DebugDisplayValue}")]
	public class BuildingTypeInfo
	{
		private List<KeyValuePair<MethodBuilder, MethodInfo>> designMethods = new List<KeyValuePair<MethodBuilder, MethodInfo>>();

		private List<KeyValuePair<PropertyBuilder, PropertyInfo>> designProperties = new List<KeyValuePair<PropertyBuilder, PropertyInfo>>();

		public BuildingTypeInfo BaseTypeInfo
		{
			get;
			set;
		}

		public ConstructorInfo Constructor
		{
			get;
			set;
		}

		public string DebugDisplayValue
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.SourceType.Namespace);
				stringBuilder.Append(".");
				stringBuilder.Append(BuildingTypeInfo.GetTypeDisplayName(this.SourceType));
				if (this.DesignType != null)
				{
					Type designType = this.DesignType;
					if (this.IsReplacement)
					{
						bool isGenericType = this.SourceType.IsGenericType;
					}
					stringBuilder.Append("  =>  ");
					stringBuilder.Append(designType.Namespace);
					stringBuilder.Append(".");
					stringBuilder.Append(BuildingTypeInfo.GetTypeDisplayName(designType));
					if (this.DesignType is TypeBuilder)
					{
						stringBuilder.Append(", unbuilt");
					}
				}
				if (this.IsReplacement)
				{
					stringBuilder.Append(", replacement");
				}
				return stringBuilder.ToString();
			}
		}

		public Type DesignType
		{
			get;
			set;
		}

		public Dictionary<object, object> Extensions
		{
			get;
			private set;
		}

		public bool IsReplacement
		{
			get;
			set;
		}

		public bool IsReplacementCreated
		{
			get;
			set;
		}

		public Type SourceType
		{
			get;
			set;
		}

		public BuildingTypeInfo()
		{
			this.Extensions = new Dictionary<object, object>();
		}

		public void AddMethod(MethodBuilder designMethod, MethodInfo sourceMethod)
		{
			this.designMethods.Add(new KeyValuePair<MethodBuilder, MethodInfo>(designMethod, sourceMethod));
		}

		public void AddProperty(PropertyBuilder designProperty, PropertyInfo sourceProperty)
		{
			this.designProperties.Add(new KeyValuePair<PropertyBuilder, PropertyInfo>(designProperty, sourceProperty));
		}

		private static string GetDesignPrefix(Type type)
		{
			if (type.Namespace.StartsWith("_d", StringComparison.Ordinal))
			{
				int num = type.Namespace.IndexOf('.');
				if (num > 0)
				{
					return type.Namespace.Substring(0, num + 1);
				}
			}
			return string.Empty;
		}

		private static string GetTypeDisplayName(Type type)
		{
			string str = string.Concat(BuildingTypeInfo.GetDesignPrefix(type), type.Name);
			if (type.IsArray)
			{
				Type arrayItemType = DesignTypeGenerator.GetArrayItemType(type);
				if (arrayItemType != null)
				{
					str = string.Concat(BuildingTypeInfo.GetTypeDisplayName(arrayItemType), "[]");
					string designPrefix = BuildingTypeInfo.GetDesignPrefix(arrayItemType);
					if (string.IsNullOrEmpty(designPrefix))
					{
						str = string.Concat(designPrefix, str);
					}
				}
			}
			if (!type.IsGenericType)
			{
				return str;
			}
			int num = str.IndexOf('\u0060');
			if (num > 0)
			{
				str = str.Substring(0, num);
			}
			str = string.Concat(str, "<");
			Type[] genericArguments = type.GetGenericArguments();
			for (int i = 0; i < (int)genericArguments.Length; i++)
			{
				if (i > 0)
				{
					str = string.Concat(str, ", ");
				}
				string typeDisplayName = BuildingTypeInfo.GetTypeDisplayName(genericArguments[i]);
				str = string.Concat(str, typeDisplayName);
			}
			str = string.Concat(str, ">");
			return str;
		}

		public bool MethodExists(MethodInfo sourceMethod)
		{
			if (sourceMethod.Name.IndexOf('.') >= 0)
			{
				return true;
			}
			for (int i = 0; i < this.designMethods.Count; i++)
			{
				KeyValuePair<MethodBuilder, MethodInfo> item = this.designMethods[i];
				if (item.Value == sourceMethod)
				{
					return true;
				}
				if (!(item.Key.Name != sourceMethod.Name) && object.Equals(item.Key.ReturnType, sourceMethod.ReturnType))
				{
					if (item.Value == null)
					{
						return true;
					}
					if (DesignTypeGenerator.CompareParameters(item.Value.GetParameters(), sourceMethod.GetParameters()))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool PropertyExists(PropertyInfo sourceProperty)
		{
			if (sourceProperty.Name.IndexOf('.') >= 0)
			{
				return true;
			}
			for (int i = 0; i < this.designProperties.Count; i++)
			{
				KeyValuePair<PropertyBuilder, PropertyInfo> item = this.designProperties[i];
				if (item.Value == sourceProperty)
				{
					return true;
				}
				if (!(item.Key.Name != sourceProperty.Name) && object.Equals(item.Key.PropertyType, sourceProperty.PropertyType))
				{
					if (item.Value == null)
					{
						return true;
					}
					if (DesignTypeGenerator.CompareParameters(item.Value.GetIndexParameters(), sourceProperty.GetIndexParameters()))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}