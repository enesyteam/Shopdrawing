using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class AttachedClrPropertyImplementation : ClrPropertyImplementationBase
	{
		private string name;

		private MethodInfo getMethod;

		private MethodInfo setMethod;

		private Type targetType;

		private Type valueType;

		private bool isResolved;

		private DesignerSerializationVisibility? serializationVisibility;

		private bool typeConverterInitialized;

		private System.ComponentModel.TypeConverter typeConverter;

		private KeyValuePair<bool, object>? defaultValue;

		private AttributeCollection attributes;

		public override AttributeCollection Attributes
		{
			get
			{
				if (this.attributes == null && this.getMethod != null)
				{
					this.attributes = new AttributeCollection(((PlatformTypes)base.PlatformMetadata).GetCustomAttributes(this.getMethod, null, true));
				}
				return this.attributes;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return ((this.getMethod != null ? this.getMethod : this.setMethod)).DeclaringType;
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

		public override bool IsAttachable
		{
			get
			{
				return true;
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

		public override Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return Microsoft.Expression.DesignModel.Metadata.MemberType.AttachedProperty;
			}
		}

		public override ITypeId MemberTypeId
		{
			get
			{
				return PlatformTypes.MethodInfo;
			}
		}

		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		private KeyValuePair<bool, object> PrivateDefaultValue
		{
			get
			{
				if (!this.defaultValue.HasValue)
				{
					if (this.valueType == null)
					{
						this.defaultValue = new KeyValuePair<bool, object>?(new KeyValuePair<bool, object>(false, null));
					}
					else if (this.getMethod == null)
					{
						this.defaultValue = new KeyValuePair<bool, object>?(PlatformTypeHelper.GetDefaultValue(this.valueType));
					}
					else
					{
						this.defaultValue = new KeyValuePair<bool, object>?(PlatformTypeHelper.GetDefaultValue((PlatformTypes)base.PlatformMetadata, this.getMethod, this.valueType));
					}
				}
				return this.defaultValue.Value;
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
					if (this.getMethod == null)
					{
						this.serializationVisibility = new DesignerSerializationVisibility?(DesignerSerializationVisibility.Visible);
					}
					else
					{
						this.serializationVisibility = new DesignerSerializationVisibility?(PlatformTypeHelper.GetSerializationVisibility((PlatformTypes)base.PlatformMetadata, this.getMethod));
					}
				}
				return this.serializationVisibility.Value != DesignerSerializationVisibility.Hidden;
			}
		}

		public override Type TargetType
		{
			get
			{
				return this.targetType;
			}
		}

		public override System.ComponentModel.TypeConverter TypeConverter
		{
			get
			{
				if (!this.typeConverterInitialized)
				{
					if (this.getMethod == null)
					{
						this.typeConverter = null;
					}
					else
					{
						this.typeConverter = base.PlatformMetadata.GetTypeConverter(this.getMethod);
					}
					this.typeConverterInitialized = true;
				}
				return this.typeConverter;
			}
		}

		public override Type ValueType
		{
			get
			{
				return this.valueType;
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

		public AttachedClrPropertyImplementation(IPlatformMetadata platformMetadata, string name, MethodInfo getMethod, MethodInfo setMethod) : base(platformMetadata)
		{
			Type valueType;
			Type type;
			if (getMethod == null && setMethod == null)
			{
				throw new ArgumentNullException();
			}
			this.name = name;
			this.getMethod = getMethod;
			this.setMethod = setMethod;
			if (this.getMethod == null)
			{
				ParameterInfo[] parameters = this.setMethod.GetParameters();
				valueType = PlatformTypeHelper.GetValueType(parameters[0]);
				type = PlatformTypeHelper.GetValueType(parameters[1]);
			}
			else
			{
				valueType = PlatformTypeHelper.GetValueType(this.getMethod.GetParameters()[0]);
				type = PlatformTypeHelper.GetValueType(this.getMethod);
			}
			this.targetType = (valueType != null ? valueType : typeof(DependencyObject));
			this.valueType = (type != null ? type : typeof(object));
			this.isResolved = type != null;
		}

		public override object GetValue(object target)
		{
			if (!this.IsReadable)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string propertyCannotReadValue = ExceptionStringTable.PropertyCannotReadValue;
				object[] name = new object[] { this.DeclaringType.Name, this.name };
				throw new InvalidOperationException(string.Format(currentCulture, propertyCannotReadValue, name));
			}
			return this.getMethod.Invoke(null, new object[] { target });
		}

		public override void SetValue(object target, object valueToSet)
		{
			if (!this.IsWritable)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string propertyCannotWriteValue = ExceptionStringTable.PropertyCannotWriteValue;
				object[] name = new object[] { this.DeclaringType.Name, this.name };
				throw new InvalidOperationException(string.Format(currentCulture, propertyCannotWriteValue, name));
			}
			MethodInfo methodInfo = this.setMethod;
			object[] objArray = new object[] { target, valueToSet };
			methodInfo.Invoke(null, objArray);
		}
	}
}