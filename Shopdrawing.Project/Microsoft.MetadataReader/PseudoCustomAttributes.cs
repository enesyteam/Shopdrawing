using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Microsoft.MetadataReader
{
	internal static class PseudoCustomAttributes
	{
		public const string TypeForwardedToAttributeName = "System.Runtime.CompilerServices.TypeForwardedToAttribute";

		public const string SerializableAttributeName = "System.SerializableAttribute";

		internal static IEnumerable<UnresolvedTypeName> GetRawTypeForwardedToAttributes(MetadataOnlyAssembly assembly)
		{
			return PseudoCustomAttributes.GetRawTypeForwardedToAttributes(assembly.ManifestModuleInternal);
		}

		internal static IEnumerable<UnresolvedTypeName> GetRawTypeForwardedToAttributes(MetadataOnlyModule manifestModule)
		{
			int num;
			uint num1;
			int num2;
			int num3;
			int num4;
			CorTypeAttr corTypeAttr;
			HCORENUM hCORENUM = new HCORENUM();
			IMetadataAssemblyImport rawImport = (IMetadataAssemblyImport)manifestModule.RawImport;
			try
			{
				while (true)
				{
					rawImport.EnumExportedTypes(ref hCORENUM, out num, 1, out num1);
					if (num1 == 0)
					{
						break;
					}
					rawImport.GetExportedTypeProps(num, null, 0, out num2, out num3, out num4, out corTypeAttr);
					Token token = new Token(num3);
					if (token.TokenType == System.Reflection.Adds.TokenType.AssemblyRef)
					{
						StringBuilder stringBuilder = new StringBuilder(num2);
						rawImport.GetExportedTypeProps(num, stringBuilder, stringBuilder.Capacity, out num2, out num3, out num4, out corTypeAttr);
						AssemblyName assemblyNameFromRef = AssemblyNameHelper.GetAssemblyNameFromRef(token, manifestModule, rawImport);
						yield return new UnresolvedTypeName(stringBuilder.ToString(), assemblyNameFromRef);
					}
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
		}

		public static CustomAttributeData GetSerializableAttribute(MetadataOnlyModule module, Token token)
		{
			int num;
			TypeAttributes typeAttribute;
			int num1;
			if (token.TokenType != System.Reflection.Adds.TokenType.TypeDef)
			{
				return null;
			}
			module.RawImport.GetTypeDefProps(token.Value, null, 0, out num, out typeAttribute, out num1);
			if ((typeAttribute & TypeAttributes.Serializable) == TypeAttributes.NotPublic)
			{
				return null;
			}
			Assembly systemAssembly = module.AssemblyResolver.GetSystemAssembly();
			ConstructorInfo[] constructors = systemAssembly.GetType("System.SerializableAttribute", true, false).GetConstructors();
			List<CustomAttributeTypedArgument> customAttributeTypedArguments = new List<CustomAttributeTypedArgument>(0);
			List<CustomAttributeNamedArgument> customAttributeNamedArguments = new List<CustomAttributeNamedArgument>(0);
			return new MetadataOnlyCustomAttributeData(constructors[0], customAttributeTypedArguments, customAttributeNamedArguments);
		}

		public static IEnumerable<CustomAttributeData> GetTypeForwardedToAttributes(MetadataOnlyAssembly assembly)
		{
			return PseudoCustomAttributes.GetTypeForwardedToAttributes(assembly.ManifestModuleInternal);
		}

		public static IEnumerable<CustomAttributeData> GetTypeForwardedToAttributes(MetadataOnlyModule manifestModule, Token token)
		{
			if (token.TokenType != System.Reflection.Adds.TokenType.Assembly)
			{
				return new CustomAttributeData[0];
			}
			return PseudoCustomAttributes.GetTypeForwardedToAttributes(manifestModule);
		}

		public static IEnumerable<CustomAttributeData> GetTypeForwardedToAttributes(MetadataOnlyModule manifestModule)
		{
			ITypeUniverse assemblyResolver = manifestModule.AssemblyResolver;
			Type builtInType = assemblyResolver.GetBuiltInType(System.Reflection.Adds.CorElementType.Type);
			Assembly assembly = assemblyResolver.GetSystemAssembly();
			Type type = assembly.GetType("System.Runtime.CompilerServices.TypeForwardedToAttribute", false, false);
			if (type != null)
			{
				foreach (UnresolvedTypeName rawTypeForwardedToAttribute in PseudoCustomAttributes.GetRawTypeForwardedToAttributes(manifestModule))
				{
					ConstructorInfo[] constructorInfoArray = type.GetConstructors();
					Type type1 = rawTypeForwardedToAttribute.ConvertToType(assemblyResolver, manifestModule);
					List<CustomAttributeTypedArgument> customAttributeTypedArguments = new List<CustomAttributeTypedArgument>(1)
					{
						new CustomAttributeTypedArgument(builtInType, type1)
					};
					List<CustomAttributeNamedArgument> customAttributeNamedArguments = new List<CustomAttributeNamedArgument>(0);
					yield return new MetadataOnlyCustomAttributeData(constructorInfoArray[0], customAttributeTypedArguments, customAttributeNamedArguments);
				}
			}
		}

		public static Type GetTypeFromTypeForwardToAttribute(CustomAttributeData data)
		{
			return (Type)data.ConstructorArguments[0].Value;
		}
	}
}