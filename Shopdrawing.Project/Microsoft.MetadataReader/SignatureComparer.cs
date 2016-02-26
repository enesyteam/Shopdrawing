using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.MetadataReader
{
	internal static class SignatureComparer
	{
		private const BindingFlags MembersDeclaredOnTypeOnly = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		public static IEnumerable<MethodBase> FilterConstructors(MethodFilter filter, ConstructorInfo[] allConstructors)
		{
			List<MethodBase> methodBases = new List<MethodBase>();
			CallingConventions reflectionCallingConvention = SignatureUtil.GetReflectionCallingConvention(filter.CallingConvention);
			ConstructorInfo[] constructorInfoArray = allConstructors;
			for (int i = 0; i < (int)constructorInfoArray.Length; i++)
			{
				ConstructorInfo constructorInfo = constructorInfoArray[i];
				if (constructorInfo.Name.Equals(filter.Name, StringComparison.Ordinal) && SignatureUtil.IsCallingConventionMatch(constructorInfo, reflectionCallingConvention) && (int)constructorInfo.GetParameters().Length == filter.ParameterCount)
				{
					methodBases.Add(constructorInfo);
				}
			}
			return methodBases;
		}

		public static IEnumerable<MethodBase> FilterMethods(MethodFilter filter, MethodInfo[] allMethods)
		{
			List<MethodBase> methodBases = new List<MethodBase>();
			CallingConventions reflectionCallingConvention = SignatureUtil.GetReflectionCallingConvention(filter.CallingConvention);
			MethodInfo[] methodInfoArray = allMethods;
			for (int i = 0; i < (int)methodInfoArray.Length; i++)
			{
				MethodInfo methodInfo = methodInfoArray[i];
				if (methodInfo.Name.Equals(filter.Name, StringComparison.Ordinal) && SignatureUtil.IsCallingConventionMatch(methodInfo, reflectionCallingConvention) && SignatureUtil.IsGenericParametersCountMatch(methodInfo, filter.GenericParameterCount) && (int)methodInfo.GetParameters().Length == filter.ParameterCount)
				{
					methodBases.Add(methodInfo);
				}
			}
			return methodBases;
		}

		public static MethodBase FindMatchingMethod(string methodName, Type typeToInspect, MethodSignatureDescriptor expectedSignature, GenericContext context)
		{
			bool flag = (methodName.Equals(".ctor", StringComparison.Ordinal) ? true : methodName.Equals(".cctor", StringComparison.Ordinal));
			int genericParameterCount = expectedSignature.GenericParameterCount;
			IEnumerable<MethodBase> methodBases = null;
			MethodFilter methodFilter = new MethodFilter(methodName, genericParameterCount, (int)expectedSignature.Parameters.Length, expectedSignature.CallingConvention);
			methodBases = (!flag ? SignatureComparer.FilterMethods(methodFilter, typeToInspect.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) : SignatureComparer.FilterConstructors(methodFilter, typeToInspect.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)));
			MethodBase methodBase = null;
			bool flag1 = false;
			foreach (MethodBase methodBase1 in methodBases)
			{
				MethodBase methodBase2 = methodBase1;
				bool flag2 = false;
				if (genericParameterCount > 0 && (int)context.MethodArgs.Length > 0)
				{
					methodBase2 = (methodBase1 as MethodInfo).MakeGenericMethod(context.MethodArgs);
					flag2 = true;
				}
				MethodBase templateMethod = null;
				if (!typeToInspect.IsGenericType)
				{
					templateMethod = (!flag2 ? methodBase2 : methodBase1);
				}
				else
				{
					templateMethod = SignatureComparer.GetTemplateMethod(typeToInspect, methodBase2.MetadataToken);
				}
				if (!flag && !expectedSignature.ReturnParameter.Type.Equals((templateMethod as MethodInfo).ReturnType) || !SignatureComparer.IsParametersTypeMatch(templateMethod, expectedSignature.Parameters))
				{
					continue;
				}
				if (flag1)
				{
					throw new AmbiguousMatchException();
				}
				methodBase = methodBase2;
				flag1 = true;
			}
			return methodBase;
		}

		private static MethodBase GetTemplateMethod(Type typeToInspect, int methodToken)
		{
			return typeToInspect.GetGenericTypeDefinition().Module.ResolveMethod(methodToken);
		}

		internal static bool IsParametersTypeMatch(MethodBase templateMethod, TypeSignatureDescriptor[] parameters)
		{
			ParameterInfo[] parameterInfoArray = templateMethod.GetParameters();
			int length = (int)parameterInfoArray.Length;
			for (int i = 0; i < length; i++)
			{
				if (!parameters[i].Type.Equals(parameterInfoArray[i].ParameterType))
				{
					return false;
				}
			}
			return true;
		}
	}
}