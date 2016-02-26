// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.EditingPropertyManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class EditingPropertyManager
  {
    public static readonly DependencyProperty EditingPropertyManagerProperty = DependencyProperty.RegisterAttached("EditingPropertyManager", typeof (EditingPropertyManager), typeof (FrameworkElement));
    private FrameworkElement hostControl;
    private PropertyReferenceProperty editingProperty;
    private IEnumerable<ReferenceStepPropertyInformation> properties;
    private SceneNodeProperty targetNameProperty;
    private SceneNodeProperty targetObjectProperty;

    public IPropertyInformation CurrentProperty
    {
      get
      {
        if (this.editingProperty != null && this.properties != null)
        {
          string propertyName = this.editingProperty.GetValue() as string;
          if (propertyName != null)
          {
            IPropertyInformation propertyInformation = (IPropertyInformation) Enumerable.FirstOrDefault<ReferenceStepPropertyInformation>(this.properties, (Func<ReferenceStepPropertyInformation, bool>) (item => item.Name == propertyName));
            if (propertyInformation == null)
            {
              IType propertyType = this.editingProperty.ObjectSet.ProjectContext.ResolveType(PlatformTypes.Object);
              propertyInformation = (IPropertyInformation) new FallbackPropertyInformation(propertyName, propertyType);
            }
            return propertyInformation;
          }
        }
        return (IPropertyInformation) null;
      }
      set
      {
        using (SceneEditTransaction editTransaction = ((SceneNodeObjectSet) this.editingProperty.ObjectSet).CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
        {
          (object) this.editingProperty.PropertyName
        })))
        {
          this.editingProperty.SetValue((object) value.Name);
          if (this.EditingPropertyChanged != null)
            this.EditingPropertyChanged((object) this, EventArgs.Empty);
          editTransaction.Commit();
        }
      }
    }

    public IEnumerable<ReferenceStepPropertyInformation> Properties
    {
      get
      {
        return this.properties;
      }
    }

    public PropertyReferenceProperty EditingProperty
    {
      get
      {
        return this.editingProperty;
      }
    }

    public event EventHandler Rebuilt;

    public event EventHandler EditingPropertyChanged;

    public EditingPropertyManager(FrameworkElement hostControl)
    {
      this.hostControl = hostControl;
      this.hostControl.Loaded += new RoutedEventHandler(this.OnPropertyPickerEditorLoaded);
      this.hostControl.Unloaded += new RoutedEventHandler(this.OnPropertyPickerEditorUnloaded);
      this.hostControl.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnPropertyPickerEditorDataContextChanged);
    }

    private void OnPropertyPickerEditorLoaded(object sender, RoutedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void OnPropertyPickerEditorUnloaded(object sender, RoutedEventArgs e)
    {
      this.UnhookEditingProperty();
      this.Rebuild();
    }

    private void OnPropertyPickerEditorDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.UpdateFromDataContext();
    }

    private void UnhookEditingProperty()
    {
      if (this.editingProperty != null)
      {
        this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
        this.editingProperty = (PropertyReferenceProperty) null;
      }
      if (this.targetNameProperty != null)
      {
        this.targetNameProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
        this.targetNameProperty.OnRemoveFromCategory();
        this.targetNameProperty = (SceneNodeProperty) null;
      }
      if (this.targetObjectProperty == null)
        return;
      this.targetObjectProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
      this.targetObjectProperty.OnRemoveFromCategory();
      this.targetObjectProperty = (SceneNodeProperty) null;
    }

    private void UpdateFromDataContext()
    {
      this.UnhookEditingProperty();
      Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue = this.hostControl.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      if (propertyValue != null)
        this.editingProperty = (PropertyReferenceProperty) propertyValue.ParentProperty;
      if (this.editingProperty == null)
        return;
      this.editingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
      SceneNodeObjectSetBase nodeObjectSetBase = (SceneNodeObjectSetBase) this.editingProperty.ObjectSet;
      ReferenceStep singleStep1 = (ReferenceStep) nodeObjectSetBase.ProjectContext.ResolveProperty(BehaviorTargetedTriggerActionNode.BehaviorTargetNameProperty);
      if (singleStep1 != null)
      {
        this.targetNameProperty = (SceneNodeProperty) nodeObjectSetBase.CreateProperty(new PropertyReference(singleStep1), (AttributeCollection) null);
        this.targetNameProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
      }
      ReferenceStep singleStep2 = (ReferenceStep) nodeObjectSetBase.ProjectContext.ResolveProperty(BehaviorTargetedTriggerActionNode.BehaviorTargetObjectProperty);
      if (singleStep2 != null)
      {
        this.targetObjectProperty = (SceneNodeProperty) nodeObjectSetBase.CreateProperty(new PropertyReference(singleStep2), (AttributeCollection) null);
        this.targetObjectProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnEditingPropertyChanged);
      }
      this.Rebuild();
    }

    private void OnEditingPropertyChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    public void Rebuild()
    {
      SceneNodeProperty sceneNodeProperty = this.editingProperty as SceneNodeProperty;
      BehaviorTargetedTriggerActionNode triggerActionNode = sceneNodeProperty != null ? sceneNodeProperty.SceneNodeObjectSet.RepresentativeSceneNode as BehaviorTargetedTriggerActionNode : (BehaviorTargetedTriggerActionNode) null;
      this.properties = triggerActionNode == null || triggerActionNode.TargetNode == null ? (IEnumerable<ReferenceStepPropertyInformation>) null : ReferenceStepPropertyInformation.GetPropertiesForType(triggerActionNode.TargetNode.TrueTargetTypeId);
      if (this.Rebuilt == null)
        return;
      this.Rebuilt((object) this, EventArgs.Empty);
    }
  }
}
