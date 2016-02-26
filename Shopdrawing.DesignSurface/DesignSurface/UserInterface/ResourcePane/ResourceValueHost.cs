// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourceValueHost
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.Framework.PropertyInspector;
using Microsoft.Windows.Design.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  internal class ResourceValueHost : ContentControl
  {
    public static readonly DependencyProperty ResourceValueModelProperty = DependencyProperty.Register("ResourceValueModel", typeof (ResourceValueModel), typeof (ResourceValueHost), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(ResourceValueHost.OnResourceValueModelChanged)));

    public ResourceValueModel ResourceValueModel
    {
      get
      {
        return (ResourceValueModel) this.GetValue(ResourceValueHost.ResourceValueModelProperty);
      }
      set
      {
        this.SetValue(ResourceValueHost.ResourceValueModelProperty, (object) value);
      }
    }

    private static void OnResourceValueModelChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
      ResourceValueHost resourceValueHost = sender as ResourceValueHost;
      ResourceValueModel resourceValueModel = args.NewValue as ResourceValueModel;
      if (resourceValueHost == null || resourceValueModel == null)
        return;
      resourceValueHost.CommandBindings.Clear();
      SupportedPropertyCommands supportedCommands = ~SupportedPropertyCommands.PinnedEditor;
      resourceValueModel.PropertyEditingHelper.AddCommandBindings((UIElement) resourceValueHost, supportedCommands);
      resourceValueHost.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.ShowExtendedPinnedEditor, new ExecutedRoutedEventHandler(ResourceValueHost.OnShowExtendedPinnedEditor)));
      PropertyInspectorHelper.SetOwningPropertyInspectorModel((DependencyObject) resourceValueHost, (IPropertyInspector) resourceValueModel);
      PropertyInspectorHelper.SetOwningPropertyInspectorElement((DependencyObject) resourceValueHost, (UIElement) resourceValueHost);
    }

    private static void OnShowExtendedPinnedEditor(object sender, ExecutedRoutedEventArgs eventArgs)
    {
      PropertyContainer propertyContainer = PropertyEditingHelper.GetPropertyContainer(eventArgs.OriginalSource);
      if (propertyContainer == null)
        return;
      if (propertyContainer.PropertyEntry != null && !(propertyContainer.PropertyEntry is ResourcePaneSceneNodeProperty))
        propertyContainer.ActiveEditMode = PropertyContainerEditMode.ExtendedPinned;
      eventArgs.Handled = true;
    }
  }
}
