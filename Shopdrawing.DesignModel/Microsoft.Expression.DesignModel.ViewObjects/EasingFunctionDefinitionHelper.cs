using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public static class EasingFunctionDefinitionHelper
	{
		private const string EasingFunctionNameSuffix = "Ease";

		public static string GetEasingFunctionGroupName(IEasingFunctionDefinition easingFunctionDefinition)
		{
			Type type = easingFunctionDefinition.PlatformSpecificObject.GetType();
			if (type.Namespace.IndexOf(".", StringComparison.Ordinal) <= 0)
			{
				return type.Namespace;
			}
			return type.Namespace.Substring(0, type.Namespace.IndexOf(".", StringComparison.Ordinal));
		}

		public static string GetEasingFunctionName(IEasingFunctionDefinition easingFunctionDefinition)
		{
			return EasingFunctionDefinitionHelper.GetEasingFunctionName(easingFunctionDefinition.PlatformSpecificObject.GetType());
		}

		internal static string GetEasingFunctionName(Type easingFunctionType)
		{
			if (!easingFunctionType.Name.EndsWith("Ease", StringComparison.Ordinal) || easingFunctionType.Name.LastIndexOf("Ease", StringComparison.Ordinal) <= 0)
			{
				return easingFunctionType.Name;
			}
			return easingFunctionType.Name.Substring(0, easingFunctionType.Name.Length - "Ease".Length);
		}
	}
}