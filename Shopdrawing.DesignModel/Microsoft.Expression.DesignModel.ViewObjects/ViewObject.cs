using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public abstract class ViewObject : IViewObject
	{
		private object viewObject;

		public abstract object DefaultStyleKey
		{
			get;
		}

		protected object Object
		{
			get
			{
				return this.viewObject;
			}
		}

		public abstract IPlatform Platform
		{
			get;
		}

		public object PlatformSpecificObject
		{
			get
			{
				return this.viewObject;
			}
		}

		public Type TargetType
		{
			get
			{
				if (this.viewObject == null)
				{
					return null;
				}
				return this.viewObject.GetType();
			}
		}

		protected ViewObject(object viewObject)
		{
			this.viewObject = viewObject;
		}

		public void ClearValue(ITypeResolver typeResolver, IPropertyId propertyId)
		{
			this.ClearValue(typeResolver.ResolveProperty(propertyId));
		}

		public void ClearValue(IProperty propertyKey)
		{
			this.PropertyReferenceFromPropertyKey(propertyKey).ClearValue(this.viewObject);
		}

		public void ClearValue(PropertyReference propertyReference)
		{
			propertyReference.ClearValue(this.viewObject);
		}

		public override bool Equals(object obj)
		{
			ViewObject viewObject = obj as ViewObject;
			if (viewObject == null)
			{
				return false;
			}
			return this.viewObject.Equals(viewObject.viewObject);
		}

		public virtual double GetBaseline()
		{
			return double.NaN;
		}

		public object GetBaseValue(ITypeResolver typeResolver, IPropertyId propertyId)
		{
			return this.GetBaseValue(typeResolver.ResolveProperty(propertyId));
		}

		public object GetBaseValue(IProperty propertyKey)
		{
			return this.PropertyReferenceFromPropertyKey(propertyKey).GetBaseValue(this.viewObject);
		}

		public object GetBaseValue(PropertyReference propertyReference)
		{
			return propertyReference.GetBaseValue(this.viewObject);
		}

		public object GetCurrentValue(ITypeResolver typeResolver, IPropertyId propertyId)
		{
			return this.GetCurrentValue(typeResolver.ResolveProperty(propertyId));
		}

		public object GetCurrentValue(IProperty propertyKey)
		{
			return this.PropertyReferenceFromPropertyKey(propertyKey).GetCurrentValue(this.viewObject);
		}

		public object GetCurrentValue(PropertyReference propertyReference)
		{
			return propertyReference.GetCurrentValue(this.viewObject);
		}

		public override int GetHashCode()
		{
			return this.PlatformSpecificObject.GetHashCode();
		}

		public IType GetIType(ITypeResolver typeResolver)
		{
			if (this.viewObject == null)
			{
				return null;
			}
			IType type = null;
			for (Type i = this.viewObject.GetType(); i != typeof(object); i = i.BaseType)
			{
				type = typeResolver.GetType(i);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		public object GetValue(ITypeResolver typeResolver, IPropertyId propertyId)
		{
			return this.GetValue(typeResolver.ResolveProperty(propertyId));
		}

		public object GetValue(IProperty propertyKey)
		{
			return this.PropertyReferenceFromPropertyKey(propertyKey).GetValue(this.viewObject);
		}

		public object GetValue(PropertyReference propertyReference)
		{
			return propertyReference.GetValue(this.viewObject);
		}

		public bool IsSet(ITypeResolver typeResolver, IPropertyId propertyId)
		{
			return this.IsSet(typeResolver.ResolveProperty(propertyId));
		}

		public bool IsSet(IProperty propertyKey)
		{
			return this.PropertyReferenceFromPropertyKey(propertyKey).IsSet(this.viewObject);
		}

		public bool IsSet(PropertyReference propertyReference)
		{
			return propertyReference.IsSet(this.viewObject);
		}

		private PropertyReference PropertyReferenceFromPropertyKey(IProperty propertyKey)
		{
			return new PropertyReference(propertyKey as ReferenceStep);
		}

		public void SetValue(ITypeResolver typeResolver, IPropertyId propertyId, object value)
		{
			this.SetValue(typeResolver, typeResolver.ResolveProperty(propertyId), value);
		}

		public void SetValue(ITypeResolver typeResolver, IProperty propertyKey, object value)
		{
			this.SetValue(typeResolver, this.PropertyReferenceFromPropertyKey(propertyKey), value);
		}

		public void SetValue(ITypeResolver typeResolver, PropertyReference propertyReference, object value)
		{
			if (this.viewObject != null)
			{
				propertyReference = DesignTimeProperties.GetAppliedShadowPropertyReference(propertyReference, typeResolver.GetType(this.TargetType));
				propertyReference.SetValue(this.viewObject, value);
			}
		}
	}
}