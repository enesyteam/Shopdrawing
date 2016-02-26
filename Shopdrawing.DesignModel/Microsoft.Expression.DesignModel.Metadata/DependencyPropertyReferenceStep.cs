using Microsoft.Expression.DesignModel.Markup;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public sealed class DependencyPropertyReferenceStep : ReferenceStep, IPropertyImplementation, IDependencyProperty, IProperty, IMember, IPropertyId, IMemberId
	{
		private DependencyPropertyImplementationBase implementation;

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

		public object DependencyProperty
		{
			get
			{
				return this.implementation.DependencyProperty;
			}
		}

		public override bool IsAttachable
		{
			get
			{
				return this.implementation.IsAttachable;
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
				this.implementation = value as DependencyPropertyImplementationBase;
				if (this.implementation == null)
				{
					this.implementation = UndefinedDependencyPropertyImplementation.Instance;
				}
			}
		}

		public Microsoft.Expression.DesignModel.Metadata.PlatformTypes PlatformTypes
		{
			get
			{
				return (Microsoft.Expression.DesignModel.Metadata.PlatformTypes)this.implementation.ClrImplementation.PlatformMetadata;
			}
		}

		public override MemberAccessType ReadAccess
		{
			get
			{
				return this.implementation.ReadAccess;
			}
		}

		public override bool ShouldSerialize
		{
			get
			{
				return this.implementation.ShouldSerialize;
			}
		}

		public override Type TargetType
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

		internal DependencyPropertyReferenceStep(IType declaringType, string memberName, IType valueType, DependencyPropertyImplementationBase implementation, int sortValue) : base(declaringType, memberName, valueType, sortValue)
		{
			this.implementation = implementation;
		}

		internal DependencyPropertyReferenceStep(IType declaringType, string memberName, IType valueType, DependencyPropertyImplementationBase implementation) : this(declaringType, memberName, valueType, implementation, PropertySortValue.NoValue)
		{
		}

		public bool BindsTwoWayByDefault(Type targetType)
		{
			return this.implementation.BindsTwoWayByDefault(targetType);
		}

		public override void ClearValue(object target)
		{
			this.implementation.ClearValue(target);
		}

		public override object GetBaseValue(object target)
		{
			return this.implementation.GetBaseValue(target);
		}

		public override object GetCurrentValue(object target)
		{
			return this.implementation.GetCurrentValue(target);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return this.implementation.GetCustomAttributes(attributeType, inherit);
		}

		public override object GetDefaultValue(Type targetType)
		{
			return this.implementation.GetDefaultValue(targetType);
		}

		public static DependencyPropertyReferenceStep GetReferenceStep(ITypeResolver typeResolver, Type targetType, System.Windows.DependencyProperty dependencyProperty)
		{
			return DependencyPropertyReferenceStep.GetReferenceStep(typeResolver, targetType, dependencyProperty, Microsoft.Expression.DesignModel.Metadata.MemberType.Property);
		}

		public static DependencyPropertyReferenceStep GetReferenceStep(ITypeResolver typeResolver, Type targetType, System.Windows.DependencyProperty dependencyProperty, Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes)
		{
			return DependencyPropertyReferenceStep.GetReferenceStep(typeResolver, targetType, dependencyProperty, dependencyProperty.OwnerType, dependencyProperty.Name, Microsoft.Expression.DesignModel.Metadata.MemberType.Property);
		}

		public static DependencyPropertyReferenceStep GetReferenceStep(ITypeResolver typeResolver, Type targetType, DependencyPropertyReferenceStep referenceStep)
		{
			return DependencyPropertyReferenceStep.GetReferenceStep(typeResolver, targetType, referenceStep, Microsoft.Expression.DesignModel.Metadata.MemberType.Property);
		}

		public static DependencyPropertyReferenceStep GetReferenceStep(ITypeResolver typeResolver, Type targetType, DependencyPropertyReferenceStep referenceStep, Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes)
		{
			return DependencyPropertyReferenceStep.GetReferenceStep(typeResolver, targetType, referenceStep.DependencyProperty, PlatformTypeHelper.GetDeclaringType(referenceStep), referenceStep.Name, memberTypes);
		}

		public static DependencyPropertyReferenceStep GetReferenceStep(ITypeResolver typeResolver, Type targetType, object dependencyProperty, Type declaringType, string propertyName, Microsoft.Expression.DesignModel.Metadata.MemberType memberTypes)
		{
			if (typeof(IDesignTimePropertyImplementor).IsAssignableFrom(declaringType))
			{
				IPropertyId propertyId = DesignTimeProperties.FromName(propertyName, (Microsoft.Expression.DesignModel.Metadata.PlatformTypes)typeResolver.PlatformMetadata, null);
				return propertyId as DependencyPropertyReferenceStep;
			}
			Microsoft.Expression.DesignModel.Metadata.PlatformTypes platformMetadata = (Microsoft.Expression.DesignModel.Metadata.PlatformTypes)typeResolver.PlatformMetadata;
			DependencyPropertyReferenceStep property = platformMetadata.GetProperty(typeResolver, targetType, memberTypes, propertyName) as DependencyPropertyReferenceStep;
			if (property != null && property.DependencyProperty == dependencyProperty)
			{
				return property;
			}
			if (!declaringType.IsAssignableFrom(targetType))
			{
				if (TypeHelper.IsSet(memberTypes, Microsoft.Expression.DesignModel.Metadata.MemberType.AttachedProperty))
				{
					DependencyPropertyReferenceStep dependencyPropertyReferenceStep = platformMetadata.GetProperty(typeResolver, declaringType, Microsoft.Expression.DesignModel.Metadata.MemberType.AttachedProperty, propertyName) as DependencyPropertyReferenceStep;
					property = dependencyPropertyReferenceStep;
					if (dependencyPropertyReferenceStep == null)
					{
						goto Label1;
					}
					if (property.TargetType.IsAssignableFrom(targetType))
					{
						return property;
					}
					else
					{
						return null;
					}
				}
			Label1:
				if (TypeHelper.IsSet(memberTypes, Microsoft.Expression.DesignModel.Metadata.MemberType.DependencyProperty))
				{
					DependencyPropertyReferenceStep property1 = platformMetadata.GetProperty(typeResolver, declaringType, Microsoft.Expression.DesignModel.Metadata.MemberType.DependencyProperty, propertyName) as DependencyPropertyReferenceStep;
					property = property1;
					if (property1 != null)
					{
						return property;
					}
				}
			}
			return null;
		}

		public override object GetValue(object target)
		{
			return this.implementation.GetValue(target);
		}

		public BaseValueSource GetValueSource(object target)
		{
			return this.implementation.GetValueSource(target);
		}

		public override bool HasDefaultValue(Type targetType)
		{
			return this.implementation.HasDefaultValue(targetType);
		}

		public bool Inherits(Type targetType)
		{
			return this.implementation.Inherits(targetType);
		}

		public override bool IsAnimated(object target)
		{
			return this.implementation.IsAnimated(target);
		}

		public bool IsAnimationProhibited(Type targetType)
		{
			return this.implementation.IsAnimationProhibited(targetType);
		}

		public bool IsDefaultValue(object instance)
		{
			return this.implementation.IsDefaultValue(instance);
		}

		public override bool IsSet(object target)
		{
			return this.implementation.IsSet(target);
		}

		IMember Microsoft.Expression.DesignModel.Metadata.IMember.Clone(ITypeResolver typeResolver)
		{
			if (this.PlatformTypes != typeResolver.PlatformMetadata && this.MemberType == Microsoft.Expression.DesignModel.Metadata.MemberType.DesignTimeProperty)
			{
				return DesignTimeProperties.Clone(this, typeResolver);
			}
			return base.Clone(typeResolver);
		}

		void Microsoft.Expression.DesignModel.Metadata.IPropertyImplementation.Invalidate()
		{
			this.implementation = UndefinedDependencyPropertyImplementation.Instance;
		}

		public void SetBinding(object target, object value)
		{
			this.implementation.SetBinding(target, value);
		}

		public void SetResourceReference(object target, object value)
		{
			this.implementation.SetResourceReference(target, value);
		}

		public override object SetValue(object target, object valueToSet)
		{
			this.implementation.SetValue(target, valueToSet);
			return target;
		}

		public sealed override bool ShouldSerializeMethod(object objToInspect)
		{
			LocalClrPropertyImplementation clrImplementation = this.implementation.ClrImplementation as LocalClrPropertyImplementation;
			if (clrImplementation == null || !(clrImplementation.ShouldSerializeMethod != null))
			{
				return true;
			}
			return (bool)clrImplementation.ShouldSerializeMethod.Invoke(objToInspect, new object[0]);
		}
	}
}