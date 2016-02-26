using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class ClrPropertyReferenceStep : ReferenceStep, IPropertyImplementation
	{
		private ClrPropertyImplementationBase implementation;

		public override MemberAccessType Access
		{
			get
			{
				return this.implementation.Access;
			}
		}

		public override AttributeCollection Attributes
		{
			get
			{
				return this.implementation.Attributes;
			}
		}

		public override string ConstructorArgument
		{
			get
			{
				return this.implementation.ConstructorArgument;
			}
		}

		public MethodInfo GetMethod
		{
			get
			{
				return this.implementation.GetMethod;
			}
		}

		public override bool IsAttachable
		{
			get
			{
				return this.implementation.IsAttachable;
			}
		}

		public override bool IsProxy
		{
			get
			{
				return this.implementation.IsProxy;
			}
		}

		public override bool IsResolvable
		{
			get
			{
				return this.implementation.IsResolved;
			}
		}

		public override Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return this.implementation.MemberType;
			}
		}

		public override ITypeId MemberTypeId
		{
			get
			{
				return this.implementation.MemberTypeId;
			}
		}

		PropertyImplementationBase Microsoft.Expression.DesignModel.Metadata.IPropertyImplementation.Implementation
		{
			get
			{
				return this.implementation;
			}
			set
			{
				this.implementation = value as ClrPropertyImplementationBase;
				if (this.implementation == null)
				{
					this.implementation = ((PlatformTypes)base.DeclaringType.PlatformMetadata).UndefinedClrPropertyImplementationInstance;
				}
			}
		}

		public System.Reflection.PropertyInfo PropertyInfo
		{
			get
			{
				LocalClrPropertyImplementation localClrPropertyImplementation = this.implementation as LocalClrPropertyImplementation;
				if (localClrPropertyImplementation == null)
				{
					return null;
				}
				return localClrPropertyImplementation.PropertyInfo;
			}
		}

		public override MemberAccessType ReadAccess
		{
			get
			{
				return this.implementation.ReadAccess;
			}
		}

		public MethodInfo SetMethod
		{
			get
			{
				return this.implementation.SetMethod;
			}
		}

		public override bool ShouldSerialize
		{
			get
			{
				return this.implementation.ShouldSerialize;
			}
		}

		public sealed override Type TargetType
		{
			get
			{
				return this.implementation.TargetType;
			}
		}

		public override System.ComponentModel.TypeConverter TypeConverter
		{
			get
			{
				return this.implementation.TypeConverter;
			}
		}

		public override MemberAccessType WriteAccess
		{
			get
			{
				return this.implementation.WriteAccess;
			}
		}

		internal ClrPropertyReferenceStep(IType declaringType, string memberName, IType valueType, ClrPropertyImplementationBase implementation, int sortIndex) : base(declaringType, memberName, valueType, sortIndex)
		{
			this.implementation = implementation;
		}

		public sealed override void ClearValue(object target)
		{
			this.implementation.ClearValue(target);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return this.implementation.GetCustomAttributes(attributeType, inherit);
		}

		public override object GetDefaultValue(Type targetType)
		{
			return this.implementation.GetDefaultValue(targetType);
		}

		protected virtual object[] GetIndexParameters()
		{
			return null;
		}

		public sealed override object GetValue(object target)
		{
			return this.implementation.GetValue(target);
		}

		public override bool HasDefaultValue(Type targetType)
		{
			return this.implementation.HasDefaultValue(targetType);
		}

		public sealed override bool IsAnimated(object target)
		{
			return false;
		}

		public sealed override bool IsSet(object target)
		{
			return this.GetValue(target) != DependencyProperty.UnsetValue;
		}

		void Microsoft.Expression.DesignModel.Metadata.IPropertyImplementation.Invalidate()
		{
			this.implementation = ((PlatformTypes)base.DeclaringType.PlatformMetadata).UndefinedClrPropertyImplementationInstance;
		}

		public sealed override object SetValue(object target, object valueToSet)
		{
			this.implementation.SetValue(target, valueToSet);
			return target;
		}

		public sealed override bool ShouldSerializeMethod(object objToInspect)
		{
			LocalClrPropertyImplementation localClrPropertyImplementation = this.implementation as LocalClrPropertyImplementation;
			if (localClrPropertyImplementation == null || !(localClrPropertyImplementation.ShouldSerializeMethod != null))
			{
				return true;
			}
			return (bool)localClrPropertyImplementation.ShouldSerializeMethod.Invoke(objToInspect, new object[0]);
		}
	}
}