// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyPickerExtendedEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class PropertyPickerExtendedEditor : UserControl
  {
    private PropertyContainer propertyContainer;
    private EditingPropertyManager contextHelper;
    private SceneNodeProperty targetPropertyEntry;

    public PropertyPickerExtendedEditor()
    {
      this.propertyContainer = new PropertyContainer();
      this.Content = this.propertyContainer;
      this.Loaded += new RoutedEventHandler(this.OnPropertyPickerExtendedEditor);
    }

    private void OnPropertyPickerExtendedEditor(object sender, RoutedEventArgs e)
    {
      PropertyContainer propertyContainer = (PropertyContainer) this.GetValue((DependencyProperty) PropertyContainer.OwningPropertyContainerProperty);
      if (propertyContainer == null)
        return;
      this.contextHelper = (EditingPropertyManager) ((DependencyObject) propertyContainer).GetValue(EditingPropertyManager.EditingPropertyManagerProperty);
      this.contextHelper.Rebuilt += new EventHandler(this.Rebuild);
      this.contextHelper.EditingPropertyChanged += new EventHandler(this.EditingPropertyChanged);
      this.contextHelper.Rebuild();
    }

    private void EditingPropertyChanged(object sender, EventArgs e)
    {
      if (this.targetPropertyEntry != null)
      {
        object defaultValue = this.contextHelper.CurrentProperty.DefaultValue;
        if (defaultValue != null)
          this.targetPropertyEntry.SetValue(defaultValue);
        else
          this.targetPropertyEntry.ClearValue();
      }
      this.Rebuild(this, EventArgs.Empty);
    }

    private void Rebuild(object sender, EventArgs args)
    {
      if (this.targetPropertyEntry != null)
      {
        this.targetPropertyEntry.OnRemoveFromCategory();
        this.targetPropertyEntry = (SceneNodeProperty) null;
      }
      if (this.contextHelper.EditingProperty != null && this.contextHelper.CurrentProperty != null)
      {
        SceneNodeObjectSetBase nodeObjectSetBase = (SceneNodeObjectSetBase) this.contextHelper.EditingProperty.ObjectSet;
        ReferenceStep referenceStep = (ReferenceStep) nodeObjectSetBase.ProjectContext.ResolveProperty(ChangePropertyActionNode.ValueProperty);
        PropertyPickerExtendedEditor.WrapperObjectSet wrapperObjectSet = new PropertyPickerExtendedEditor.WrapperObjectSet((SceneNodeObjectSet) nodeObjectSetBase, referenceStep, this.contextHelper.CurrentProperty);
        this.targetPropertyEntry = (SceneNodeProperty) new PropertyPickerExtendedEditor.WrapperTypedSceneNodeProperty((SceneNodeObjectSet) wrapperObjectSet, new PropertyReference(referenceStep), this.contextHelper.CurrentProperty.Name, this.contextHelper.CurrentProperty.Attributes, this.contextHelper.CurrentProperty.PropertyType.RuntimeType, (ITypeResolver) nodeObjectSetBase.ProjectContext);
        this.targetPropertyEntry.Recache();
        wrapperObjectSet.TargetProperty = this.targetPropertyEntry;
        this.propertyContainer.set_PropertyEntry((PropertyEntry) this.targetPropertyEntry);
        if (this.propertyContainer.get_ExtendedEditorTemplate() == null)
          return;
        this.propertyContainer.set_ActiveEditMode((PropertyContainerEditMode) 2);
      }
      else
        this.propertyContainer.set_PropertyEntry((PropertyEntry) null);
    }

    private class WrapperTypedSceneNodeProperty : TypedSceneNodeProperty
    {
      public override bool IsEnabledDatabind
      {
        get
        {
          if (this.SceneNodeObjectSet.Count != 1)
            return false;
          return BindingPropertyHelper.IsBindableType(this.SceneNodeObjectSet.Objects[0].Type);
        }
      }

      public WrapperTypedSceneNodeProperty(SceneNodeObjectSet objectSet, PropertyReference propertyReference, string propertyName, AttributeCollection attributeCollection, Type valueType, ITypeResolver typeResolver)
        : base(objectSet, propertyReference, attributeCollection, valueType, typeResolver)
      {
        this.InitializeConverter(propertyName);
      }
    }

    private class WrapperObjectSet : SceneNodeObjectSetBase
    {
      private SceneNodeObjectSet baseSet;
      private ReferenceStep targetStep;
      private IPropertyInformation redirectedProperty;

      public SceneNodeProperty TargetProperty { get; set; }

      public override bool ShouldWalkParentsForGetValue
      {
        get
        {
          return this.baseSet.ShouldWalkParentsForGetValue;
        }
      }

      public override bool ShouldAllowAnimation
      {
        get
        {
          return this.baseSet.ShouldAllowAnimation;
        }
      }

      public override SceneDocument Document
      {
        get
        {
          return this.baseSet.Document;
        }
      }

      public override IDocumentContext DocumentContext
      {
        get
        {
          return this.baseSet.DocumentContext;
        }
      }

      public override IProjectContext ProjectContext
      {
        get
        {
          return this.baseSet.ProjectContext;
        }
      }

      public override SceneViewModel ViewModel
      {
        get
        {
          return this.baseSet.ViewModel;
        }
      }

      public override bool IsValidForUpdate
      {
        get
        {
          return this.baseSet.IsValidForUpdate;
        }
      }

      public override SceneNode[] Objects
      {
        get
        {
          return this.baseSet.Objects;
        }
      }

      public override bool IsHomogenous
      {
        get
        {
          return this.baseSet.IsHomogenous;
        }
      }

      public WrapperObjectSet(SceneNodeObjectSet baseSet, ReferenceStep targetStep, IPropertyInformation redirectedProperty)
        : base(baseSet.DesignerContext, baseSet.TransactionContext)
      {
        this.baseSet = baseSet;
        this.targetStep = targetStep;
        this.redirectedProperty = redirectedProperty;
      }

      public override object GetValue(PropertyReference propertyReference, PropertyReference.GetValueFlags getValueFlags)
      {
        if (propertyReference[0] == this.targetStep)
        {
          TypeConverter typeConverter = this.redirectedProperty.TypeConverter;
          object target = base.GetValue(new PropertyReference(this.targetStep), getValueFlags);
          if (target == null)
          {
            Type runtimeType = this.redirectedProperty.PropertyType.RuntimeType;
            if (runtimeType != (Type) null && runtimeType.IsValueType)
              target = Activator.CreateInstance(runtimeType);
          }
          if (target != null && typeConverter != null && typeConverter.CanConvertFrom(target.GetType()))
            target = typeConverter.ConvertFrom(target);
          if (target != null)
          {
            if (propertyReference.Count > 1)
              return propertyReference.PartialGetValue(target, 1, propertyReference.Count - 1);
            return target;
          }
        }
        return base.GetValue(propertyReference, getValueFlags);
      }

      protected override void ModifyValue(PropertyReferenceProperty property, object valueToSet, Modification modification, int index)
      {
        if (property.Reference[0] == this.targetStep && property.Reference.Count > 1 && !(valueToSet is DocumentNode))
        {
          object obj = this.GetValue((IPropertyId) this.targetStep);
          if (obj == null && PlatformTypes.Transform.IsAssignableFrom((ITypeId) this.redirectedProperty.PropertyType))
            obj = new CanonicalTransform().GetPlatformTransform(this.ProjectContext.Platform.GeometryHelper);
          if (modification == Modification.InsertValue)
            obj = property.Reference.PartialAdd(obj, index, valueToSet, 1, property.Reference.Count - 1);
          else if (modification == Modification.SetValue)
            obj = property.Reference.PartialSetValue(obj, valueToSet, 1, property.Reference.Count - 1);
          else if (modification == Modification.ClearValue)
            property.Reference.PartialClearValue(obj, 1, property.Reference.Count - 1);
          else if (modification == Modification.RemoveValue)
            obj = property.Reference.PartialRemoveAt(obj, 1, property.Reference.Count - 1, index);
          base.ModifyValue((PropertyReferenceProperty) this.TargetProperty, obj, Modification.SetValue, -1);
        }
        else
        {
          if (valueToSet != null && valueToSet.GetType().IsPrimitive)
            valueToSet = valueToSet.ToString();
          base.ModifyValue(property, valueToSet, modification, index);
        }
      }

      protected override ObservableCollectionWorkaround<LocalResourceModel> RecalculateLocalResources(ObservableCollectionWorkaround<LocalResourceModel> currentResources)
      {
        return (ObservableCollectionWorkaround<LocalResourceModel>) this.baseSet.LocalResources;
      }

      public override void RegisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
      {
        this.baseSet.RegisterPropertyChangedHandler(propertyReference, handler);
      }

      public override void UnregisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
      {
        this.baseSet.UnregisterPropertyChangedHandler(propertyReference, handler);
      }
    }
  }
}
