using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public sealed class FieldReferenceStep : ReferenceStep, ICachedMemberInfo
	{
		private FieldReferenceStep.FieldImplementationBase implementation;

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

		public System.Reflection.FieldInfo FieldInfo
		{
			get
			{
				return this.implementation.FieldInfo;
			}
		}

		public override bool IsAttachable
		{
			get
			{
				return false;
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
				return Microsoft.Expression.DesignModel.Metadata.MemberType.Field;
			}
		}

		public override ITypeId MemberTypeId
		{
			get
			{
				return PlatformTypes.FieldInfo;
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
				IType declaringType = base.DeclaringType;
				if (declaringType == null)
				{
					return null;
				}
				return declaringType.RuntimeType;
			}
		}

		private FieldReferenceStep(ITypeResolver typeResolver, IType declaringType, System.Reflection.FieldInfo fieldInfo) : base(declaringType, fieldInfo.Name, PlatformTypeHelper.GetFieldTypeId(typeResolver, fieldInfo), PropertySortValue.NoValue)
		{
			this.implementation = new FieldReferenceStep.FieldImplementation(this, fieldInfo);
		}

		public override void ClearValue(object target)
		{
			throw new InvalidOperationException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return this.implementation.GetCustomAttributes(attributeType, inherit);
		}

		public override object GetDefaultValue(Type targetType)
		{
			return this.implementation.GetDefaultValue(targetType);
		}

		public static FieldReferenceStep GetReferenceStep(ITypeResolver typeResolver, System.Reflection.FieldInfo fieldInfo)
		{
			FieldReferenceStep fieldReferenceStep;
			IType type = typeResolver.GetType(fieldInfo.DeclaringType);
			IMutableMembers mutableMember = (IMutableMembers)type;
			IMemberId member = mutableMember.GetMember(Microsoft.Expression.DesignModel.Metadata.MemberType.Field, fieldInfo.Name);
			if (member == null)
			{
				fieldReferenceStep = new FieldReferenceStep(typeResolver, type, fieldInfo);
				mutableMember.AddMember(fieldReferenceStep);
			}
			else
			{
				fieldReferenceStep = Member.GetMemberAs<FieldReferenceStep>(member);
			}
			return fieldReferenceStep;
		}

		public override object GetValue(object target)
		{
			return this.implementation.GetValue(target);
		}

		public override bool HasDefaultValue(Type targetType)
		{
			return this.implementation.HasDefaultValue(targetType);
		}

		public override bool IsAnimated(object target)
		{
			return false;
		}

		public override bool IsSet(object target)
		{
			return true;
		}

		bool Microsoft.Expression.DesignModel.Metadata.ICachedMemberInfo.Refresh()
		{
			System.Reflection.FieldInfo fieldInfo = PlatformTypeHelper.GetFieldInfo(base.DeclaringType.NearestResolvedType.RuntimeType, this.Name);
			if (fieldInfo == null)
			{
				this.implementation = FieldReferenceStep.UndefinedFieldImplementation.Instance;
			}
			else
			{
				this.implementation = new FieldReferenceStep.FieldImplementation(this, fieldInfo);
			}
			return this.implementation.IsResolved;
		}

		public override object SetValue(object target, object valueToSet)
		{
			this.implementation.SetValue(target, valueToSet);
			return target;
		}

		public override bool ShouldSerializeMethod(object objToInspect)
		{
			return true;
		}

		private sealed class FieldImplementation : FieldReferenceStep.FieldImplementationBase
		{
			private FieldReferenceStep referenceStep;

			private System.Reflection.FieldInfo fieldInfo;

			private bool isResolved;

			private DelayedInstance<DesignerSerializationVisibility> serializationVisibility;

			private DelayedInstance<KeyValuePair<bool, object>> defaultValue;

			public override MemberAccessType Access
			{
				get
				{
					return PlatformTypeHelper.GetMemberAccess(this.fieldInfo);
				}
			}

			public override AttributeCollection Attributes
			{
				get
				{
					return new AttributeCollection(this.PlatformTypes.GetCustomAttributes(this.fieldInfo));
				}
			}

			public override System.Reflection.FieldInfo FieldInfo
			{
				get
				{
					return this.fieldInfo;
				}
			}

			public override bool IsResolved
			{
				get
				{
					return this.isResolved;
				}
			}

			private PlatformTypes PlatformTypes
			{
				get
				{
					return (PlatformTypes)this.referenceStep.DeclaringType.PlatformMetadata;
				}
			}

			public override bool ShouldSerialize
			{
				get
				{
					return this.serializationVisibility.Value != DesignerSerializationVisibility.Hidden;
				}
			}

			public FieldImplementation(FieldReferenceStep referenceStep, System.Reflection.FieldInfo fieldInfo)
			{
				this.referenceStep = referenceStep;
				this.fieldInfo = fieldInfo;
				this.isResolved = PlatformTypeHelper.GetFieldType(this.fieldInfo) != null;
				this.serializationVisibility = new DelayedInstance<DesignerSerializationVisibility>(() => PlatformTypeHelper.GetSerializationVisibility(this.PlatformTypes, this.fieldInfo));
				this.defaultValue = new DelayedInstance<KeyValuePair<bool, object>>(() => {
					Type fieldType = PlatformTypeHelper.GetFieldType(this.fieldInfo);
					if (fieldType == null)
					{
						return new KeyValuePair<bool, object>(false, null);
					}
					return PlatformTypeHelper.GetDefaultValue(this.PlatformTypes, this.fieldInfo, fieldType);
				});
			}

			public override object[] GetCustomAttributes(Type attributeType, bool inherits)
			{
				return this.PlatformTypes.GetCustomAttributes(this.fieldInfo, attributeType, inherits);
			}

			public override object GetDefaultValue(Type targetType)
			{
				return this.defaultValue.Value.Value;
			}

			public override object GetValue(object target)
			{
				return this.fieldInfo.GetValue(target);
			}

			public override bool HasDefaultValue(Type targetType)
			{
				return this.defaultValue.Value.Key;
			}

			public override void SetValue(object target, object valueToSet)
			{
				this.fieldInfo.SetValue(target, valueToSet);
			}
		}

		private abstract class FieldImplementationBase
		{
			public abstract MemberAccessType Access
			{
				get;
			}

			public abstract AttributeCollection Attributes
			{
				get;
			}

			public abstract System.Reflection.FieldInfo FieldInfo
			{
				get;
			}

			public abstract bool IsResolved
			{
				get;
			}

			public abstract bool ShouldSerialize
			{
				get;
			}

			protected FieldImplementationBase()
			{
			}

			public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);

			public abstract object GetDefaultValue(Type targetType);

			public abstract object GetValue(object target);

			public abstract bool HasDefaultValue(Type targetType);

			public abstract void SetValue(object target, object valueToSet);
		}

		private sealed class UndefinedFieldImplementation : FieldReferenceStep.FieldImplementationBase
		{
			public readonly static FieldReferenceStep.UndefinedFieldImplementation Instance;

			public override MemberAccessType Access
			{
				get
				{
					return MemberAccessType.Public;
				}
			}

			public override AttributeCollection Attributes
			{
				get
				{
					return null;
				}
			}

			public override System.Reflection.FieldInfo FieldInfo
			{
				get
				{
					return null;
				}
			}

			public override bool IsResolved
			{
				get
				{
					return false;
				}
			}

			public override bool ShouldSerialize
			{
				get
				{
					return true;
				}
			}

			static UndefinedFieldImplementation()
			{
				FieldReferenceStep.UndefinedFieldImplementation.Instance = new FieldReferenceStep.UndefinedFieldImplementation();
			}

			private UndefinedFieldImplementation()
			{
			}

			public override object[] GetCustomAttributes(Type attributeType, bool inherits)
			{
				return null;
			}

			public override object GetDefaultValue(Type targetType)
			{
				throw new InvalidOperationException();
			}

			public override object GetValue(object target)
			{
				throw new InvalidOperationException();
			}

			public override bool HasDefaultValue(Type targetType)
			{
				throw new InvalidOperationException();
			}

			public override void SetValue(object target, object valueToSet)
			{
			}
		}
	}
}