using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	[DebuggerDisplay("{m_typeSpecToken}")]
	internal class TypeSpec : TypeProxy, ITypeSpec, ITypeSignatureBlob, ITypeProxy
	{
		private readonly Token m_typeSpecToken;

		private readonly Microsoft.MetadataReader.GenericContext m_context;

		public byte[] Blob
		{
			get
			{
				EmbeddedBlobPointer embeddedBlobPointer;
				int num;
				this.m_resolver.RawImport.GetTypeSpecFromToken(this.m_typeSpecToken, out embeddedBlobPointer, out num);
				return this.m_resolver.ReadEmbeddedBlob(embeddedBlobPointer, num);
			}
		}

		public System.Reflection.Module DeclaringScope
		{
			get
			{
				return this.Resolver;
			}
		}

		public Token TypeSpecToken
		{
			get
			{
				return this.m_typeSpecToken;
			}
		}

		public TypeSpec(MetadataOnlyModule module, Token typeSpecToken, Type[] typeArgs, Type[] methodArgs) : base(module)
		{
			this.m_typeSpecToken = typeSpecToken;
			this.m_context = new Microsoft.MetadataReader.GenericContext(typeArgs, methodArgs);
		}

		protected override Type GetResolvedTypeWorker()
		{
			byte[] blob = this.Blob;
			int num = 0;
			Type type = SignatureUtil.ExtractType(blob, ref num, this.Resolver, this.m_context);
			return type;
		}
	}
}