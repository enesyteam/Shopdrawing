using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public static class ITypeExtensions
	{
		public static IEnumerable<IProperty> GetProperties(this IType type, MemberAccessTypes access, bool flattenHierarchy)
		{
			if (!flattenHierarchy)
			{
				return type.GetProperties(access);
			}
			List<IProperty> properties = new List<IProperty>();
			while (type != null)
			{
				properties.AddRange(type.GetProperties(access));
				type = type.BaseType;
			}
			return properties;
		}
	}
}