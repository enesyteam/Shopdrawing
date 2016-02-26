using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class InstanceTypeReplacement
	{
		private Dictionary<IProperty, IProperty> propertyReplacementDictionary;

		public IType ReplacementType
		{
			get;
			set;
		}

		public IType SourceType
		{
			get;
			set;
		}

		public InstanceTypeReplacement(IType sourceType, IType replacementType)
		{
			this.SourceType = sourceType;
			this.ReplacementType = replacementType;
			this.propertyReplacementDictionary = new Dictionary<IProperty, IProperty>();
		}

		public IProperty GetReplacementProperty(IProperty propertyReference)
		{
			IProperty property1;
			if (this.propertyReplacementDictionary.TryGetValue(propertyReference, out property1))
			{
				return property1;
			}
			IList<IProperty> properties = new List<IProperty>(this.ReplacementType.GetProperties(MemberAccessTypes.Public, true));
			property1 = properties.FirstOrDefault<IProperty>((IProperty property) => property.Name.Equals(propertyReference.Name, StringComparison.Ordinal));
			if (property1 != null && property1.PropertyType == propertyReference.PropertyType)
			{
				this.propertyReplacementDictionary.Add(propertyReference, property1);
				return property1;
			}
			this.propertyReplacementDictionary.Add(propertyReference, propertyReference);
			return propertyReference;
		}
	}
}