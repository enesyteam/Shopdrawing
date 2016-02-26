using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public sealed class UndefinedClrPropertyImplementation : ClrPropertyImplementationBase
	{
		public override Type DeclaringType
		{
			get
			{
				return null;
			}
		}

		public override object DefaultValue
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override bool HasDefault
		{
			get
			{
				return false;
			}
		}

		public override bool IsResolved
		{
			get
			{
				return false;
			}
		}

		public override string Name
		{
			get
			{
				return "[Undefined]";
			}
		}

		public override MemberAccessType ReadAccess
		{
			get
			{
				return MemberAccessType.Public;
			}
		}

		public override Type TargetType
		{
			get
			{
				return typeof(object);
			}
		}

		public override Type ValueType
		{
			get
			{
				return null;
			}
		}

		public override MemberAccessType WriteAccess
		{
			get
			{
				return MemberAccessType.Public;
			}
		}

		internal UndefinedClrPropertyImplementation(PlatformTypes platformTypes) : base(platformTypes)
		{
		}

		public override object GetValue(object target)
		{
			throw new InvalidOperationException();
		}
	}
}