using System;
using System.ComponentModel;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public abstract class ReferenceStep : Property
	{
		public virtual bool AllowNullObjectToInspect
		{
			get
			{
				return false;
			}
		}

		public string Category
		{
			get
			{
				string str;
				if (PlatformNeutralAttributeHelper.TryGetAttributeValue<string>(this.Attributes, PlatformTypes.CategoryAttribute, "Category", out str))
				{
					return str;
				}
				return CategoryAttribute.Default.Category;
			}
		}

		public virtual string ConstructorArgument
		{
			get
			{
				return null;
			}
		}

		public string Description
		{
			get
			{
				string str;
				if (PlatformNeutralAttributeHelper.TryGetAttributeValue<string>(this.Attributes, PlatformTypes.DescriptionAttribute, "Description", out str))
				{
					return str;
				}
				return DescriptionAttribute.Default.Description;
			}
		}

		public bool IsBrowsable
		{
			get
			{
				bool flag;
				if (PlatformNeutralAttributeHelper.TryGetAttributeValue<bool>(this.Attributes, PlatformTypes.BrowsableAttribute, "Browsable", out flag))
				{
					return flag;
				}
				return BrowsableAttribute.Default.Browsable;
			}
		}

        //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        //public override Type TargetType
        //{
        //    get;
        //}

		protected ReferenceStep(IType declaringType, string name, IType valueType, int sortValue) : base(declaringType, name, valueType, sortValue)
		{
		}

		public abstract void ClearValue(object target);

		public virtual object GetBaseValue(object objToInspect)
		{
			return this.GetValue(objToInspect);
		}

		public virtual object GetCurrentValue(object objToInspect)
		{
			return this.GetValue(objToInspect);
		}

		public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);

		public abstract object GetValue(object objToInspect);

		public abstract bool IsAnimated(object target);

		public abstract bool IsSet(object objToInspect);

		public abstract object SetValue(object target, object valueToSet);

		public abstract bool ShouldSerializeMethod(object objToInspect);
	}
}