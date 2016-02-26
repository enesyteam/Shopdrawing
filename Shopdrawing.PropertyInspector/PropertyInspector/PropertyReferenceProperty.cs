// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyReferenceProperty
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Expression.Project;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class PropertyReferenceProperty : PropertyBase
  {
    private static ArrayList emptyStandardValuesList = new ArrayList(0);
    private ObjectSetBase objectSet;
    private PropertyReference propertyReference;
    private TypeConverter converter;
    private AttributeCollection attributes;
    private ValueEditorParameters valueEditorParameters;
    private Type proxyType;
    private PropertyValueEditor propertyValueEditor;
    private Type propertyValueEditorTypeCache;

    public AttributeCollection Attributes
    {
      get
      {
        return this.attributes;
      }
    }

    public PropertyOrderWrapper PropertyOrder
    {
      get
      {
        Microsoft.Windows.Design.PropertyEditing.PropertyOrder propertyOrder = Microsoft.Windows.Design.PropertyEditing.PropertyOrder.get_Default();
        if (this.attributes != null)
        {
          PropertyOrderAttribute propertyOrderAttribute = this.attributes[typeof (PropertyOrderAttribute)] as PropertyOrderAttribute;
          if (propertyOrderAttribute != null)
            propertyOrder = propertyOrderAttribute.get_Order();
        }
        return new PropertyOrderWrapper(propertyOrder);
      }
    }

    public override ValueEditorParameters ValueEditorParameters
    {
      get
      {
        return this.valueEditorParameters;
      }
    }

    public virtual EditingContext Context
    {
      get
      {
        return (EditingContext) null;
      }
    }

    public virtual PropertyIdentifier Identifier
    {
      get
      {
        return new PropertyIdentifier(this.ObjectSet.ObjectType, base.get_PropertyName());
      }
    }

    public IType DeclaringTypeId
    {
      get
      {
        return this.Reference.FirstStep.DeclaringType;
      }
    }

    public override TypeConverter Converter
    {
      get
      {
        return this.converter;
      }
    }

    public IType ComputedValueTypeId
    {
      get
      {
        object obj = this.GetValue();
        if (obj == null)
          return (IType) null;
        return this.objectSet.ProjectContext.GetType(obj.GetType());
      }
    }

    public ObjectSetBase ObjectSet
    {
      get
      {
        return this.objectSet;
      }
    }

    public bool IsEmpty
    {
      get
      {
        if (this.objectSet != null)
          return this.objectSet.Count == 0;
        return true;
      }
    }

    public virtual PropertyValueEditor PropertyValueEditor
    {
      get
      {
        Type propertyOwnerType = this.PropertyOwnerType;
        if (propertyOwnerType != this.propertyValueEditorTypeCache)
        {
          this.propertyValueEditorTypeCache = propertyOwnerType;
          this.propertyValueEditor = ExtensibilityMetadataHelper.GetValueEditor((IEnumerable) this.attributes, this.objectSet.DesignerContext.MessageLoggingService);
          if (this.propertyValueEditor == null)
          {
            this.propertyValueEditor = BehaviorHelper.GetCustomPropertyValueEditor(this);
            if (this.propertyValueEditor == null)
              this.propertyValueEditor = PropertyEditorTemplateLookup.GetPropertyEditorTemplate(this);
          }
        }
        return this.propertyValueEditor;
      }
    }

    private Type PropertyOwnerType
    {
      get
      {
        Type type = this.ObjectSet.ObjectType;
        if (type == (Type) null || this.Reference.LastStep.IsAttachable)
          type = PlatformTypeHelper.GetDeclaringType((IMember) this.Reference.FirstStep);
        return type;
      }
    }

    public PropertyReference Reference
    {
      get
      {
        return this.propertyReference;
      }
    }

    public virtual IEnumerable<ModelProperty> ModelProperties
    {
      get
      {
        return Enumerable.Empty<ModelProperty>();
      }
    }

    public virtual bool IsDefaultValue
    {
      get
      {
        throw new NotImplementedException(ExceptionStringTable.ThisMethodIsNotCurrentlyImplemented);
      }
    }

    public bool IsMixedValue
    {
      get
      {
        object obj;
        try
        {
          obj = this.GetValue();
        }
        catch
        {
          return false;
        }
        return obj == MixedProperty.Mixed;
      }
    }

    public virtual bool IsReadOnly
    {
      get
      {
        return this.Reference.LastStep.WriteAccess != MemberAccessType.Public;
      }
    }

    public virtual ICollection StandardValues
    {
      get
      {
        ICollection collection = (ICollection) null;
        if (this.Converter.GetStandardValuesSupported())
          collection = this.Converter.GetStandardValues();
        if (collection == null || collection.Count <= 0)
          return (ICollection) PropertyReferenceProperty.emptyStandardValuesList;
        if (!JoltHelper.TypeHasEnumValues(this.PropertyTypeId) || this.objectSet.ProjectContext == null)
          return collection;
        HashSet<object> hashSet = new HashSet<object>();
        foreach (object obj in (IEnumerable) collection)
        {
          if (!hashSet.Contains(obj))
            hashSet.Add(obj);
        }
        return (ICollection) Enumerable.ToList<object>((IEnumerable<object>) hashSet);
      }
    }

    public bool HasProxyPropertyType
    {
      get
      {
        return this.proxyType != (Type) null;
      }
    }

    public Type ProxyPropertyType
    {
      get
      {
        if (this.proxyType != (Type) null && !this.get_PropertyValue().get_IsMixedValue())
          return this.proxyType;
        return base.get_PropertyType();
      }
    }

    public virtual Type PropertyType
    {
      get
      {
        return PlatformTypeHelper.GetPropertyType((IProperty) this.Reference.LastStep);
      }
    }

    public virtual IType PropertyTypeId
    {
      get
      {
        return this.Reference.LastStep.PropertyType;
      }
    }

    public virtual string CategoryName
    {
      get
      {
        throw new NotImplementedException(ExceptionStringTable.ThisMethodIsNotCurrentlyImplemented);
      }
    }

    public virtual bool IsAdvanced
    {
      get
      {
        throw new NotImplementedException(ExceptionStringTable.ThisMethodIsNotCurrentlyImplemented);
      }
    }

    public virtual string Description
    {
      get
      {
        string result;
        if (PlatformNeutralAttributeHelper.TryGetAttributeValue<string>((IEnumerable) this.Attributes, PlatformTypes.DescriptionAttribute, "Description", out result) && !string.IsNullOrEmpty(result))
          return result;
        return DescriptionAttribute.Default.Description;
      }
    }

    public virtual bool ShouldClearValueWhenSettingNull
    {
      get
      {
        return true;
      }
    }

    public event Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler PropertyReferenceChanged;

    public PropertyReferenceProperty(ObjectSetBase objectSet, PropertyReference propertyReference, AttributeCollection attributes)
      : this(objectSet, propertyReference, attributes, (PropertyValue) null, (Type) null)
    {
    }

    public PropertyReferenceProperty(ObjectSetBase objectSet, PropertyReference propertyReference, AttributeCollection attributes, PropertyValue parentValue, Type proxyType)
      : base(parentValue)
    {
      if (attributes == null)
      {
        DependencyPropertyReferenceStep propertyReferenceStep = propertyReference.LastStep as DependencyPropertyReferenceStep;
        if (propertyReferenceStep != null)
          attributes = propertyReferenceStep.Attributes;
      }
      this.proxyType = proxyType;
      this.attributes = attributes;
      this.objectSet = objectSet;
      this.propertyReference = propertyReference;
      if (propertyReference.LastStep.IsAttachable)
        this.SetName(propertyReference.LastStep.DeclaringType.Name + "." + propertyReference.LastStep.Name);
      else
        this.SetName(propertyReference.LastStep.Name);
      this.InitializeConverter(base.get_PropertyName());
      this.InitializeValueEditorParameters();
      if (!JoltHelper.TypeHasEnumValues(this.PropertyTypeId))
        return;
      this.objectSet.DesignerContext.ProjectManager.ProjectOpened += new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectOpened);
    }

    protected void InitializeConverter(string propertyName)
    {
      if ((propertyName == "Content" || propertyName == "Header" || (propertyName == "ToolTip" || propertyName == "Tag") || propertyName == "ToolTipService.ToolTip") && base.get_PropertyType().IsAssignableFrom(typeof (object)))
      {
        this.converter = (TypeConverter) new StringConverter();
      }
      else
      {
        if (this.attributes != null)
          this.converter = Microsoft.Expression.DesignModel.Metadata.MetadataStore.GetTypeConverterFromAttributes(base.get_PropertyType() != (Type) null ? base.get_PropertyType().Assembly : (Assembly) null, this.attributes);
        if (this.converter != null && (!(this.proxyType != (Type) null) || this.converter.CanConvertFrom(this.proxyType)))
          return;
        this.converter = (TypeConverter) null;
        if (this.proxyType == (Type) null || PlatformTypeHelper.GetPropertyType((IProperty) this.propertyReference.LastStep).IsAssignableFrom(this.proxyType))
          this.converter = this.propertyReference.LastStep.TypeConverter;
        if (this.converter != null)
          return;
        if (this.proxyType != (Type) null)
          this.converter = this.Reference.LastStep.DeclaringType.PlatformMetadata.GetTypeConverter((MemberInfo) this.proxyType);
        else
          this.converter = this.Reference.LastStep.DeclaringType.PlatformMetadata.GetTypeConverter((MemberInfo) PlatformTypeHelper.GetPropertyType((IProperty) this.Reference.LastStep));
      }
    }

    private void ProjectManager_ProjectOpened(object sender, ProjectEventArgs e)
    {
      this.OnPropertyChanged("StandardValues");
    }

    internal virtual bool UpdateAndRefresh(PropertyReference propertyReference, AttributeCollection attributes, Type proxyType)
    {
      bool flag1 = !object.Equals(this.propertyReference, (object) propertyReference);
      bool force = this.propertyReference.PlatformMetadata != propertyReference.PlatformMetadata;
      bool flag2 = force;
      if (flag1)
        this.propertyReference = propertyReference;
      if (this.proxyType != proxyType)
      {
        flag2 = true;
        this.proxyType = proxyType;
      }
      if (this.attributes != attributes)
      {
        flag2 = true;
        this.attributes = attributes;
        this.InitializeValueEditorParameters();
      }
      if (flag2)
      {
        this.InitializeConverter(base.get_PropertyName());
        this.Recache();
        this.Associated = true;
        this.RefreshPropertyValueEditor(force);
      }
      if (flag1)
        this.OnPropertyReferenceChanged(new PropertyReferenceChangedEventArgs(SceneViewModel.ViewStateBits.EntireScene, this.Reference));
      return flag2;
    }

    private void RefreshPropertyValueEditor(bool force)
    {
      if (!force && this.PropertyOwnerType == this.propertyValueEditorTypeCache)
        return;
      this.OnPropertyChanged("PropertyValueEditor");
    }

    public void OverrideValueEditorParameters(ValueEditorParameters newParams)
    {
      this.valueEditorParameters = newParams;
    }

    protected virtual void ModifyValue(object valueToSet, PropertyReferenceProperty.Modification modification, int index)
    {
      switch (modification)
      {
        case PropertyReferenceProperty.Modification.SetValue:
          this.objectSet.SetValue(this, valueToSet);
          break;
        case PropertyReferenceProperty.Modification.ClearValue:
          this.objectSet.ClearValue(this);
          break;
        case PropertyReferenceProperty.Modification.InsertValue:
          this.objectSet.InsertValue(this, index, valueToSet);
          break;
        case PropertyReferenceProperty.Modification.RemoveValue:
          this.objectSet.RemoveValueAt(this, index);
          break;
        case PropertyReferenceProperty.Modification.AddValue:
          this.objectSet.AddValue(this, valueToSet);
          break;
      }
    }

    public override void SetValue(object value)
    {
      this.ModifyValue(value, PropertyReferenceProperty.Modification.SetValue, -1);
    }

    public override void ClearValue()
    {
      this.ModifyValue(null, PropertyReferenceProperty.Modification.ClearValue, -1);
    }

    public override object GetValue()
    {
      return this.objectSet.GetValue(this);
    }

    public override void AddValue(object value)
    {
      this.ModifyValue(value, PropertyReferenceProperty.Modification.AddValue, -1);
    }

    public override void InsertValue(int index, object value)
    {
      this.ModifyValue(value, PropertyReferenceProperty.Modification.InsertValue, index);
    }

    public override void RemoveValueAt(int index)
    {
      this.ModifyValue(null, PropertyReferenceProperty.Modification.RemoveValue, index);
    }

    public override void OnRemoveFromCategory()
    {
      if (!JoltHelper.TypeHasEnumValues(this.PropertyTypeId) || this.objectSet.DesignerContext.ProjectManager == null)
        return;
      this.objectSet.DesignerContext.ProjectManager.ProjectOpened -= new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectOpened);
    }

    protected virtual void OnPropertyReferenceChanged(PropertyReferenceChangedEventArgs e)
    {
      if (this.PropertyReferenceChanged == null)
        return;
      this.PropertyReferenceChanged(this, e);
    }

    protected virtual PropertyValue CreatePropertyValueInstance()
    {
      return (PropertyValue) new PropertyReferencePropertyValue(this);
    }

    public T GetAttribute<T>(bool useTypeAttributes) where T : Attribute
    {
      if (this.Attributes != null)
        return (T) this.Attributes[typeof (T)];
      if (useTypeAttributes)
      {
        foreach (Attribute attribute in TypeUtilities.GetAttributes(this.Reference.LastStep.TargetType))
        {
          T obj = attribute as T;
          if ((object) obj != null)
            return obj;
        }
      }
      return default (T);
    }

    public IList<T> GetAttributes<T>(bool useTypeAttributes) where T : Attribute
    {
      List<T> list = new List<T>();
      AttributeCollection attributes = this.Attributes;
      if (attributes == null && useTypeAttributes)
        attributes = TypeUtilities.GetAttributes(this.Reference.LastStep.TargetType);
      if (attributes != null)
      {
        foreach (Attribute attribute in attributes)
        {
          T obj = attribute as T;
          if ((object) obj != null)
            list.Add(obj);
        }
      }
      return (IList<T>) list;
    }

    private void InitializeValueEditorParameters()
    {
      this.valueEditorParameters = this.Attributes == null ? (ValueEditorParameters) null : new ValueEditorParameters(this.Attributes);
      this.OnPropertyChanged("ValueEditorParameters");
    }

    public virtual string ToString()
    {
      return this.propertyReference.ToString();
    }

    protected enum Modification
    {
      SetValue,
      ClearValue,
      InsertValue,
      RemoveValue,
      AddValue,
    }
  }
}
