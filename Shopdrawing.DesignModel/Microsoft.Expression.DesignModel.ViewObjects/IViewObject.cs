using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public interface IViewObject
	{
		object DefaultStyleKey
		{
			get;
		}

		IPlatform Platform
		{
			get;
		}

		object PlatformSpecificObject
		{
			get;
		}

		Type TargetType
		{
			get;
		}

		void ClearValue(ITypeResolver typeResolver, IPropertyId propertyId);

		void ClearValue(IProperty propertyKey);

		void ClearValue(PropertyReference propertyReference);

		double GetBaseline();

		object GetBaseValue(ITypeResolver typeResolver, IPropertyId propertyId);

		object GetBaseValue(IProperty propertyKey);

		object GetBaseValue(PropertyReference propertyReference);

		object GetCurrentValue(ITypeResolver typeResolver, IPropertyId propertyId);

		object GetCurrentValue(IProperty propertyKey);

		object GetCurrentValue(PropertyReference propertyReference);

		IType GetIType(ITypeResolver typeResolver);

		object GetValue(ITypeResolver typeResolver, IPropertyId propertyId);

		object GetValue(IProperty propertyKey);

		object GetValue(PropertyReference propertyReference);

		bool IsSet(ITypeResolver typeResolver, IPropertyId propertyId);

		bool IsSet(IProperty propertyKey);

		bool IsSet(PropertyReference propertyReference);

		void SetValue(ITypeResolver typeResolver, IPropertyId propertyId, object value);

		void SetValue(ITypeResolver typeResolver, IProperty propertyKey, object value);

		void SetValue(ITypeResolver typeResolver, PropertyReference propertyReference, object value);
	}
}