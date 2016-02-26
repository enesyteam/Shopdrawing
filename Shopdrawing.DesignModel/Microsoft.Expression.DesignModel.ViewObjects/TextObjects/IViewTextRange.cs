using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using System;

namespace Microsoft.Expression.DesignModel.ViewObjects.TextObjects
{
	public interface IViewTextRange : IViewObject
	{
		IViewTextPointer End
		{
			get;
		}

		bool IsEmpty
		{
			get;
		}

		IViewTextPointer Start
		{
			get;
		}

		string Text
		{
			get;
			set;
		}

		void ApplyPropertyValue(IPropertyId propertyId, object value);

		bool Contains(IViewTextPointer pointer);

		object GetPropertyValue(IPropertyId propertyId);

		object GetPropertyValue(IPropertyId propertyId, bool defaultIfInvalid);
	}
}