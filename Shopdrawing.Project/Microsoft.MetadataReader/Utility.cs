using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.MetadataReader
{
	internal static class Utility
	{
		public static bool Compare(string string1, string string2, bool ignoreCase)
		{
			return string.Equals(string1, string2, (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
		}

		internal static string GetNamespaceHelper(string fullName)
		{
			if (!fullName.Contains("."))
			{
				return null;
			}
			return fullName.Substring(0, fullName.LastIndexOf('.'));
		}

		internal static string GetTypeNameFromFullNameHelper(string fullName, bool isNested)
		{
			if (isNested)
			{
				return fullName.Substring(fullName.LastIndexOf('+') + 1);
			}
			return fullName.Substring(fullName.LastIndexOf('.') + 1);
		}

		public static bool IsBindingFlagsMatching(MethodBase method, bool isInherited, BindingFlags bindingFlags)
		{
			return Utility.IsBindingFlagsMatching(method, method.IsStatic, method.IsPublic, isInherited, bindingFlags);
		}

		public static bool IsBindingFlagsMatching(FieldInfo fieldInfo, bool isInherited, BindingFlags bindingFlags)
		{
			return Utility.IsBindingFlagsMatching(fieldInfo, fieldInfo.IsStatic, fieldInfo.IsPublic, isInherited, bindingFlags);
		}

		public static bool IsBindingFlagsMatching(MemberInfo memberInfo, bool isStatic, bool isPublic, bool isInherited, BindingFlags bindingFlags)
		{
			if ((bindingFlags & BindingFlags.DeclaredOnly) != BindingFlags.Default && isInherited)
			{
				return false;
			}
			if (isPublic)
			{
				if ((bindingFlags & BindingFlags.Public) == BindingFlags.Default)
				{
					return false;
				}
			}
			else if ((bindingFlags & BindingFlags.NonPublic) == BindingFlags.Default)
			{
				return false;
			}
			if (memberInfo.MemberType != MemberTypes.TypeInfo && memberInfo.MemberType != MemberTypes.NestedType)
			{
				if (isStatic)
				{
					if ((bindingFlags & BindingFlags.FlattenHierarchy) == BindingFlags.Default && isInherited)
					{
						return false;
					}
					if ((bindingFlags & BindingFlags.Static) == BindingFlags.Default)
					{
						return false;
					}
				}
				else if ((bindingFlags & BindingFlags.Instance) == BindingFlags.Default)
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsValidPath(string modulePath)
		{
			bool flag;
			if (string.IsNullOrEmpty(modulePath))
			{
				return false;
			}
			char[] invalidPathChars = Path.GetInvalidPathChars();
			int num = 0;
			while (true)
			{
				if (num < (int)invalidPathChars.Length)
				{
					char chr = invalidPathChars[num];
					string str = modulePath;
					int num1 = 0;
					while (num1 < str.Length)
					{
						if (chr != str[num1])
						{
							num1++;
						}
						else
						{
							flag = false;
							return flag;
						}
					}
					num++;
				}
				else
				{
					try
					{
						if (!Path.IsPathRooted(modulePath))
						{
							flag = false;
							break;
						}
					}
					catch (Exception exception)
					{
						throw;
					}
					return true;
				}
			}
			return flag;
		}

		internal static void VerifyNotByRef(MetadataOnlyCommonType type)
		{
			if (type.IsByRef)
			{
				string str = string.Concat(type.Name, "&");
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string cannotFindTypeInModule = MetadataStringTable.CannotFindTypeInModule;
				object[] objArray = new object[] { str, type.Resolver.ToString() };
				throw new TypeLoadException(string.Format(invariantCulture, cannotFindTypeInModule, objArray));
			}
		}
	}
}