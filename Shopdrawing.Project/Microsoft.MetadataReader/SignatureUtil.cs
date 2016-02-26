using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal static class SignatureUtil
	{
		private const byte FieldId = 83;

		private const byte PropertyId = 84;

		private const System.Reflection.Adds.CorElementType BoxedValue = System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Modifier | System.Reflection.Adds.CorElementType.Sentinel | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.Type;

		private readonly static uint[] s_tkCorEncodeToken;

		static SignatureUtil()
		{
			SignatureUtil.s_tkCorEncodeToken = new uint[] { 33554432, 16777216, 452984832, 1912602624 };
		}

		internal static Microsoft.MetadataReader.CorCallingConvention ExtractCallingConvention(byte[] sig, ref int index)
		{
			return (Microsoft.MetadataReader.CorCallingConvention)SignatureUtil.ExtractInt(sig, ref index);
		}

		internal static void ExtractCustomAttributeArgumentType(ITypeUniverse universe, Module module, byte[] customAttributeBlob, ref int index, out System.Reflection.Adds.CorElementType argumentTypeId, out Type argumentType)
		{
			argumentTypeId = SignatureUtil.ExtractElementType(customAttributeBlob, ref index);
			SignatureUtil.VerifyElementType((System.Reflection.Adds.CorElementType)((int)argumentTypeId));
			if ((int)argumentTypeId == 29)
			{
				System.Reflection.Adds.CorElementType corElementType = SignatureUtil.ExtractElementType(customAttributeBlob, ref index);
				SignatureUtil.VerifyElementType(corElementType);
				if (corElementType == (System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Modifier | System.Reflection.Adds.CorElementType.Sentinel | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.Type))
				{
					argumentType = universe.GetBuiltInType(System.Reflection.Adds.CorElementType.Object).MakeArrayType();
					return;
				}
				argumentType = universe.GetBuiltInType(corElementType).MakeArrayType();
				return;
			}
			if ((int)argumentTypeId != 85)
			{
				if ((int)argumentTypeId == 81)
				{
					argumentType = null;
					return;
				}
				argumentType = universe.GetBuiltInType((System.Reflection.Adds.CorElementType)((int)argumentTypeId));
			}
			else
			{
				argumentType = SignatureUtil.ExtractTypeValue(universe, module, customAttributeBlob, ref index);
				if (argumentType == null)
				{
					throw new ArgumentException(MetadataStringTable.InvalidCustomAttributeFormatForEnum);
				}
			}
		}

		internal static CustomModifiers ExtractCustomModifiers(byte[] sig, ref int index, MetadataOnlyModule resolver, GenericContext context)
		{
			int num = index;
			System.Reflection.Adds.CorElementType corElementType = SignatureUtil.ExtractElementType(sig, ref index);
			List<Type> types = null;
			List<Type> types1 = null;
			if (corElementType != System.Reflection.Adds.CorElementType.CModOpt && corElementType != System.Reflection.Adds.CorElementType.CModReqd)
			{
				index = num;
				return null;
			}
			types = new List<Type>();
			types1 = new List<Type>();
			while (corElementType == System.Reflection.Adds.CorElementType.CModOpt || corElementType == System.Reflection.Adds.CorElementType.CModReqd)
			{
				Token token = SignatureUtil.ExtractToken(sig, ref index);
				Type type = resolver.ResolveTypeTokenInternal(token, context);
				if (corElementType != System.Reflection.Adds.CorElementType.CModOpt)
				{
					types1.Add(type);
				}
				else
				{
					types.Add(type);
				}
				num = index;
				corElementType = SignatureUtil.ExtractElementType(sig, ref index);
			}
			index = num;
			return new CustomModifiers(types, types1);
		}

		internal static System.Reflection.Adds.CorElementType ExtractElementType(byte[] sig, ref int index)
		{
			return (System.Reflection.Adds.CorElementType)SignatureUtil.ExtractInt(sig, ref index);
		}

		internal static int ExtractInt(byte[] sig, ref int index)
		{
			int num;
			if ((sig[index] & 128) == 0)
			{
				num = sig[index];
				index = index + 1;
			}
			else if ((sig[index] & 192) != 128)
			{
				if ((sig[index] & 224) != 192)
				{
					throw new ArgumentException(MetadataStringTable.InvalidMetadataSignature);
				}
				num = (sig[index] & 31) << 24 | sig[index + 1] << 16 | sig[index + 2] << 8 | sig[index + 3];
				index = index + 4;
			}
			else
			{
				num = (sig[index] & 63) << 8 | sig[index + 1];
				index = index + 2;
			}
			return num;
		}

		internal static IList<CustomAttributeTypedArgument> ExtractListOfValues(Type elementType, ITypeUniverse universe, Module module, uint size, byte[] blob, ref int index)
		{
			System.Reflection.Adds.CorElementType typeId = SignatureUtil.GetTypeId(elementType);
			List<CustomAttributeTypedArgument> customAttributeTypedArguments = new List<CustomAttributeTypedArgument>((int)size);
			if (typeId == System.Reflection.Adds.CorElementType.Object)
			{
				for (int i = 0; (long)i < (ulong)size; i++)
				{
					System.Reflection.Adds.CorElementType corElementType = SignatureUtil.ExtractElementType(blob, ref index);
					SignatureUtil.VerifyElementType(corElementType);
					Type builtInType = null;
					object obj = null;
					if (corElementType == System.Reflection.Adds.CorElementType.SzArray)
					{
						throw new NotImplementedException(MetadataStringTable.ArrayInsideArrayInAttributeNotSupported);
					}
					if (corElementType != System.Reflection.Adds.CorElementType.Enum)
					{
						builtInType = universe.GetBuiltInType(corElementType);
						obj = SignatureUtil.ExtractValue(corElementType, blob, ref index);
					}
					else
					{
						builtInType = SignatureUtil.ExtractTypeValue(universe, module, blob, ref index);
						if (builtInType == null)
						{
							throw new ArgumentException(MetadataStringTable.InvalidCustomAttributeFormatForEnum);
						}
						System.Reflection.Adds.CorElementType typeId1 = SignatureUtil.GetTypeId(MetadataOnlyModule.GetUnderlyingType(builtInType));
						obj = SignatureUtil.ExtractValue(typeId1, blob, ref index);
					}
					customAttributeTypedArguments.Add(new CustomAttributeTypedArgument(builtInType, obj));
				}
			}
			else if (typeId != System.Reflection.Adds.CorElementType.Type)
			{
				if (typeId == System.Reflection.Adds.CorElementType.SzArray)
				{
					throw new ArgumentException(MetadataStringTable.JaggedArrayInAttributeNotSupported);
				}
				for (int j = 0; (long)j < (ulong)size; j++)
				{
					object obj1 = SignatureUtil.ExtractValue(typeId, blob, ref index);
					customAttributeTypedArguments.Add(new CustomAttributeTypedArgument(elementType, obj1));
				}
			}
			else
			{
				for (int k = 0; (long)k < (ulong)size; k++)
				{
					object obj2 = SignatureUtil.ExtractTypeValue(universe, module, blob, ref index);
					customAttributeTypedArguments.Add(new CustomAttributeTypedArgument(elementType, obj2));
				}
			}
			return customAttributeTypedArguments.AsReadOnly();
		}

		internal static MethodSignatureDescriptor ExtractMethodSignature(SignatureBlob methodSignatureBlob, MetadataOnlyModule resolver, GenericContext context)
		{
			byte[] signatureAsByteArray = methodSignatureBlob.GetSignatureAsByteArray();
			int num = 0;
			MethodSignatureDescriptor methodSignatureDescriptor = new MethodSignatureDescriptor()
			{
				ReturnParameter = new TypeSignatureDescriptor(),
				GenericParameterCount = 0,
				CallingConvention = SignatureUtil.ExtractCallingConvention(signatureAsByteArray, ref num)
			};
			bool callingConvention = (methodSignatureDescriptor.CallingConvention & Microsoft.MetadataReader.CorCallingConvention.ExplicitThis) != Microsoft.MetadataReader.CorCallingConvention.Default;
			if ((methodSignatureDescriptor.CallingConvention & Microsoft.MetadataReader.CorCallingConvention.Generic) != Microsoft.MetadataReader.CorCallingConvention.Default)
			{
				int num1 = SignatureUtil.ExtractInt(signatureAsByteArray, ref num);
				if (num1 <= 0)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					object[] invalidMetadataSignature = new object[] { MetadataStringTable.InvalidMetadataSignature, MetadataStringTable.ExpectedPositiveNumberOfGenericParameters };
					throw new ArgumentException(string.Format(invariantCulture, "{0} {1}", invalidMetadataSignature));
				}
				context = context.VerifyAndUpdateMethodArguments(num1);
				methodSignatureDescriptor.GenericParameterCount = num1;
			}
			int num2 = SignatureUtil.ExtractInt(signatureAsByteArray, ref num);
			bool flag = false;
			CustomModifiers customModifier = SignatureUtil.ExtractCustomModifiers(signatureAsByteArray, ref num, resolver, context);
			methodSignatureDescriptor.ReturnParameter = SignatureUtil.ExtractType(signatureAsByteArray, ref num, resolver, context, flag);
			methodSignatureDescriptor.ReturnParameter.CustomModifiers = customModifier;
			if (callingConvention)
			{
				SignatureUtil.ExtractType(signatureAsByteArray, ref num, resolver, context);
				num2--;
			}
			methodSignatureDescriptor.Parameters = new TypeSignatureDescriptor[num2];
			for (int i = 0; i < num2; i++)
			{
				customModifier = SignatureUtil.ExtractCustomModifiers(signatureAsByteArray, ref num, resolver, context);
				methodSignatureDescriptor.Parameters[i] = SignatureUtil.ExtractType(signatureAsByteArray, ref num, resolver, context, flag);
				methodSignatureDescriptor.Parameters[i].CustomModifiers = customModifier;
			}
			if (num != (int)signatureAsByteArray.Length)
			{
				CultureInfo cultureInfo = CultureInfo.InvariantCulture;
				object[] objArray = new object[] { MetadataStringTable.InvalidMetadataSignature, MetadataStringTable.ExtraInformationAfterLastParameter };
				throw new ArgumentException(string.Format(cultureInfo, "{0} {1}", objArray));
			}
			return methodSignatureDescriptor;
		}

		internal static NamedArgumentType ExtractNamedArgumentType(byte[] customAttributeBlob, ref int index)
		{
			byte num = (byte)SignatureUtil.ExtractValue(System.Reflection.Adds.CorElementType.Byte, customAttributeBlob, ref index);
			if (num == 84)
			{
				return NamedArgumentType.Property;
			}
			if (num != 83)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				object[] invalidCustomAttributeFormat = new object[] { MetadataStringTable.InvalidCustomAttributeFormat, MetadataStringTable.ExpectedPropertyOrFieldId };
				throw new ArgumentException(string.Format(invariantCulture, "{0} {1}", invalidCustomAttributeFormat));
			}
			return NamedArgumentType.Field;
		}

		internal static string ExtractStringValue(byte[] blob, ref int index)
		{
			return (string)SignatureUtil.ExtractValue(System.Reflection.Adds.CorElementType.String, blob, ref index);
		}

		internal static Token ExtractToken(byte[] sig, ref int index)
		{
			unsafe
			{
				uint num = (uint)SignatureUtil.ExtractInt(sig, ref index);
				uint sTkCorEncodeToken = SignatureUtil.s_tkCorEncodeToken[num & 3];
				return new Token(SignatureUtil.TokenFromRid(num >> 2, sTkCorEncodeToken));
			}
		}

		internal static Type ExtractType(byte[] sig, ref int index, MetadataOnlyModule resolver, GenericContext context)
		{
			TypeSignatureDescriptor typeSignatureDescriptor = SignatureUtil.ExtractType(sig, ref index, resolver, context, false);
			return typeSignatureDescriptor.Type;
		}

		internal static TypeSignatureDescriptor ExtractType(byte[] sig, ref int index, MetadataOnlyModule resolver, GenericContext context, bool fAllowPinned)
		{
			TypeSignatureDescriptor typeSignatureDescriptor = new TypeSignatureDescriptor()
			{
				IsPinned = false
			};
			System.Reflection.Adds.CorElementType corElementType = SignatureUtil.ExtractElementType(sig, ref index);
			switch (corElementType)
			{
				case System.Reflection.Adds.CorElementType.End:
				case System.Reflection.Adds.CorElementType.Array | System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Short | System.Reflection.Adds.CorElementType.TypedByRef | System.Reflection.Adds.CorElementType.Byte | System.Reflection.Adds.CorElementType.UShort | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.TypeVar | System.Reflection.Adds.CorElementType.GenericInstantiation:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.IntPtr | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Long:
				case System.Reflection.Adds.CorElementType.Internal:
				case System.Reflection.Adds.CorElementType.Max:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Max | System.Reflection.Adds.CorElementType.Void:
				case System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte:
				case System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Byte | System.Reflection.Adds.CorElementType.Void:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Short | System.Reflection.Adds.CorElementType.Max:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Short | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Max | System.Reflection.Adds.CorElementType.Byte | System.Reflection.Adds.CorElementType.UShort | System.Reflection.Adds.CorElementType.Void:
				case System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.Int:
				case System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.UInt | System.Reflection.Adds.CorElementType.Void:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Long | System.Reflection.Adds.CorElementType.Max:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Long | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Max | System.Reflection.Adds.CorElementType.UInt | System.Reflection.Adds.CorElementType.ULong | System.Reflection.Adds.CorElementType.Void:
				case System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Float:
				case System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Float | System.Reflection.Adds.CorElementType.Double | System.Reflection.Adds.CorElementType.Byte | System.Reflection.Adds.CorElementType.UInt | System.Reflection.Adds.CorElementType.Void:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Short | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Long | System.Reflection.Adds.CorElementType.Max | System.Reflection.Adds.CorElementType.Float | System.Reflection.Adds.CorElementType.String:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Short | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Long | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Max | System.Reflection.Adds.CorElementType.Pointer | System.Reflection.Adds.CorElementType.Float | System.Reflection.Adds.CorElementType.Double | System.Reflection.Adds.CorElementType.String | System.Reflection.Adds.CorElementType.Byte | System.Reflection.Adds.CorElementType.UShort | System.Reflection.Adds.CorElementType.UInt | System.Reflection.Adds.CorElementType.ULong | System.Reflection.Adds.CorElementType.Void:
				case System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.CModOpt:
				case System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.Max:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Max | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.TypeVar:
				case System.Reflection.Adds.CorElementType.Array | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte:
				case System.Reflection.Adds.CorElementType.Array | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Byte | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.GenericInstantiation:
				case System.Reflection.Adds.CorElementType.Array | System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Short | System.Reflection.Adds.CorElementType.Max | System.Reflection.Adds.CorElementType.TypedByRef:
				case System.Reflection.Adds.CorElementType.Array | System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Short | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Max | System.Reflection.Adds.CorElementType.TypedByRef | System.Reflection.Adds.CorElementType.Byte | System.Reflection.Adds.CorElementType.UShort | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.TypeVar | System.Reflection.Adds.CorElementType.GenericInstantiation:
				case System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.IntPtr | System.Reflection.Adds.CorElementType.Int:
				case System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.IntPtr | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.UIntPtr | System.Reflection.Adds.CorElementType.UInt | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.IntPtr | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Long | System.Reflection.Adds.CorElementType.Max:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.FnPtr | System.Reflection.Adds.CorElementType.IntPtr | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Long | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Max | System.Reflection.Adds.CorElementType.UIntPtr | System.Reflection.Adds.CorElementType.UInt | System.Reflection.Adds.CorElementType.ULong | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.TypeVar:
				case System.Reflection.Adds.CorElementType.Array | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.IntPtr | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Object | System.Reflection.Adds.CorElementType.Float:
				case System.Reflection.Adds.CorElementType.Array | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.IntPtr | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Object | System.Reflection.Adds.CorElementType.Float | System.Reflection.Adds.CorElementType.Double | System.Reflection.Adds.CorElementType.SzArray | System.Reflection.Adds.CorElementType.UIntPtr | System.Reflection.Adds.CorElementType.Byte | System.Reflection.Adds.CorElementType.UInt | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.GenericInstantiation:
				case System.Reflection.Adds.CorElementType.Array | System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.IntPtr | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Short | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Long | System.Reflection.Adds.CorElementType.Max | System.Reflection.Adds.CorElementType.Object | System.Reflection.Adds.CorElementType.Float | System.Reflection.Adds.CorElementType.String | System.Reflection.Adds.CorElementType.TypedByRef | System.Reflection.Adds.CorElementType.MethodVar:
				case System.Reflection.Adds.CorElementType.Array | System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.CModOpt | System.Reflection.Adds.CorElementType.CModReqd | System.Reflection.Adds.CorElementType.FnPtr | System.Reflection.Adds.CorElementType.IntPtr | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Short | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Long | System.Reflection.Adds.CorElementType.Internal | System.Reflection.Adds.CorElementType.Max | System.Reflection.Adds.CorElementType.Object | System.Reflection.Adds.CorElementType.Pointer | System.Reflection.Adds.CorElementType.Float | System.Reflection.Adds.CorElementType.Double | System.Reflection.Adds.CorElementType.String | System.Reflection.Adds.CorElementType.SzArray | System.Reflection.Adds.CorElementType.TypedByRef | System.Reflection.Adds.CorElementType.UIntPtr | System.Reflection.Adds.CorElementType.Byte | System.Reflection.Adds.CorElementType.UShort | System.Reflection.Adds.CorElementType.UInt | System.Reflection.Adds.CorElementType.ULong | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.TypeVar | System.Reflection.Adds.CorElementType.MethodVar | System.Reflection.Adds.CorElementType.GenericInstantiation:
				case System.Reflection.Adds.CorElementType.Modifier:
				case System.Reflection.Adds.CorElementType.Sentinel:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Modifier:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.Modifier | System.Reflection.Adds.CorElementType.Sentinel | System.Reflection.Adds.CorElementType.Void:
				case System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Modifier:
				{
					throw new ArgumentException(MetadataStringTable.IncorrectElementTypeValue);
				}
				case System.Reflection.Adds.CorElementType.Void:
				case System.Reflection.Adds.CorElementType.Bool:
				case System.Reflection.Adds.CorElementType.Char:
				case System.Reflection.Adds.CorElementType.SByte:
				case System.Reflection.Adds.CorElementType.Byte:
				case System.Reflection.Adds.CorElementType.Short:
				case System.Reflection.Adds.CorElementType.UShort:
				case System.Reflection.Adds.CorElementType.Int:
				case System.Reflection.Adds.CorElementType.UInt:
				case System.Reflection.Adds.CorElementType.Long:
				case System.Reflection.Adds.CorElementType.ULong:
				case System.Reflection.Adds.CorElementType.Float:
				case System.Reflection.Adds.CorElementType.Double:
				case System.Reflection.Adds.CorElementType.String:
				case System.Reflection.Adds.CorElementType.IntPtr:
				case System.Reflection.Adds.CorElementType.UIntPtr:
				case System.Reflection.Adds.CorElementType.Object:
				{
					typeSignatureDescriptor.Type = resolver.AssemblyResolver.GetBuiltInType(corElementType);
					break;
				}
				case System.Reflection.Adds.CorElementType.Pointer:
				{
					typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context).MakePointerType();
					break;
				}
				case System.Reflection.Adds.CorElementType.Byref:
				{
					typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context).MakeByRefType();
					break;
				}
				case System.Reflection.Adds.CorElementType.ValueType:
				case System.Reflection.Adds.CorElementType.Class:
				{
					Token token = SignatureUtil.ExtractToken(sig, ref index);
					typeSignatureDescriptor.Type = resolver.ResolveTypeTokenInternal(token, context);
					break;
				}
				case System.Reflection.Adds.CorElementType.TypeVar:
				{
					int num = SignatureUtil.ExtractInt(sig, ref index);
					if (GenericContext.IsNullOrEmptyTypeArgs(context))
					{
						throw new ArgumentException(MetadataStringTable.TypeArgumentCannotBeResolved);
					}
					typeSignatureDescriptor.Type = context.TypeArgs[num];
					break;
				}
				case System.Reflection.Adds.CorElementType.Array:
				{
					Type type = SignatureUtil.ExtractType(sig, ref index, resolver, context);
					int num1 = SignatureUtil.ExtractInt(sig, ref index);
					int num2 = SignatureUtil.ExtractInt(sig, ref index);
					for (int i = 0; i < num2; i++)
					{
						SignatureUtil.ExtractInt(sig, ref index);
					}
					int num3 = SignatureUtil.ExtractInt(sig, ref index);
					for (int j = 0; j < num3; j++)
					{
						SignatureUtil.ExtractInt(sig, ref index);
					}
					typeSignatureDescriptor.Type = type.MakeArrayType(num1);
					break;
				}
				case System.Reflection.Adds.CorElementType.GenericInstantiation:
				{
					Type type1 = SignatureUtil.ExtractType(sig, ref index, resolver, null);
					Type[] typeArray = new Type[SignatureUtil.ExtractInt(sig, ref index)];
					for (int k = 0; k < (int)typeArray.Length; k++)
					{
						typeArray[k] = SignatureUtil.ExtractType(sig, ref index, resolver, context);
					}
					typeSignatureDescriptor.Type = type1.MakeGenericType(typeArray);
					break;
				}
				case System.Reflection.Adds.CorElementType.TypedByRef:
				{
					typeSignatureDescriptor.Type = resolver.AssemblyResolver.GetTypeXFromName("System.TypedReference");
					break;
				}
				case System.Reflection.Adds.CorElementType.FnPtr:
				{
					SignatureUtil.ExtractCallingConvention(sig, ref index);
					int num4 = SignatureUtil.ExtractInt(sig, ref index);
					SignatureUtil.ExtractType(sig, ref index, resolver, context);
					for (int l = 0; l < num4; l++)
					{
						SignatureUtil.ExtractType(sig, ref index, resolver, context);
					}
					typeSignatureDescriptor.Type = resolver.AssemblyResolver.GetBuiltInType(System.Reflection.Adds.CorElementType.IntPtr);
					break;
				}
				case System.Reflection.Adds.CorElementType.SzArray:
				{
					typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context).MakeArrayType();
					break;
				}
				case System.Reflection.Adds.CorElementType.MethodVar:
				{
					int num5 = SignatureUtil.ExtractInt(sig, ref index);
					if (GenericContext.IsNullOrEmptyMethodArgs(context))
					{
						throw new ArgumentException(MetadataStringTable.TypeArgumentCannotBeResolved);
					}
					typeSignatureDescriptor.Type = context.MethodArgs[num5];
					break;
				}
				case System.Reflection.Adds.CorElementType.CModReqd:
				{
					Token token1 = SignatureUtil.ExtractToken(sig, ref index);
					resolver.ResolveTypeTokenInternal(token1, context);
					typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context);
					break;
				}
				case System.Reflection.Adds.CorElementType.CModOpt:
				{
					Token token2 = SignatureUtil.ExtractToken(sig, ref index);
					resolver.ResolveTypeTokenInternal(token2, context);
					typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context);
					break;
				}
				case System.Reflection.Adds.CorElementType.Pinned:
				{
					typeSignatureDescriptor.IsPinned = true;
					typeSignatureDescriptor.Type = SignatureUtil.ExtractType(sig, ref index, resolver, context);
					break;
				}
				default:
				{
					throw new ArgumentException(MetadataStringTable.IncorrectElementTypeValue);
				}
			}
			return typeSignatureDescriptor;
		}

		internal static Type ExtractTypeValue(ITypeUniverse universe, Module module, byte[] blob, ref int index)
		{
			Type type = null;
			string str = SignatureUtil.ExtractStringValue(blob, ref index);
			if (!string.IsNullOrEmpty(str))
			{
				type = System.Reflection.Adds.TypeNameParser.ParseTypeName(universe, module, str, false);
				if (type == null)
				{
					module = universe.GetSystemAssembly().ManifestModule;
					type = System.Reflection.Adds.TypeNameParser.ParseTypeName(universe, module, str);
				}
			}
			return type;
		}

		internal static uint ExtractUIntValue(byte[] blob, ref int index)
		{
			return (uint)SignatureUtil.ExtractValue(System.Reflection.Adds.CorElementType.UInt, blob, ref index);
		}

		internal static object ExtractValue(System.Reflection.Adds.CorElementType typeId, byte[] blob, ref int index)
		{
			object flag;
			switch (typeId)
			{
				case System.Reflection.Adds.CorElementType.Bool:
				{
					flag = BitConverter.ToBoolean(blob, index);
					index = index + 1;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.Char:
				{
					flag = BitConverter.ToChar(blob, index);
					index = index + 2;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.SByte:
				{
					flag = (sbyte)blob[index];
					index = index + 1;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.Byte:
				{
					flag = blob[index];
					index = index + 1;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.Short:
				{
					flag = BitConverter.ToInt16(blob, index);
					index = index + 2;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.UShort:
				{
					flag = BitConverter.ToUInt16(blob, index);
					index = index + 2;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.Int:
				{
					flag = BitConverter.ToInt32(blob, index);
					index = index + 4;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.UInt:
				{
					flag = BitConverter.ToUInt32(blob, index);
					index = index + 4;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.Long:
				{
					flag = BitConverter.ToInt64(blob, index);
					index = index + 8;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.ULong:
				{
					flag = BitConverter.ToUInt64(blob, index);
					index = index + 8;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.Float:
				{
					flag = BitConverter.ToSingle(blob, index);
					index = index + 4;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.Double:
				{
					flag = BitConverter.ToDouble(blob, index);
					index = index + 8;
					return flag;
				}
				case System.Reflection.Adds.CorElementType.String:
				{
					if (blob[index] != 255)
					{
						int num = SignatureUtil.ExtractInt(blob, ref index);
						flag = Encoding.UTF8.GetString(blob, index, num);
						index = index + num;
					}
					else
					{
						index = index + 1;
						flag = null;
					}
					return flag;
				}
			}
			throw new InvalidOperationException(MetadataStringTable.IncorrectElementTypeValue);
		}

		internal static CallingConventions GetReflectionCallingConvention(Microsoft.MetadataReader.CorCallingConvention callConvention)
		{
			CallingConventions callingConvention = (CallingConventions)0;
			if ((callConvention & Microsoft.MetadataReader.CorCallingConvention.Mask) == Microsoft.MetadataReader.CorCallingConvention.HasThis)
			{
				callingConvention = callingConvention | CallingConventions.HasThis;
			}
			else if ((callConvention & Microsoft.MetadataReader.CorCallingConvention.Mask) == Microsoft.MetadataReader.CorCallingConvention.ExplicitThis)
			{
				callingConvention = callingConvention | CallingConventions.ExplicitThis;
			}
			callingConvention = (!SignatureUtil.IsVarArg(callConvention) ? callingConvention | CallingConventions.Standard : callingConvention | CallingConventions.VarArgs);
			return callingConvention;
		}

		internal static StringComparison GetStringComparison(BindingFlags flags)
		{
			return ((flags & BindingFlags.IgnoreCase) == BindingFlags.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
		}

		internal static System.Reflection.Adds.CorElementType GetTypeId(Type type)
		{
			System.Reflection.Adds.CorElementType corElementType;
			if (type.IsEnum)
			{
				return SignatureUtil.GetTypeId(MetadataOnlyModule.GetUnderlyingType(type));
			}
			if (type.IsArray)
			{
				return System.Reflection.Adds.CorElementType.SzArray;
			}
			if (!SignatureUtil.TypeMapForAttributes.LookupPrimitive(type, out corElementType))
			{
				throw new ArgumentException(MetadataStringTable.UnsupportedTypeInAttributeSignature);
			}
			return corElementType;
		}

		internal static bool IsCallingConventionMatch(MethodBase method, CallingConventions callConvention)
		{
			if ((int)(callConvention & CallingConventions.Any) == 0)
			{
				if ((int)(callConvention & CallingConventions.VarArgs) != 0 && (int)(method.CallingConvention & CallingConventions.VarArgs) == 0)
				{
					return false;
				}
				if ((int)(callConvention & CallingConventions.Standard) != 0 && (int)(method.CallingConvention & CallingConventions.Standard) == 0)
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsGenericParametersCountMatch(MethodInfo method, int expectedGenericParameterCount)
		{
			int length = 0;
			if (method.IsGenericMethod)
			{
				length = (int)method.GetGenericArguments().Length;
			}
			return length == expectedGenericParameterCount;
		}

		internal static bool IsParametersTypeMatch(MethodBase method, Type[] parameterTypes)
		{
			if (parameterTypes == null)
			{
				return true;
			}
			ParameterInfo[] parameters = method.GetParameters();
			if ((int)parameters.Length != (int)parameterTypes.Length)
			{
				return false;
			}
			int length = (int)parameters.Length;
			for (int i = 0; i < length; i++)
			{
				if (!parameters[i].ParameterType.Equals(parameterTypes[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsValidCustomAttributeElementType(System.Reflection.Adds.CorElementType elementType)
		{
			return SignatureUtil.TypeMapForAttributes.IsValidCustomAttributeElementType(elementType);
		}

		internal static bool IsVarArg(Microsoft.MetadataReader.CorCallingConvention conv)
		{
			return (conv & Microsoft.MetadataReader.CorCallingConvention.Mask) == Microsoft.MetadataReader.CorCallingConvention.VarArg;
		}

		private static uint TokenFromRid(uint rid, uint tkType)
		{
			return rid | tkType;
		}

		internal static void VerifyElementType(System.Reflection.Adds.CorElementType elementType)
		{
			if (elementType != System.Reflection.Adds.CorElementType.Bool && elementType != System.Reflection.Adds.CorElementType.Char && elementType != System.Reflection.Adds.CorElementType.SByte && elementType != System.Reflection.Adds.CorElementType.Byte && elementType != System.Reflection.Adds.CorElementType.Short && elementType != System.Reflection.Adds.CorElementType.UShort && elementType != System.Reflection.Adds.CorElementType.Int && elementType != System.Reflection.Adds.CorElementType.UInt && elementType != System.Reflection.Adds.CorElementType.Long && elementType != System.Reflection.Adds.CorElementType.ULong && elementType != System.Reflection.Adds.CorElementType.Float && elementType != System.Reflection.Adds.CorElementType.Double && elementType != System.Reflection.Adds.CorElementType.String && elementType != System.Reflection.Adds.CorElementType.Type && elementType != System.Reflection.Adds.CorElementType.SzArray && elementType != System.Reflection.Adds.CorElementType.Enum && elementType != (System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Modifier | System.Reflection.Adds.CorElementType.Sentinel | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.Type))
			{
				throw new ArgumentException(MetadataStringTable.InvalidElementTypeInAttribute);
			}
		}

		public static class TypeMapForAttributes
		{
			private readonly static Dictionary<string, System.Reflection.Adds.CorElementType> s_typeNameMapForAttributes;

			static TypeMapForAttributes()
			{
				Dictionary<string, System.Reflection.Adds.CorElementType> strs = new Dictionary<string, System.Reflection.Adds.CorElementType>()
				{
					{ "System.Boolean", System.Reflection.Adds.CorElementType.Bool },
					{ "System.Char", System.Reflection.Adds.CorElementType.Char },
					{ "System.SByte", System.Reflection.Adds.CorElementType.SByte },
					{ "System.Byte", System.Reflection.Adds.CorElementType.Byte },
					{ "System.Int16", System.Reflection.Adds.CorElementType.Short },
					{ "System.UInt16", System.Reflection.Adds.CorElementType.UShort },
					{ "System.Int32", System.Reflection.Adds.CorElementType.Int },
					{ "System.UInt32", System.Reflection.Adds.CorElementType.UInt },
					{ "System.Int64", System.Reflection.Adds.CorElementType.Long },
					{ "System.UInt64", System.Reflection.Adds.CorElementType.ULong },
					{ "System.Single", System.Reflection.Adds.CorElementType.Float },
					{ "System.Double", System.Reflection.Adds.CorElementType.Double },
					{ "System.IntPtr", System.Reflection.Adds.CorElementType.IntPtr },
					{ "System.UIntPtr", System.Reflection.Adds.CorElementType.UIntPtr },
					{ "System.Array", System.Reflection.Adds.CorElementType.SzArray },
					{ "System.String", System.Reflection.Adds.CorElementType.String },
					{ "System.Type", System.Reflection.Adds.CorElementType.Type },
					{ "System.Object", System.Reflection.Adds.CorElementType.Object }
				};
				SignatureUtil.TypeMapForAttributes.s_typeNameMapForAttributes = strs;
			}

			public static bool IsValidCustomAttributeElementType(System.Reflection.Adds.CorElementType elementType)
			{
				return SignatureUtil.TypeMapForAttributes.s_typeNameMapForAttributes.ContainsValue(elementType);
			}

			public static bool LookupPrimitive(Type type, out System.Reflection.Adds.CorElementType result)
			{
				result = System.Reflection.Adds.CorElementType.End;
				ITypeUniverse typeUniverse = Helpers.Universe(type);
				if (typeUniverse != null && !typeUniverse.GetSystemAssembly().Equals(type.Assembly))
				{
					return false;
				}
				return SignatureUtil.TypeMapForAttributes.s_typeNameMapForAttributes.TryGetValue(type.FullName, out result);
			}
		}
	}
}