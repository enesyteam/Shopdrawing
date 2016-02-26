using System;
using System.Reflection;

namespace Microsoft.MetadataReader
{
	internal class ProxyGenericType : TypeProxy
	{
		private readonly TypeProxy m_rawType;

		private readonly Type[] m_args;

		public override Type DeclaringType
		{
			get
			{
				return this.m_rawType.DeclaringType;
			}
		}

		public override bool IsEnum
		{
			get
			{
				return this.m_rawType.IsEnum;
			}
		}

		public override bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		public override bool IsGenericType
		{
			get
			{
				return true;
			}
		}

		public override System.Reflection.Module Module
		{
			get
			{
				return this.m_rawType.Module;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_rawType.Name;
			}
		}

		public override string Namespace
		{
			get
			{
				return this.m_rawType.Namespace;
			}
		}

		public ProxyGenericType(TypeProxy rawType, Type[] args) : base(rawType.Resolver)
		{
			this.m_rawType = rawType;
			this.m_args = args;
		}

		public override Type[] GetGenericArguments()
		{
			return (Type[])this.m_args.Clone();
		}

		public override Type GetGenericTypeDefinition()
		{
			return this.m_rawType;
		}

		protected override Type GetResolvedTypeWorker()
		{
			return this.m_rawType.GetResolvedType().MakeGenericType(this.m_args);
		}

		protected override bool IsArrayImpl()
		{
			return false;
		}

		protected override bool IsByRefImpl()
		{
			return false;
		}

		protected override bool IsPointerImpl()
		{
			return false;
		}

		protected override bool IsValueTypeImpl()
		{
			return this.m_rawType.IsValueType;
		}
	}
}