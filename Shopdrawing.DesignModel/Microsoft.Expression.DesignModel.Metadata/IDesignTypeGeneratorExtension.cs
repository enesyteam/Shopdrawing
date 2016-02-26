using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public interface IDesignTypeGeneratorExtension
	{
		Type GetDesignType(IDesignTypeGeneratorContext context, Type runtimeType);

		Type GetReplacementType(Type type);

		void OnPropertySet(IDesignTypeGeneratorContext context, TypeBuilder designTimeType, PropertyInfo propertyInfo, ILGenerator setMethod);

		void OnTypeCloned(IDesignTypeGeneratorContext context, BuildingTypeInfo typeInfo);

		bool ShouldReflectMethod(IDesignTypeGeneratorContext context, BuildingTypeInfo typeInfo, MethodInfo sourceMethod);

		bool ShouldReflectProperty(IDesignTypeGeneratorContext context, BuildingTypeInfo typeInfo, PropertyInfo sourceProperty);
	}
}