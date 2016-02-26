using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class ClrObjectMetadata : ITypeMetadata
	{
		public readonly static Type StaticBaseType;

		private ITypeResolver typeResolver;

		private Type baseType;

		private bool? isNameScope;

		private bool? whitespaceSignificant;

		private bool supportsInlineXml;

		private DelayedInstance<IProperty> nameProperty;

		private bool contentPropertyIsInitialized;

		private bool alternateContentPropertiesAreInitialized;

		private IProperty contentProperty;

		private bool contentPropertyIsHidden;

		private bool? trimSurroundingWhitespace;

		private ReadOnlyCollection<IPropertyId> contentProperties;

		private ReadOnlyCollection<IPropertyId> alternateContentProperties;

		private ReadOnlyCollection<IProperty> properties;

		private ReadOnlyCollection<IPropertyId> styleProperties;

		public Type BaseType
		{
			get
			{
				return this.baseType;
			}
		}

		public ReadOnlyCollection<IPropertyId> ContentProperties
		{
			get
			{
				if (this.contentProperties == null)
				{
					this.contentProperties = new ReadOnlyCollection<IPropertyId>(this.GetContentProperties());
				}
				return this.contentProperties;
			}
		}

		public virtual IProperty DefaultContentProperty
		{
			get
			{
				this.InitializeContentPropertyIfNecessary();
				if (this.contentPropertyIsHidden)
				{
					return null;
				}
				return this.contentProperty;
			}
		}

		public virtual IPropertyId ImplicitDictionaryKeyProperty
		{
			get
			{
				return null;
			}
		}

		public bool IsNameScope
		{
			get
			{
				if (!this.isNameScope.HasValue)
				{
					this.isNameScope = new bool?(this.typeResolver.PlatformMetadata.GetIsTypeItsOwnNameScope(this.typeResolver.GetType(this.baseType)));
				}
				return this.isNameScope.Value;
			}
		}

		public bool IsWhitespaceSignificant
		{
			get
			{
				if (!this.whitespaceSignificant.HasValue)
				{
					WhitespaceSignificantCollectionAttribute attribute = TypeUtilities.GetAttribute<WhitespaceSignificantCollectionAttribute>(this.baseType);
					this.whitespaceSignificant = new bool?(attribute != null);
				}
				return this.whitespaceSignificant.Value;
			}
		}

		IProperty Microsoft.Expression.DesignModel.Metadata.ITypeMetadata.NameProperty
		{
			get
			{
				return this.nameProperty.Value;
			}
		}

		IPropertyId Microsoft.Expression.DesignModel.Metadata.ITypeMetadata.ResourcesProperty
		{
			get
			{
				return this.ResourcesPropertyInternal;
			}
		}

		public ReadOnlyCollection<IProperty> Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ReadOnlyCollection<IProperty>(this.GetProperties());
				}
				return this.properties;
			}
		}

		protected virtual IPropertyId ResourcesPropertyInternal
		{
			get
			{
				return null;
			}
		}

		public bool ShouldTrimSurroundingWhitespace
		{
			get
			{
				if (!this.trimSurroundingWhitespace.HasValue)
				{
					TrimSurroundingWhitespaceAttribute attribute = TypeUtilities.GetAttribute<TrimSurroundingWhitespaceAttribute>(this.baseType);
					this.trimSurroundingWhitespace = new bool?(attribute != null);
				}
				return this.trimSurroundingWhitespace.Value;
			}
		}

		public virtual ReadOnlyCollection<IPropertyId> StyleProperties
		{
			get
			{
				if (this.styleProperties == null)
				{
					this.styleProperties = new ReadOnlyCollection<IPropertyId>(this.GetStyleProperties());
				}
				return this.styleProperties;
			}
		}

		public bool SupportsInlineXml
		{
			get
			{
				return this.supportsInlineXml;
			}
		}

		public ITypeResolver TypeResolver
		{
			get
			{
				return this.typeResolver;
			}
			set
			{
				this.typeResolver = value;
			}
		}

		static ClrObjectMetadata()
		{
			ClrObjectMetadata.StaticBaseType = typeof(object);
		}

		public ClrObjectMetadata(Type baseType)
		{
			this.baseType = baseType;
			this.isNameScope = null;
			this.supportsInlineXml = typeof(IXmlSerializable).IsAssignableFrom(baseType);
			this.nameProperty = new DelayedInstance<IProperty>(() => {
				IProperty nameProperty = ((PlatformTypes)this.typeResolver.PlatformMetadata).GetNameProperty(this.typeResolver, this.baseType);
				if (nameProperty != null && (nameProperty.WriteAccess & MemberAccessType.Public) != MemberAccessType.None)
				{
					return nameProperty;
				}
				return this.TypeResolver.ResolveProperty(this.typeResolver.PlatformMetadata.KnownProperties.DesignTimeXName);
			});
		}

		protected virtual IList<IPropertyId> GetContentProperties()
		{
			List<IPropertyId> propertyIds = new List<IPropertyId>();
			this.InitializeContentPropertyIfNecessary();
			this.InitializeAlternateContentPropertiesIfNecessary();
			if (this.contentProperty != null)
			{
				propertyIds.Add(this.contentProperty);
			}
			if (this.alternateContentProperties != null)
			{
				propertyIds.AddRange(this.alternateContentProperties);
			}
			return propertyIds;
		}

		private static ReferenceStep GetContentProperty(ITypeResolver typeResolver, Type type, string propertyName)
		{
			IType type1 = typeResolver.GetType(type);
			if (type1 == null)
			{
				return null;
			}
			MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(typeResolver, type1);
			if (PlatformTypes.UserControl.IsAssignableFrom(type1))
			{
				allowableMemberAccess = allowableMemberAccess | MemberAccessTypes.Protected;
			}
			return (ReferenceStep)type1.GetMember(MemberType.Property, propertyName, allowableMemberAccess);
		}

		private static string GetNameFromNearestNonEmptyContentPropertyAttribute(IPlatformMetadata platformMetadata, Type type, out bool anySkipped)
		{
			anySkipped = false;
			IType type1 = platformMetadata.ResolveType(PlatformTypes.ContentPropertyAttribute);
			ReferenceStep member = (ReferenceStep)type1.GetMember(MemberType.Property, "Name", MemberAccessTypes.Public);
			string valueFromAttribute = ClrObjectMetadata.GetValueFromAttribute(type1, member, type, true) as string;
			if (!string.IsNullOrEmpty(valueFromAttribute))
			{
				return valueFromAttribute;
			}
			while (type != null)
			{
				valueFromAttribute = ClrObjectMetadata.GetValueFromAttribute(type1, member, type, false) as string;
				if (!string.IsNullOrEmpty(valueFromAttribute))
				{
					anySkipped = true;
					return valueFromAttribute;
				}
				type = type.BaseType;
			}
			return null;
		}

		protected virtual IList<IProperty> GetProperties()
		{
			IType type = this.TypeResolver.GetType(this.baseType);
			IEnumerable<IProperty> properties = type.GetProperties(MemberAccessTypes.Public, true);
			IList<IProperty> properties1 = properties as IList<IProperty>;
			if (properties1 == null)
			{
				properties1 = new List<IProperty>(properties);
			}
			return properties1;
		}

		public static ReferenceStep GetProperty(ITypeResolver typeResolver, Type type, MemberType propertyType, string name)
		{
			PlatformTypes platformMetadata = (PlatformTypes)typeResolver.PlatformMetadata;
			return (ReferenceStep)platformMetadata.GetProperty(typeResolver, type, propertyType, name);
		}

		protected virtual IList<IPropertyId> GetStyleProperties()
		{
			IList<IPropertyId> propertyIds = new List<IPropertyId>();
			Type runtimeType = null;
			foreach (IProperty property in this.Properties)
			{
				if (runtimeType == null)
				{
					runtimeType = property.DeclaringType.PlatformMetadata.ResolveType(PlatformTypes.Style).RuntimeType;
				}
				if (!runtimeType.IsAssignableFrom(PlatformTypeHelper.GetPropertyType(property)))
				{
					continue;
				}
				propertyIds.Add(property);
			}
			return propertyIds;
		}

		public virtual Type GetStylePropertyTargetType(IPropertyId propertyKey)
		{
			Type stylePropertyTargetType;
			try
			{
				PlatformTypes platformMetadata = this.typeResolver.PlatformMetadata as PlatformTypes;
				stylePropertyTargetType = platformMetadata.GetStylePropertyTargetType(this.BaseType, propertyKey);
			}
			catch (Exception exception)
			{
				return null;
			}
			return stylePropertyTargetType;
		}

		private static object GetValueFromAttribute(IType attributeType, ReferenceStep referenceStep, Type type, bool inherits)
		{
			object obj = null;
			PlatformTypes platformMetadata = (PlatformTypes)attributeType.PlatformMetadata;
			Attribute[] customAttributes = platformMetadata.GetCustomAttributes(type, attributeType.RuntimeType, inherits);
			int num = 0;
			if (num < (int)customAttributes.Length)
			{
				obj = customAttributes[num];
			}
			if (obj == null)
			{
				return null;
			}
			return referenceStep.GetValue(obj);
		}

		protected virtual void InitializeAlternateContentPropertiesIfNecessary()
		{
			if (!this.alternateContentPropertiesAreInitialized)
			{
				List<IPropertyId> propertyIds = new List<IPropertyId>();
				Type type = typeof(AlternateContentPropertyAttribute);
				foreach (IProperty property in this.Properties)
				{
					ReferenceStep referenceStep = property as ReferenceStep;
					if (referenceStep == null || referenceStep.Attributes == null || referenceStep.Attributes[type] == null && !PlatformNeutralAttributeHelper.AttributeExists(referenceStep.Attributes, PlatformTypes.AlternateContentPropertyAttribute))
					{
						continue;
					}
					propertyIds.Add(property);
				}
				this.alternateContentProperties = new ReadOnlyCollection<IPropertyId>(propertyIds);
			}
			this.alternateContentPropertiesAreInitialized = true;
		}

		private void InitializeContentPropertyIfNecessary()
		{
			bool flag;
			if (!this.contentPropertyIsInitialized)
			{
				this.contentPropertyIsInitialized = true;
				string nameFromNearestNonEmptyContentPropertyAttribute = ClrObjectMetadata.GetNameFromNearestNonEmptyContentPropertyAttribute(this.typeResolver.PlatformMetadata, this.baseType, out flag);
				if (!string.IsNullOrEmpty(nameFromNearestNonEmptyContentPropertyAttribute))
				{
					this.contentProperty = ClrObjectMetadata.GetContentProperty(this.typeResolver, this.baseType, nameFromNearestNonEmptyContentPropertyAttribute);
					if (this.contentProperty != null)
					{
						this.contentPropertyIsHidden = flag;
						return;
					}
				}
				else if (this.supportsInlineXml)
				{
					this.contentProperty = this.TypeResolver.ResolveProperty(this.typeResolver.PlatformMetadata.KnownProperties.DesignTimeInlineXml);
				}
			}
		}

		public bool IsNameProperty(IPropertyId propertyKey)
		{
			if (propertyKey == null)
			{
				return false;
			}
			if (DesignTimeProperties.XNameProperty.Equals(propertyKey))
			{
				return true;
			}
			return propertyKey.Equals(((ITypeMetadata)this).NameProperty);
		}
	}
}