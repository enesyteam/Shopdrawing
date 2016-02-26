using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyMethodBodyWorker : MetadataOnlyMethodBody
	{
		private readonly MetadataOnlyMethodBodyWorker.IMethodHeader m_header;

		public override IList<ExceptionHandlingClause> ExceptionHandlingClauses
		{
			get
			{
				int num;
				int num1;
				MetadataOnlyMethodBodyWorker.IEHClause eHClause;
				if ((int)(this.m_header.Flags & MetadataOnlyMethodBodyWorker.MethodHeaderFlags.MoreSects) == 0)
				{
					return new ExceptionHandlingClause[0];
				}
				MetadataOnlyModule resolver = base.Method.Resolver;
				uint methodRva = resolver.GetMethodRva(base.Method.MetadataToken);
				long headerSizeBytes = (long)((ulong)methodRva + (long)this.m_header.HeaderSizeBytes + (long)this.m_header.CodeSize);
				headerSizeBytes = (headerSizeBytes - (long)1 & (long)-4) + (long)4;
				MetadataOnlyMethodBodyWorker.CorILMethod_Sect corILMethodSect = (MetadataOnlyMethodBodyWorker.CorILMethod_Sect)resolver.RawMetadata.ReadRvaStruct<byte>(headerSizeBytes);
				if ((int)(corILMethodSect & (MetadataOnlyMethodBodyWorker.CorILMethod_Sect.OptILTable | MetadataOnlyMethodBodyWorker.CorILMethod_Sect.MoreSects)) != 0)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					string unsupportedExceptionFlags = MetadataStringTable.UnsupportedExceptionFlags;
					object[] str = new object[] { corILMethodSect.ToString() };
					throw new NotSupportedException(string.Format(invariantCulture, unsupportedExceptionFlags, str));
				}
				bool flag = (int)(corILMethodSect & MetadataOnlyMethodBodyWorker.CorILMethod_Sect.FatFormat) != 0;
				if (!flag)
				{
					num = resolver.RawMetadata.ReadRvaStruct<byte>(headerSizeBytes + (long)1);
					num1 = 12;
				}
				else
				{
					byte[] numArray = resolver.RawMetadata.ReadRva(headerSizeBytes + (long)1, 3);
					num = numArray[0] + numArray[1] * 256 + numArray[2] * 65536;
					num1 = 24;
				}
				int num2 = (num - 4) / num1;
				ExceptionHandlingClause[] exceptionHandlingClauseWorker = new MetadataOnlyMethodBodyWorker.ExceptionHandlingClauseWorker[num2];
				long num3 = headerSizeBytes + (long)4;
				for (int i = 0; i < num2; i++)
				{
					if (flag)
					{
						eHClause = resolver.RawMetadata.ReadRvaStruct<MetadataOnlyMethodBodyWorker.EHFat>(num3);
					}
					else
					{
						eHClause = resolver.RawMetadata.ReadRvaStruct<MetadataOnlyMethodBodyWorker.EHSmall>(num3);
					}
					MetadataOnlyMethodBodyWorker.IEHClause eHClause1 = eHClause;
					num3 = num3 + (long)num1;
					exceptionHandlingClauseWorker[i] = new MetadataOnlyMethodBodyWorker.ExceptionHandlingClauseWorker(base.Method, eHClause1);
				}
				return Array.AsReadOnly<ExceptionHandlingClause>(exceptionHandlingClauseWorker);
			}
		}

		public override bool InitLocals
		{
			get
			{
				return (int)(this.m_header.Flags & MetadataOnlyMethodBodyWorker.MethodHeaderFlags.InitLocals) != 0;
			}
		}

		public override int LocalSignatureMetadataToken
		{
			get
			{
				return this.m_header.LocalVarSigTok.Value;
			}
		}

		public override int MaxStackSize
		{
			get
			{
				return this.m_header.MaxStack;
			}
		}

		public MetadataOnlyMethodBodyWorker(MetadataOnlyMethodInfo method, MetadataOnlyMethodBodyWorker.IMethodHeader header) : base(method)
		{
			this.m_header = header;
		}

		internal static MethodBody Create(MetadataOnlyMethodInfo method)
		{
			MetadataOnlyModule resolver = method.Resolver;
			uint methodRva = resolver.GetMethodRva(method.MetadataToken);
			if (methodRva == 0)
			{
				return null;
			}
			return new MetadataOnlyMethodBodyWorker(method, MetadataOnlyMethodBodyWorker.GetMethodHeader(methodRva, resolver));
		}

		public override byte[] GetILAsByteArray()
		{
			MetadataOnlyModule resolver = base.Method.Resolver;
			uint methodRva = resolver.GetMethodRva(base.Method.MetadataToken);
			byte[] numArray = resolver.RawMetadata.ReadRva((long)((ulong)methodRva + (long)this.m_header.HeaderSizeBytes), this.m_header.CodeSize);
			return numArray;
		}

		public static MetadataOnlyMethodBodyWorker.IMethodHeader GetMethodHeader(uint rva, MetadataOnlyModule scope)
		{
			MetadataOnlyMethodBodyWorker.IMethodHeader tinyHeader;
			byte[] numArray = scope.RawMetadata.ReadRva((long)rva, 1);
			MetadataOnlyMethodBodyWorker.MethodHeaderFlags methodHeaderFlag = (MetadataOnlyMethodBodyWorker.MethodHeaderFlags)(numArray[0] & 3);
			if (methodHeaderFlag == MetadataOnlyMethodBodyWorker.MethodHeaderFlags.FatFormat)
			{
				tinyHeader = scope.RawMetadata.ReadRvaStruct<MetadataOnlyMethodBodyWorker.FatHeader>((long)rva);
				return tinyHeader;
			}
			if (methodHeaderFlag != MetadataOnlyMethodBodyWorker.MethodHeaderFlags.TinyFormat)
			{
				throw new InvalidOperationException(MetadataStringTable.InvalidMetadata);
			}
			tinyHeader = new MetadataOnlyMethodBodyWorker.TinyHeader(numArray[0]);
			return tinyHeader;
		}

		[Flags]
		private enum CorILMethod_Sect
		{
			EHTable = 1,
			OptILTable = 2,
			FatFormat = 64,
			MoreSects = 128
		}

		internal class EHFat : MetadataOnlyMethodBodyWorker.IEHClause
		{
			private readonly uint m_Flags;

			private readonly int m_TryOffset;

			private readonly int m_TryLength;

			private readonly int m_HandlerOffset;

			private readonly int m_HandlerLength;

			private readonly uint m_ClassToken;

			private readonly int m_FilterOffset;

			Token Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.ClassToken
			{
				get
				{
					return new Token(this.m_ClassToken);
				}
			}

			int Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.FilterOffset
			{
				get
				{
					return this.m_FilterOffset;
				}
			}

			ExceptionHandlingClauseOptions Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.Flags
			{
				get
				{
					return (ExceptionHandlingClauseOptions)this.m_Flags;
				}
			}

			int Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.HandlerLength
			{
				get
				{
					return this.m_HandlerLength;
				}
			}

			int Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.HandlerOffset
			{
				get
				{
					return this.m_HandlerOffset;
				}
			}

			int Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.TryLength
			{
				get
				{
					return this.m_TryLength;
				}
			}

			int Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.TryOffset
			{
				get
				{
					return this.m_TryOffset;
				}
			}

			public EHFat()
			{
			}
		}

		internal class EHSmall : MetadataOnlyMethodBodyWorker.IEHClause
		{
			private readonly ushort m_Flags;

			private readonly ushort m_TryOffset;

			private readonly byte m_TryLength;

			private readonly byte m_HandlerOffset1;

			private readonly byte m_HandlerOffset2;

			private readonly byte m_HandlerLength;

			private readonly uint m_ClassToken;

			private readonly int m_FilterOffset;

			Token Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.ClassToken
			{
				get
				{
					return new Token(this.m_ClassToken);
				}
			}

			int Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.FilterOffset
			{
				get
				{
					return this.m_FilterOffset;
				}
			}

			ExceptionHandlingClauseOptions Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.Flags
			{
				get
				{
					return (ExceptionHandlingClauseOptions)this.m_Flags;
				}
			}

			int Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.HandlerLength
			{
				get
				{
					return this.m_HandlerLength;
				}
			}

			int Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.HandlerOffset
			{
				get
				{
					return this.m_HandlerOffset2 * 256 + this.m_HandlerOffset1;
				}
			}

			int Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.TryLength
			{
				get
				{
					return this.m_TryLength;
				}
			}

			int Microsoft.MetadataReader.MetadataOnlyMethodBodyWorker.IEHClause.TryOffset
			{
				get
				{
					return this.m_TryOffset;
				}
			}

			public EHSmall()
			{
			}
		}

		private class ExceptionHandlingClauseWorker : ExceptionHandlingClause
		{
			private readonly MethodInfo m_method;

			private readonly MetadataOnlyMethodBodyWorker.IEHClause m_data;

			public override Type CatchType
			{
				get
				{
					Token classToken = this.m_data.ClassToken;
					Module module = this.m_method.Module;
					Type type = module.ResolveType(classToken, this.m_method.DeclaringType.GetGenericArguments(), this.m_method.GetGenericArguments());
					return type;
				}
			}

			public override int FilterOffset
			{
				get
				{
					return this.m_data.FilterOffset;
				}
			}

			public override ExceptionHandlingClauseOptions Flags
			{
				get
				{
					return this.m_data.Flags;
				}
			}

			public override int HandlerLength
			{
				get
				{
					return this.m_data.HandlerLength;
				}
			}

			public override int HandlerOffset
			{
				get
				{
					return this.m_data.HandlerOffset;
				}
			}

			public override int TryLength
			{
				get
				{
					return this.m_data.TryLength;
				}
			}

			public override int TryOffset
			{
				get
				{
					return this.m_data.TryOffset;
				}
			}

			public ExceptionHandlingClauseWorker(MethodInfo method, MetadataOnlyMethodBodyWorker.IEHClause data)
			{
				this.m_method = method;
				this.m_data = data;
			}
		}

		internal class FatHeader : MetadataOnlyMethodBodyWorker.IMethodHeader
		{
			private readonly short m_FlagsAndSize;

			private readonly short m_MaxStack;

			private readonly uint m_CodeSize;

			private readonly uint m_LocalVarSigTok;

			public int CodeSize
			{
				get
				{
					return (int)this.m_CodeSize;
				}
			}

			public MetadataOnlyMethodBodyWorker.MethodHeaderFlags Flags
			{
				get
				{
					return (MetadataOnlyMethodBodyWorker.MethodHeaderFlags)(this.m_FlagsAndSize & 4095);
				}
			}

			public int HeaderSizeBytes
			{
				get
				{
					return (this.m_FlagsAndSize >> 12 & 15) * 4;
				}
			}

			public Token LocalVarSigTok
			{
				get
				{
					return new Token(this.m_LocalVarSigTok);
				}
			}

			public int MaxStack
			{
				get
				{
					return this.m_MaxStack;
				}
			}

			public FatHeader()
			{
			}
		}

		private interface IEHClause
		{
			Token ClassToken
			{
				get;
			}

			int FilterOffset
			{
				get;
			}

			ExceptionHandlingClauseOptions Flags
			{
				get;
			}

			int HandlerLength
			{
				get;
			}

			int HandlerOffset
			{
				get;
			}

			int TryLength
			{
				get;
			}

			int TryOffset
			{
				get;
			}
		}

		internal interface IMethodHeader
		{
			int CodeSize
			{
				get;
			}

			MetadataOnlyMethodBodyWorker.MethodHeaderFlags Flags
			{
				get;
			}

			int HeaderSizeBytes
			{
				get;
			}

			Token LocalVarSigTok
			{
				get;
			}

			int MaxStack
			{
				get;
			}
		}

		[Flags]
		internal enum MethodHeaderFlags
		{
			TinyFormat = 2,
			FatFormat = 3,
			MoreSects = 8,
			InitLocals = 16
		}

		internal class TinyHeader : MetadataOnlyMethodBodyWorker.IMethodHeader
		{
			private readonly byte FlagsAndSize;

			public int CodeSize
			{
				get
				{
					return this.FlagsAndSize >> 2 & 63;
				}
			}

			public MetadataOnlyMethodBodyWorker.MethodHeaderFlags Flags
			{
				get
				{
					return (MetadataOnlyMethodBodyWorker.MethodHeaderFlags)(this.FlagsAndSize & 3);
				}
			}

			public int HeaderSizeBytes
			{
				get
				{
					return 1;
				}
			}

			public Token LocalVarSigTok
			{
				get
				{
					return Token.Nil;
				}
			}

			public int MaxStack
			{
				get
				{
					return 8;
				}
			}

			public TinyHeader()
			{
			}

			public TinyHeader(byte data)
			{
				this.FlagsAndSize = data;
			}
		}
	}
}