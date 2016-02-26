// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.TransformEditorBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  public class TransformEditorBase : Grid, INotifyPropertyChanged
  {
    internal DesignerContext designerContext;
    private bool isRelativeTransform;

    protected SceneNodeProperty TransformProperty { get; set; }

    protected TransformPropertyLookup PropertyLookup { get; set; }

    protected Type ComponentType { get; set; }

    protected SceneNodeObjectSet ObjectSet { get; set; }

    public bool IsRelativeTransform
    {
      get
      {
        return this.isRelativeTransform;
      }
      set
      {
        if (value == this.isRelativeTransform)
          return;
        this.isRelativeTransform = value;
        this.UpdateModel();
        this.OnPropertyChanged("IsRelativeTransform");
      }
    }

    protected virtual ItemCollection TabCollection
    {
      get
      {
        return (ItemCollection) null;
      }
    }

    protected virtual bool CurrentTabSupportsRelativeMode
    {
      get
      {
        return true;
      }
    }

    public ICommand ApplyRelativeTransformCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ApplyRelativeTransform_Execute));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public TransformEditorBase()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.TransformEditorBase_DataContextChanged);
      this.Loaded += new RoutedEventHandler(this.ProjectionEditor_Loaded);
      this.Unloaded += new RoutedEventHandler(this.ProjectionEditor_Unloaded);
    }

    private void ProjectionEditor_Unloaded(object sender, RoutedEventArgs e)
    {
      this.Unhook();
    }

    private void ProjectionEditor_Loaded(object sender, RoutedEventArgs e)
    {
      this.Rehook();
    }

    private void TransformEditorBase_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) null;
      if (propertyValue != null)
        sceneNodeProperty = (SceneNodeProperty) propertyValue.ParentProperty;
      if (sceneNodeProperty == this.TransformProperty)
        return;
      this.Unhook();
      this.TransformProperty = sceneNodeProperty;
      if (this.TransformProperty == null)
        return;
      this.TransformProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.transformProperty_PropertyReferenceChanged);
      this.designerContext = this.TransformProperty.SceneNodeObjectSet.DesignerContext;
      TransformType transformType = TransformType.Transform2D;
      if (PlatformTypes.Transform3D.IsAssignableFrom((ITypeId) this.TransformProperty.PropertyTypeId))
        transformType = TransformType.Transform3D;
      else if (PlatformTypes.Transform.IsAssignableFrom((ITypeId) this.TransformProperty.PropertyTypeId))
        transformType = TransformType.Transform2D;
      else if (PlatformTypes.Projection.IsAssignableFrom((ITypeId) this.TransformProperty.PropertyTypeId))
      {
        transformType = TransformType.PlaneProjection;
      }
      else
      {
        IType type = this.TransformProperty.SceneNodeObjectSet.ProjectContext.GetType(this.TransformProperty.GetValue().GetType());
        if (PlatformTypes.Transform3D.IsAssignableFrom((ITypeId) type))
          transformType = TransformType.Transform3D;
        else if (PlatformTypes.Transform.IsAssignableFrom((ITypeId) type))
          transformType = TransformType.Transform2D;
        else if (PlatformTypes.Projection.IsAssignableFrom((ITypeId) type))
          transformType = TransformType.PlaneProjection;
      }
      this.ObjectSet = this.TransformProperty.SceneNodeObjectSet;
      this.PropertyLookup = new TransformPropertyLookup(this.TransformProperty, transformType);
      this.ComponentType = this.PropertyLookup.CreateDefaultRelativeTransform().GetType();
      this.Initialize();
      foreach (FrameworkElement frameworkElement in (IEnumerable) this.TabCollection)
        frameworkElement.DataContext = (object) this;
      this.UpdateModel();
    }

    protected virtual void Initialize()
    {
    }

    protected virtual void Unhook()
    {
      if (this.TransformProperty != null)
      {
        this.TransformProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.transformProperty_PropertyReferenceChanged);
        this.TransformProperty = (SceneNodeProperty) null;
      }
      if (this.PropertyLookup == null)
        return;
      this.PropertyLookup.Unload();
    }

    protected virtual void Rehook()
    {
      if (this.PropertyLookup == null)
        return;
      this.PropertyLookup.Unload();
      foreach (ContentControl contentControl in (IEnumerable) this.TabCollection)
      {
        ITransformEditorTab transformEditorTab = contentControl.Content as ITransformEditorTab;
        if (transformEditorTab != null)
          transformEditorTab.UpdatePropertyContainers(this.PropertyLookup);
      }
    }

    protected virtual void UpdateModel()
    {
      this.PropertyLookup.UpdateEditMode(this.IsRelativeTransform);
      this.Rehook();
    }

    private void transformProperty_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      if (this.PropertyLookup == null)
        return;
      this.PropertyLookup.RecacheProperties();
    }

    protected void TransformEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.IsRelativeTransform && !this.CurrentTabSupportsRelativeMode)
        this.IsRelativeTransform = false;
      if (!this.IsRelativeTransform)
        return;
      LocalValueObjectSet localValueObjectSet = this.PropertyLookup.ActiveObjectSet as LocalValueObjectSet;
      if (localValueObjectSet == null)
        return;
      localValueObjectSet.LocalValue = this.PropertyLookup.CreateDefaultRelativeTransform();
      this.PropertyLookup.RecacheProperties();
    }

    private bool ComposesByMultiplication(PropertyReferenceProperty property, object defaultTransform)
    {
      return (double) property.Reference.GetCurrentValue(defaultTransform) == 1.0;
    }

    private void ApplyRelativeTransform_Execute()
    {
      if (!(this.ComponentType != (Type) null) || !this.IsRelativeTransform || this.ObjectSet.RepresentativeSceneNode == null)
        return;
      SelectionManagerPerformanceHelper.MeasurePerformanceUntilPipelinePostSceneUpdate(this.designerContext.SelectionManager, PerformanceEvent.EditSingleTransformProperty);
      using (SceneEditTransaction editTransaction = this.ObjectSet.ViewModel.CreateEditTransaction(StringTable.UndoUnitTransformPaneRelativeTransform))
      {
        LocalValueObjectSet localValueObjectSet = (LocalValueObjectSet) this.PropertyLookup.ActiveObjectSet;
        foreach (SceneNode sceneNode in this.ObjectSet.Objects)
        {
          object currentTransform;
          if (this.PropertyLookup.TransformType == TransformType.Transform2D)
          {
            if (this.PropertyLookup.IsCompositeSupported)
            {
              currentTransform = sceneNode.GetComputedValue(this.TransformProperty.Reference);
              if (currentTransform != null && !PlatformTypes.CompositeTransform.IsAssignableFrom((ITypeId) this.ObjectSet.ProjectContext.GetType(currentTransform.GetType())))
                currentTransform = (object) null;
            }
            else
              currentTransform = sceneNode.GetComputedValueAsWpf(this.TransformProperty.Reference);
          }
          else
            currentTransform = sceneNode.GetComputedValue(this.TransformProperty.Reference);
          object valueToSet = this.ApplyRelativeTransform(localValueObjectSet.LocalValue, currentTransform);
          if (sceneNode is Base2DElement && this.TransformProperty.Reference.LastStep.Equals((object) Base2DElement.RenderTransformProperty) && sceneNode.IsSet(Base2DElement.RenderTransformOriginProperty).Equals((object) PropertyState.Unset))
            sceneNode.SetValueAsWpf(Base2DElement.RenderTransformOriginProperty, (object) new Point(0.5, 0.5));
          switch (this.PropertyLookup.TransformType)
          {
            case TransformType.Transform2D:
              if (this.PropertyLookup.IsCompositeSupported)
              {
                sceneNode.SetValue(this.TransformProperty.Reference, valueToSet);
                break;
              }
              sceneNode.SetValueAsWpf(this.TransformProperty.Reference, (object) ((ICanonicalTransform) valueToSet).TransformGroup);
              break;
            case TransformType.Transform3D:
              sceneNode.SetValue(this.TransformProperty.Reference, (object) ((CanonicalTransform3D) valueToSet).ToTransform());
              break;
            case TransformType.PlaneProjection:
              sceneNode.SetValue(this.TransformProperty.Reference, valueToSet);
              break;
          }
        }
        editTransaction.Commit();
      }
    }

    private object ApplyRelativeTransform(object relativeTransform, object currentTransform)
    {
      object instance1 = Activator.CreateInstance(this.ComponentType);
      object instance2 = Activator.CreateInstance(this.ComponentType);
      object target = instance2;
      if (currentTransform != null)
      {
        if (this.PropertyLookup.TransformType == TransformType.PlaneProjection || this.PropertyLookup.IsCompositeSupported)
          target = currentTransform;
        else
          target = Activator.CreateInstance(this.ComponentType, new object[1]
          {
            currentTransform
          });
      }
      foreach (PropertyReferenceProperty property in this.PropertyLookup.ActiveProperties)
      {
        if (property != null && property.Reference.FirstStep.TargetType == this.ComponentType)
        {
          if (property.PropertyType == typeof (double) && !(property.Reference.ShortPath == "RotationAngleX") && (!(property.Reference.ShortPath == "RotationAngleY") && !(property.Reference.ShortPath == "RotationAngleZ")))
          {
            double num1 = (double) property.Reference.GetValue(relativeTransform);
            double num2 = (double) property.Reference.GetValue(target);
            double num3 = (double) property.Reference.GetValue(instance2);
            double num4 = !this.ComposesByMultiplication(property, instance2) ? num2 + (num1 - num3) : num2 * num1;
            double num5 = !property.PropertyName.Contains("Angle") ? (!property.PropertyName.Contains("Scale") ? RoundingHelper.RoundLength(num4) : RoundingHelper.RoundScale(num4)) : RoundingHelper.RoundAngle(num4);
            if (num5 != num3)
              property.Reference.SetValue(instance1, (object) num5);
          }
          else if (property.Reference.ShortPath == "RotationAngles")
          {
            Quaternion orientation = Helper3D.QuaternionFromEulerAngles((Vector3D) property.Reference.GetValue(relativeTransform)) * Helper3D.QuaternionFromEulerAngles((Vector3D) property.Reference.GetCurrentValue(target));
            property.Reference.SetValue(instance1, (object) Helper3D.EulerAnglesFromQuaternion(orientation));
          }
        }
      }
      return instance1;
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
