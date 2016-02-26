// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SilverlightNumberEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SilverlightNumberEditor : NumberEditor
  {
    private static readonly RoutedCommand clearValueCommand;

    public static RoutedCommand ClearValueCommand
    {
      get
      {
        return SilverlightNumberEditor.clearValueCommand;
      }
    }

    protected override ITypeDescriptorContext TypeDescriptorContext
    {
      get
      {
        PropertyValue propertyValue = this.DataContext as PropertyValue;
        if (propertyValue != null)
        {
          PropertyReferenceProperty referenceProperty = propertyValue.get_ParentProperty() as PropertyReferenceProperty;
          if (referenceProperty != null)
          {
            IProjectContext projectContext = referenceProperty.ObjectSet.ProjectContext;
            if (projectContext != null)
            {
              ConverterContext converterContext = new ConverterContext(this);
              converterContext.AddService(typeof (SilverlightConverterService), (object) new SilverlightConverterService(projectContext.IsCapabilitySet(PlatformCapability.SupportsAutoAndNan)));
              return (ITypeDescriptorContext) converterContext;
            }
          }
        }
        return (ITypeDescriptorContext) new ConverterContext(this);
      }
    }

    static SilverlightNumberEditor()
    {
      NumberEditor.ConverterProperty.AddOwner(typeof (SilverlightNumberEditor), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(SilverlightNumberEditor.This_CoerceConverterChanged)));
      SilverlightNumberEditor.clearValueCommand = new RoutedCommand("ClearValueCommand", typeof (SilverlightNumberEditor));
    }

    public SilverlightNumberEditor()
    {
      this.CommandBindings.Add(new CommandBinding((ICommand) SilverlightNumberEditor.ClearValueCommand, new ExecutedRoutedEventHandler(this.ExecuteClearPropertyValueCommand), new CanExecuteRoutedEventHandler(this.CanExecuteClearPropertyValueCommand)));
    }

    private static object This_CoerceConverterChanged(DependencyObject d, object value)
    {
      if (value != null)
      {
        Type type = value.GetType();
        if (typeof (LengthConverter).IsAssignableFrom(type) && !typeof (ConditionalAutoLengthConverter).IsAssignableFrom(type) && d is SilverlightNumberEditor)
          return (object) new ConditionalAutoLengthConverter();
      }
      return value;
    }

    internal void ExecuteClearPropertyValueCommand(object sender, ExecutedRoutedEventArgs args)
    {
      PropertyValue propertyValue = this.DataContext as PropertyValue;
      if (propertyValue == null)
        return;
      SceneNodeProperty sceneNodeProperty = propertyValue.get_ParentProperty() as SceneNodeProperty;
      if (sceneNodeProperty == null)
        return;
      sceneNodeProperty.DoClearValue();
    }

    internal void CanExecuteClearPropertyValueCommand(object sender, CanExecuteRoutedEventArgs args)
    {
      bool flag = false;
      PropertyValue propertyValue = this.DataContext as PropertyValue;
      if (propertyValue != null && propertyValue.get_ParentProperty() is SceneNodeProperty)
        flag = true;
      args.CanExecute = flag;
    }
  }
}
