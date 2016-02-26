using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyFieldInfo : FieldInfo, IFieldInfo2
	{
		private readonly MetadataOnlyModule m_resolver;

		private readonly int m_fieldDefToken;

		private readonly FieldAttributes m_attrib;

		private readonly int m_declaringClassToken;

		private readonly Type m_fieldType;

		private readonly GenericContext m_context;

		private readonly string m_name;

		private readonly CustomModifiers m_customModifiers;

		public override FieldAttributes Attributes
		{
			get
			{
				return this.m_attrib;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				Type genericType = this.m_resolver.GetGenericType(new Token(this.m_declaringClassToken), this.m_context);
				return genericType;
			}
		}

		public override RuntimeFieldHandle FieldHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override Type FieldType
		{
			get
			{
				return this.m_fieldType;
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Field;
			}
		}

		public override int MetadataToken
		{
			get
			{
				return this.m_fieldDefToken;
			}
		}

		public override System.Reflection.Module Module
		{
			get
			{
				return this.m_resolver;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public MetadataOnlyFieldInfo(MetadataOnlyModule resolver, Token fieldDefToken, Type[] typeArgs, Type[] methodArgs)
		{
			int num;
			EmbeddedBlobPointer embeddedBlobPointer;
			int num1;
			int num2;
			IntPtr intPtr;
			int num3;
			FieldAttributes fieldAttribute;
			this.m_resolver = resolver;
			this.m_fieldDefToken = fieldDefToken;
			if (typeArgs != null || methodArgs != null)
			{
				this.m_context = new GenericContext(typeArgs, methodArgs);
			}
			IMetadataImport rawImport = this.m_resolver.RawImport;
			StringBuilder stringBuilder = new StringBuilder(256);
			rawImport.GetFieldProps(this.m_fieldDefToken, out this.m_declaringClassToken, null, 0, out num, out fieldAttribute, out embeddedBlobPointer, out num1, out num2, out intPtr, out num3);
			this.m_attrib = fieldAttribute;
			stringBuilder.Capacity = num;
			rawImport.GetFieldProps(this.m_fieldDefToken, out this.m_declaringClassToken, stringBuilder, num, out num, out fieldAttribute, out embeddedBlobPointer, out num1, out num2, out intPtr, out num3);
			this.m_attrib = fieldAttribute;
			this.m_name = stringBuilder.ToString();
			byte[] numArray = this.m_resolver.ReadEmbeddedBlob(embeddedBlobPointer, num1);
			int num4 = 0;
			SignatureUtil.ExtractCallingConvention(numArray, ref num4);
			this.m_customModifiers = SignatureUtil.ExtractCustomModifiers(numArray, ref num4, this.m_resolver, this.m_context);
			if (this.m_resolver.RawImport.IsValidToken((uint)this.m_declaringClassToken))
			{
				Type type = this.m_resolver.ResolveType(this.m_declaringClassToken);
				if (type.IsGenericType && (this.m_context == null || this.m_context.TypeArgs == null || (int)this.m_context.TypeArgs.Length == 0))
				{
					if (this.m_context != null)
					{
						this.m_context = new GenericContext(type.GetGenericArguments(), this.m_context.MethodArgs);
					}
					else
					{
						this.m_context = new GenericContext(type.GetGenericArguments(), null);
					}
				}
			}
			this.m_fieldType = SignatureUtil.ExtractType(numArray, ref num4, this.m_resolver, this.m_context);
		}

		public override bool Equals(object obj)
		{
			MetadataOnlyFieldInfo metadataOnlyFieldInfo = obj as MetadataOnlyFieldInfo;
			if (metadataOnlyFieldInfo == null)
			{
				return false;
			}
			if (!metadataOnlyFieldInfo.m_resolver.Equals(this.m_resolver) || !metadataOnlyFieldInfo.m_fieldDefToken.Equals(this.m_fieldDefToken))
			{
				return false;
			}
			return this.DeclaringType.Equals(metadataOnlyFieldInfo.DeclaringType);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.m_resolver.GetCustomAttributeData(this.MetadataToken);
		}

		public override int GetHashCode()
		{
			int mFieldDefToken = this.m_fieldDefToken;
			return this.m_resolver.GetHashCode() * 32767 + mFieldDefToken.GetHashCode();
		}

		public override Type[] GetOptionalCustomModifiers()
		{
			if (this.m_customModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return this.m_customModifiers.OptionalCustomModifiers;
		}

		public override object GetRawConstantValue()
		{
			if (!base.IsLiteral)
			{
				throw new InvalidOperationException(MetadataStringTable.OperationValidOnLiteralFieldsOnly);
			}
			return this.ParseDefaultValue();
		}

		public override Type[] GetRequiredCustomModifiers()
		{
			if (this.m_customModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return this.m_customModifiers.RequiredCustomModifiers;
		}

		public virtual byte[] GetRvaField()
		{
			uint num;
			uint num1;
			if ((this.Attributes & FieldAttributes.HasFieldRVA) == FieldAttributes.PrivateScope)
			{
				throw new InvalidOperationException(MetadataStringTable.OperationValidOnRVAFieldsOnly);
			}
			StructLayoutAttribute structLayoutAttribute = this.FieldType.StructLayoutAttribute;
			if (structLayoutAttribute.Value == LayoutKind.Auto)
			{
				throw new InvalidOperationException(MetadataStringTable.OperationInvalidOnAutoLayoutFields);
			}
			this.m_resolver.RawImport.GetRVA(this.MetadataToken, out num, out num1);
			int size = structLayoutAttribute.Size;
			if (size == 0)
			{
				switch (Type.GetTypeCode(this.FieldType))
				{
					case TypeCode.Int32:
					{
						size = 4;
						break;
					}
					case TypeCode.Int64:
					{
						size = 8;
						break;
					}
				}
			}
			return this.m_resolver.RawMetadata.ReadRva((long)num, size);
		}

		public override object GetValue(object obj)
		{
			throw new NotSupportedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		private object ParseDefaultValue()
		{
			int num;
			EmbeddedBlobPointer embeddedBlobPointer;
			int num1;
			int num2;
			IntPtr intPtr;
			int num3;
			FieldAttributes fieldAttribute;
			int num4;
			IMetadataImport rawImport = this.m_resolver.RawImport;
			rawImport.GetFieldProps(this.m_fieldDefToken, out num4, null, 0, out num, out fieldAttribute, out embeddedBlobPointer, out num1, out num2, out intPtr, out num3);
			byte[] numArray = this.m_resolver.ReadEmbeddedBlob(embeddedBlobPointer, num1);
			int num5 = 0;
			SignatureUtil.ExtractCallingConvention(numArray, ref num5);
			System.Reflection.Adds.CorElementType corElementType = SignatureUtil.ExtractElementType(numArray, ref num5);
			if (corElementType == System.Reflection.Adds.CorElementType.ValueType)
			{
				SignatureUtil.ExtractToken(numArray, ref num5);
				corElementType = (System.Reflection.Adds.CorElementType)num2;
			}
			else if (corElementType == System.Reflection.Adds.CorElementType.GenericInstantiation)
			{
				SignatureUtil.ExtractType(numArray, ref num5, this.m_resolver, this.m_context);
				corElementType = (System.Reflection.Adds.CorElementType)num2;
			}
			switch (corElementType)
			{
				case System.Reflection.Adds.CorElementType.Bool:
				{
					if (Marshal.ReadByte(intPtr) == 0)
					{
						return false;
					}
					return true;
				}
				case System.Reflection.Adds.CorElementType.Char:
				{
					return (char)Marshal.ReadInt16(intPtr);
				}
				case System.Reflection.Adds.CorElementType.SByte:
				{
					return (sbyte)Marshal.ReadByte(intPtr);
				}
				case System.Reflection.Adds.CorElementType.Byte:
				{
					return Marshal.ReadByte(intPtr);
				}
				case System.Reflection.Adds.CorElementType.Short:
				{
					return Marshal.ReadInt16(intPtr);
				}
				case System.Reflection.Adds.CorElementType.UShort:
				{
					return (ushort)Marshal.ReadInt16(intPtr);
				}
				case System.Reflection.Adds.CorElementType.Int:
				{
					return Marshal.ReadInt32(intPtr);
				}
				case System.Reflection.Adds.CorElementType.UInt:
				{
					return (uint)Marshal.ReadInt32(intPtr);
				}
				case System.Reflection.Adds.CorElementType.Long:
				{
					return Marshal.ReadInt64(intPtr);
				}
				case System.Reflection.Adds.CorElementType.ULong:
				{
					return (ulong)Marshal.ReadInt64(intPtr);
				}
				case System.Reflection.Adds.CorElementType.Float:
				{
					float[] singleArray = new float[1];
					Marshal.Copy(intPtr, singleArray, 0, 1);
					return singleArray[0];
				}
				case System.Reflection.Adds.CorElementType.Double:
				{
					double[] numArray1 = new double[1];
					Marshal.Copy(intPtr, numArray1, 0, 1);
					return numArray1[0];
				}
				case System.Reflection.Adds.CorElementType.String:
				{
					return Marshal.PtrToStringAuto(intPtr, num3);
				}
				case System.Reflection.Adds.CorElementType.Pointer:
				case System.Reflection.Adds.CorElementType.Byref:
				case System.Reflection.Adds.CorElementType.ValueType:
				case System.Reflection.Adds.CorElementType.TypeVar:
				case System.Reflection.Adds.CorElementType.Array:
				case System.Reflection.Adds.CorElementType.GenericInstantiation:
				case System.Reflection.Adds.CorElementType.TypedByRef:
				case System.Reflection.Adds.CorElementType.Array | System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Short | System.Reflection.Adds.CorElementType.TypedByRef | System.Reflection.Adds.CorElementType.Byte | System.Reflection.Adds.CorElementType.UShort | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.TypeVar | System.Reflection.Adds.CorElementType.GenericInstantiation:
				case System.Reflection.Adds.CorElementType.UIntPtr:
				{
					throw new InvalidOperationException(MetadataStringTable.IncorrectElementTypeValue);
				}
				case System.Reflection.Adds.CorElementType.Class:
				{
					return null;
				}
				case System.Reflection.Adds.CorElementType.IntPtr:
				{
					return Marshal.ReadIntPtr(intPtr);
				}
				default:
				{
					throw new InvalidOperationException(MetadataStringTable.IncorrectElementTypeValue);
				}
			}
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			return string.Concat(MetadataOnlyCommonType.TypeSigToString(this.FieldType), " ", this.Name);
		}
	}
}