using System;
using System.Globalization;

namespace System.Reflection.Adds
{
	internal static class ElementTypeUtility
	{
		public static string GetNameForPrimitive(System.Reflection.Adds.CorElementType value)
		{
			object[] objArray;
			CultureInfo invariantCulture;
			string illegalElementType;
			System.Reflection.Adds.CorElementType corElementType = value;
			switch (corElementType)
			{
				case System.Reflection.Adds.CorElementType.Void:
				{
					return "System.Void";
				}
				case System.Reflection.Adds.CorElementType.Bool:
				{
					return "System.Boolean";
				}
				case System.Reflection.Adds.CorElementType.Char:
				{
					return "System.Char";
				}
				case System.Reflection.Adds.CorElementType.SByte:
				{
					return "System.SByte";
				}
				case System.Reflection.Adds.CorElementType.Byte:
				{
					return "System.Byte";
				}
				case System.Reflection.Adds.CorElementType.Short:
				{
					return "System.Int16";
				}
				case System.Reflection.Adds.CorElementType.UShort:
				{
					return "System.UInt16";
				}
				case System.Reflection.Adds.CorElementType.Int:
				{
					return "System.Int32";
				}
				case System.Reflection.Adds.CorElementType.UInt:
				{
					return "System.UInt32";
				}
				case System.Reflection.Adds.CorElementType.Long:
				{
					return "System.Int64";
				}
				case System.Reflection.Adds.CorElementType.ULong:
				{
					return "System.UInt64";
				}
				case System.Reflection.Adds.CorElementType.Float:
				{
					return "System.Single";
				}
				case System.Reflection.Adds.CorElementType.Double:
				{
					return "System.Double";
				}
				case System.Reflection.Adds.CorElementType.String:
				{
					return "System.String";
				}
				case System.Reflection.Adds.CorElementType.Pointer:
				case System.Reflection.Adds.CorElementType.Byref:
				case System.Reflection.Adds.CorElementType.ValueType:
				case System.Reflection.Adds.CorElementType.Class:
				case System.Reflection.Adds.CorElementType.TypeVar:
				case System.Reflection.Adds.CorElementType.Array:
				case System.Reflection.Adds.CorElementType.GenericInstantiation:
				case System.Reflection.Adds.CorElementType.TypedByRef:
				case System.Reflection.Adds.CorElementType.Array | System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Char | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.SByte | System.Reflection.Adds.CorElementType.Short | System.Reflection.Adds.CorElementType.TypedByRef | System.Reflection.Adds.CorElementType.Byte | System.Reflection.Adds.CorElementType.UShort | System.Reflection.Adds.CorElementType.ValueType | System.Reflection.Adds.CorElementType.Void | System.Reflection.Adds.CorElementType.TypeVar | System.Reflection.Adds.CorElementType.GenericInstantiation:
				case System.Reflection.Adds.CorElementType.Bool | System.Reflection.Adds.CorElementType.Byref | System.Reflection.Adds.CorElementType.Class | System.Reflection.Adds.CorElementType.IntPtr | System.Reflection.Adds.CorElementType.Int | System.Reflection.Adds.CorElementType.Long:
				case System.Reflection.Adds.CorElementType.FnPtr:
				{
					invariantCulture = CultureInfo.InvariantCulture;
					illegalElementType = MetadataStringTable.IllegalElementType;
					objArray = new object[] { value };
					throw new ArgumentException(string.Format(invariantCulture, illegalElementType, objArray));
				}
				case System.Reflection.Adds.CorElementType.IntPtr:
				{
					return "System.IntPtr";
				}
				case System.Reflection.Adds.CorElementType.UIntPtr:
				{
					return "System.UIntPtr";
				}
				case System.Reflection.Adds.CorElementType.Object:
				{
					return "System.Object";
				}
				default:
				{
					if (corElementType == System.Reflection.Adds.CorElementType.Type)
					{
						return "System.Type";
					}
					invariantCulture = CultureInfo.InvariantCulture;
					illegalElementType = MetadataStringTable.IllegalElementType;
					objArray = new object[] { value };
					throw new ArgumentException(string.Format(invariantCulture, illegalElementType, objArray));
				}
			}
		}
	}
}