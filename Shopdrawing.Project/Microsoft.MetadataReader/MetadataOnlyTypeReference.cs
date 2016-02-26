using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Adds;
using System.Text;

namespace Microsoft.MetadataReader
{
	[DebuggerDisplay("\\{Name = {Name} FullName = {FullName} {m_typeRef}\\}")]
	internal class MetadataOnlyTypeReference : TypeProxy, ITypeReference, ITypeProxy
	{
		private Token m_typeRef;

		public override System.Reflection.Assembly Assembly
		{
			get
			{
				return new AssemblyRef(this.RequestedAssemblyName, base.TypeUniverse);
			}
		}

		public override string AssemblyQualifiedName
		{
			get
			{
				string str = this.RequestedAssemblyName.ToString();
				return System.Reflection.Assembly.CreateQualifiedName(str, this.FullName);
			}
		}

		public System.Reflection.Module DeclaringScope
		{
			get
			{
				return this.m_resolver;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				int num;
				int num1;
				int value = this.m_typeRef.Value;
				this.m_resolver.RawImport.GetTypeRefProps(value, out num, null, 0, out num1);
				Token token = new Token(num);
				if (!token.IsType(System.Reflection.Adds.TokenType.TypeRef))
				{
					return null;
				}
				return this.m_resolver.Factory.CreateTypeRef(this.m_resolver, token);
			}
		}

		public override string FullName
		{
			get
			{
				int num;
				int num1;
				StringBuilder stringBuilder;
				int value = this.m_typeRef.Value;
				string empty = string.Empty;
				string str = string.Empty;
				while (true)
				{
					this.m_resolver.RawImport.GetTypeRefProps(value, out num, null, 0, out num1);
					stringBuilder = new StringBuilder(num1);
					Token token = new Token(num);
					this.m_resolver.RawImport.GetTypeRefProps(value, out num, stringBuilder, stringBuilder.Capacity, out num1);
					if (!token.IsType(System.Reflection.Adds.TokenType.TypeRef))
					{
						break;
					}
					str = string.Concat("+", stringBuilder.ToString(), str);
					value = token.Value;
				}
				stringBuilder.Append(str);
				return stringBuilder.ToString();
			}
		}

		public override bool IsGenericParameter
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
				return Utility.GetTypeNameFromFullNameHelper(this.FullName, base.IsNested);
			}
		}

		public override string Namespace
		{
			get
			{
				if (this.DeclaringType != null)
				{
					return this.DeclaringType.Namespace;
				}
				return Utility.GetNamespaceHelper(this.FullName);
			}
		}

		public string RawName
		{
			get
			{
				int num;
				int num1;
				int value = this.m_typeRef.Value;
				this.m_resolver.RawImport.GetTypeRefProps(value, out num, null, 0, out num1);
				StringBuilder stringBuilder = new StringBuilder(num1);
				this.m_resolver.RawImport.GetTypeRefProps(value, out num, stringBuilder, stringBuilder.Capacity, out num1);
				return stringBuilder.ToString();
			}
		}

		private AssemblyName RequestedAssemblyName
		{
			get
			{
				Token resolutionScope = this.ResolutionScope;
				System.Reflection.Adds.TokenType tokenType = resolutionScope.TokenType;
				if (tokenType > System.Reflection.Adds.TokenType.TypeRef)
				{
					if (tokenType == System.Reflection.Adds.TokenType.ModuleRef)
					{
						return this.m_resolver.Assembly.GetName();
					}
					if (tokenType == System.Reflection.Adds.TokenType.AssemblyRef)
					{
						return this.m_resolver.GetAssemblyNameFromAssemblyRef(resolutionScope);
					}
				}
				else
				{
					if (tokenType == System.Reflection.Adds.TokenType.Module)
					{
						return this.m_resolver.Assembly.GetName();
					}
					if (tokenType == System.Reflection.Adds.TokenType.TypeRef)
					{
						return (new MetadataOnlyTypeReference(this.m_resolver, resolutionScope)).RequestedAssemblyName;
					}
				}
				throw new InvalidOperationException(MetadataStringTable.InvalidMetadata);
			}
		}

		public Token ResolutionScope
		{
			get
			{
				int num;
				int num1;
				this.m_resolver.RawImport.GetTypeRefProps(this.m_typeRef.Value, out num, null, 0, out num1);
				return new Token(num);
			}
		}

		public Token TypeRefToken
		{
			get
			{
				return this.m_typeRef;
			}
		}

		public MetadataOnlyTypeReference(MetadataOnlyModule resolver, Token typeRef) : base(resolver)
		{
			this.m_typeRef = typeRef;
		}

		protected override Type GetResolvedTypeWorker()
		{
			return this.m_resolver.ResolveTypeRef(this);
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
	}
}