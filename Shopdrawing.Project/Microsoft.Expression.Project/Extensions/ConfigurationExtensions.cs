using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Interop;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Extensions
{
	public static class ConfigurationExtensions
	{
		public static IConfigurationObjectCollection GetOrCreateConfigurationObjectCollectionProperty(this IConfigurationObject configurationObject, string propertyName)
		{
			IConfigurationObject configurationObject1 = configurationObject;
			return configurationObject.GetOrCreateProperty<IConfigurationObjectCollection>(propertyName, new Func<IConfigurationObjectCollection>(configurationObject1.CreateConfigurationObjectCollection));
		}

		public static IConfigurationObject GetOrCreateConfigurationObjectProperty(this IConfigurationObject configurationObject, string propertyName)
		{
			IConfigurationObject configurationObject1 = configurationObject;
			return configurationObject.GetOrCreateProperty<IConfigurationObject>(propertyName, new Func<IConfigurationObject>(configurationObject1.CreateConfigurationObject));
		}

		public static T GetOrCreateProperty<T>(this IConfigurationObject configurationObject, string propertyName, Func<T> propertyConstructor)
		where T : class
		{
			T property;
			if (propertyConstructor == null)
			{
				throw new ArgumentNullException("propertyConstructor");
			}
			if (configurationObject.HasProperty(propertyName))
			{
				property = (T)(configurationObject.GetProperty(propertyName) as T);
				if (property != null)
				{
					return property;
				}
			}
			property = propertyConstructor();
			configurationObject.SetProperty(propertyName, property);
			return property;
		}

		public static T GetPropertyOrDefault<T>(this IConfigurationObject configurationObject, string propertyName)
		{
			if (!configurationObject.HasProperty(propertyName))
			{
				return default(T);
			}
			return TypeHelper.ConvertType<T>(configurationObject.GetProperty(propertyName));
		}
	}
}