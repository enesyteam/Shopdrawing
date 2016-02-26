using System;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal class AttributeData
	{
		private Type _attributeType;

		private bool? _isInheritable;

		private bool? _allowsMultiple;

		internal bool AllowsMultiple
		{
			get
			{
				if (!this._allowsMultiple.HasValue)
				{
					this.ParseUsageAttributes();
				}
				return this._allowsMultiple.Value;
			}
		}

		internal Type AttributeType
		{
			get
			{
				return this._attributeType;
			}
		}

		internal bool IsInheritable
		{
			get
			{
				if (!this._isInheritable.HasValue)
				{
					this.ParseUsageAttributes();
				}
				return this._isInheritable.Value;
			}
		}

		internal AttributeData(Type attributeType)
		{
			this._attributeType = attributeType;
		}

		private void ParseUsageAttributes()
		{
			this._isInheritable = new bool?(false);
			this._allowsMultiple = new bool?(false);
			object[] customAttributes = this._attributeType.GetCustomAttributes(typeof(AttributeUsageAttribute), true);
			if (customAttributes != null && (int)customAttributes.Length > 0)
			{
				for (int i = 0; i < (int)customAttributes.Length; i++)
				{
					AttributeUsageAttribute attributeUsageAttribute = (AttributeUsageAttribute)customAttributes[i];
					this._isInheritable = new bool?(attributeUsageAttribute.Inherited);
					this._allowsMultiple = new bool?(attributeUsageAttribute.AllowMultiple);
				}
			}
		}
	}
}