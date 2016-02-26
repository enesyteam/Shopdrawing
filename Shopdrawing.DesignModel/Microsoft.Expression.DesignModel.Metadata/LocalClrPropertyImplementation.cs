using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class LocalClrPropertyImplementation : ClrPropertyImplementationBase
	{
		private System.Reflection.PropertyInfo propertyInfo;

		private MethodInfo getMethod;

		private MethodInfo setMethod;

		private bool shouldSerializeMethodInitialized;

		private MethodInfo shouldSerializeMethod;

		private object[] indexParameters;

		private bool isResolved;

		private DesignerSerializationVisibility? serializationVisibility;

		private bool typeConverterInitialized;

		private System.ComponentModel.TypeConverter typeConverter;

		private KeyValuePair<bool, object>? defaultValue;

		private string constructorArgument;

		private AttributeCollection attributes;

		private Type declaringType;

		public override AttributeCollection Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new AttributeCollection(base.PlatformMetadata.GetCustomAttributes(this.propertyInfo, true));
				}
				return this.attributes;
			}
		}

		public override string ConstructorArgument
		{
			get
			{
				string str;
				if (this.constructorArgument == null && PlatformNeutralAttributeHelper.TryGetAttributeValue<string>(this.Attributes, PlatformTypes.ConstructorArgumentAttribute, "ArgumentName", out str))
				{
					this.constructorArgument = str;
				}
				return this.constructorArgument;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.declaringType;
			}
		}

		public override object DefaultValue
		{
			get
			{
				return this.PrivateDefaultValue.Value;
			}
		}

		public override MethodInfo GetMethod
		{
			get
			{
				return this.getMethod;
			}
		}

		public override bool HasDefault
		{
			get
			{
				return this.PrivateDefaultValue.Key;
			}
		}

		public bool IsReadable
		{
			get
			{
				return this.getMethod != null;
			}
		}

		public override bool IsResolved
		{
			get
			{
				return this.isResolved;
			}
		}

		public bool IsWritable
		{
			get
			{
				return this.setMethod != null;
			}
		}

		public override string Name
		{
			get
			{
				return this.propertyInfo.Name;
			}
		}

		private KeyValuePair<bool, object> PrivateDefaultValue
		{
			get
			{
				if (!this.defaultValue.HasValue)
				{
					Type propertyType = PlatformTypeHelper.GetPropertyType(this.propertyInfo);
					if (propertyType == null)
					{
						this.defaultValue = new KeyValuePair<bool, object>?(new KeyValuePair<bool, object>(false, null));
					}
					else
					{
						this.defaultValue = new KeyValuePair<bool, object>?(PlatformTypeHelper.GetDefaultValue((PlatformTypes)base.PlatformMetadata, this.propertyInfo, propertyType));
					}
				}
				return this.defaultValue.Value;
			}
		}

		public override System.Reflection.PropertyInfo PropertyInfo
		{
			get
			{
				return this.propertyInfo;
			}
		}

		public override MemberAccessType ReadAccess
		{
			get
			{
				if (this.getMethod == null)
				{
					return MemberAccessType.None;
				}
				return PlatformTypeHelper.GetMemberAccess(this.getMethod);
			}
		}

		public override MethodInfo SetMethod
		{
			get
			{
				return this.setMethod;
			}
		}

		public override bool ShouldSerialize
		{
			get
			{
				if (!this.serializationVisibility.HasValue)
				{
					this.serializationVisibility = new DesignerSerializationVisibility?(PlatformTypeHelper.GetSerializationVisibility((PlatformTypes)base.PlatformMetadata, this.propertyInfo));
				}
				return this.serializationVisibility.Value != DesignerSerializationVisibility.Hidden;
			}
		}

		public MethodInfo ShouldSerializeMethod
		{
			get
			{
				if (!this.shouldSerializeMethodInitialized)
				{
					this.shouldSerializeMethodInitialized = true;
					if (!this.propertyInfo.Name.Contains("."))
					{
						Type declaringType = this.DeclaringType;
						MethodInfo method = PlatformTypeHelper.GetMethod(declaringType, string.Concat("ShouldSerialize", this.propertyInfo.Name));
						if (method != null && (int)method.GetParameters().Length == 0 && method.ReturnParameter != null && method.ReturnParameter.ParameterType == typeof(bool))
						{
							this.shouldSerializeMethod = method;
						}
					}
				}
				return this.shouldSerializeMethod;
			}
		}

		public override Type TargetType
		{
			get
			{
				return this.DeclaringType;
			}
		}

		public override System.ComponentModel.TypeConverter TypeConverter
		{
			get
			{
				if (!this.typeConverterInitialized)
				{
					this.typeConverter = base.PlatformMetadata.GetTypeConverter(this.propertyInfo);
					this.typeConverterInitialized = true;
				}
				return this.typeConverter;
			}
		}

		public override Type ValueType
		{
			get
			{
				return PlatformTypeHelper.GetPropertyType(this.propertyInfo);
			}
		}

		public override MemberAccessType WriteAccess
		{
			get
			{
				if (this.setMethod == null)
				{
					return MemberAccessType.None;
				}
				return PlatformTypeHelper.GetMemberAccess(this.setMethod);
			}
		}

		public LocalClrPropertyImplementation(IPlatformMetadata platformMetadata, System.Reflection.PropertyInfo propertyInfo, Type declaringType, object[] indexParameters) : base(platformMetadata)
		{
			this.declaringType = declaringType ?? propertyInfo.DeclaringType;
			this.propertyInfo = propertyInfo;
			this.getMethod = this.propertyInfo.GetGetMethod(true);
			this.setMethod = this.propertyInfo.GetSetMethod(true);
			this.indexParameters = indexParameters;
			this.isResolved = PlatformTypeHelper.GetPropertyType(this.propertyInfo) != null;
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherits)
		{
			PlatformTypes platformMetadata = (PlatformTypes)base.PlatformMetadata;
			return platformMetadata.GetCustomAttributes(this.propertyInfo, attributeType, inherits);
		}

		public override object GetValue(object target)
		{
			if (!this.IsReadable)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string propertyCannotReadValue = ExceptionStringTable.PropertyCannotReadValue;
				object[] name = new object[] { this.DeclaringType.Name, this.Name };
				throw new InvalidOperationException(string.Format(currentCulture, propertyCannotReadValue, name));
			}
			return this.propertyInfo.GetValue(target, this.indexParameters);
		}

		public override void SetValue(object target, object valueToSet)
		{
			if (!this.IsWritable)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string propertyCannotWriteValue = ExceptionStringTable.PropertyCannotWriteValue;
				object[] name = new object[] { this.DeclaringType.Name, this.Name };
				throw new InvalidOperationException(string.Format(currentCulture, propertyCannotWriteValue, name));
			}
			this.propertyInfo.SetValue(target, valueToSet, this.indexParameters);
		}
	}
}