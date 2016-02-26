using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyMethodBody : MethodBody
	{
		private readonly MetadataOnlyMethodInfo m_method;

		public override IList<ExceptionHandlingClause> ExceptionHandlingClauses
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool InitLocals
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override int LocalSignatureMetadataToken
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override IList<LocalVariableInfo> LocalVariables
		{
			get
			{
				Token token = new Token(this.LocalSignatureMetadataToken);
				EmbeddedBlobPointer embeddedBlobPointer = new EmbeddedBlobPointer();
				int num = 0;
				if (!token.IsNil)
				{
					this.m_method.Resolver.RawImport.GetSigFromToken(token, out embeddedBlobPointer, out num);
				}
				if (num == 0)
				{
					return new MetadataOnlyLocalVariableInfo[0];
				}
				GenericContext genericContext = new GenericContext(this.m_method);
				byte[] numArray = this.m_method.Resolver.ReadEmbeddedBlob(embeddedBlobPointer, num);
				int num1 = 0;
				SignatureUtil.ExtractCallingConvention(numArray, ref num1);
				int num2 = SignatureUtil.ExtractInt(numArray, ref num1);
				MetadataOnlyLocalVariableInfo[] metadataOnlyLocalVariableInfo = new MetadataOnlyLocalVariableInfo[num2];
				for (int i = 0; i < num2; i++)
				{
					TypeSignatureDescriptor typeSignatureDescriptor = SignatureUtil.ExtractType(numArray, ref num1, this.m_method.Resolver, genericContext, true);
					metadataOnlyLocalVariableInfo[i] = new MetadataOnlyLocalVariableInfo(i, typeSignatureDescriptor.Type, typeSignatureDescriptor.IsPinned);
				}
				return metadataOnlyLocalVariableInfo;
			}
		}

		public override int MaxStackSize
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		protected MetadataOnlyMethodInfo Method
		{
			get
			{
				return this.m_method;
			}
		}

		protected MetadataOnlyMethodBody(MetadataOnlyMethodInfo method)
		{
			this.m_method = method;
		}

		public override byte[] GetILAsByteArray()
		{
			throw new InvalidOperationException();
		}

		internal static MethodBody TryCreate(MetadataOnlyMethodInfo method)
		{
			MetadataOnlyModule resolver = method.Resolver;
			MethodBody methodBody = null;
			if (resolver.Factory.TryCreateMethodBody(method, ref methodBody))
			{
				return methodBody;
			}
			return MetadataOnlyMethodBodyWorker.Create(method);
		}
	}
}