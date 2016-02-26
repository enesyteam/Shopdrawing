using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal abstract class DependencyPropertyImplementation : DependencyPropertyImplementationBase
	{
		private ClrPropertyImplementationBase clrImplementation;

		public override ClrPropertyImplementationBase ClrImplementation
		{
			get
			{
				return this.clrImplementation;
			}
		}

		public override string ConstructorArgument
		{
			get
			{
				return this.clrImplementation.ConstructorArgument;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.clrImplementation.DeclaringType;
			}
		}

		public override bool IsAttachable
		{
			get
			{
				return this.clrImplementation.IsAttachable;
			}
		}

		public override bool IsResolved
		{
			get
			{
				return this.clrImplementation.IsResolved;
			}
		}

		public override Microsoft.Expression.DesignModel.Metadata.MemberType MemberType
		{
			get
			{
				return this.clrImplementation.MemberType;
			}
		}

		public override ITypeId MemberTypeId
		{
			get
			{
				return this.clrImplementation.MemberTypeId;
			}
		}

		public override bool ShouldSerialize
		{
			get
			{
				return this.clrImplementation.ShouldSerialize;
			}
		}

		public override Type TargetType
		{
			get
			{
				return this.clrImplementation.TargetType;
			}
		}

		public override System.ComponentModel.TypeConverter TypeConverter
		{
			get
			{
				return this.clrImplementation.TypeConverter;
			}
		}

		protected DependencyPropertyImplementation(ClrPropertyImplementationBase clrImplementation)
		{
			if (clrImplementation == null)
			{
				throw new ArgumentNullException("clrImplementation");
			}
			this.clrImplementation = clrImplementation;
		}

		public override bool HasDefaultValue(Type targetType)
		{
			return true;
		}
	}
}