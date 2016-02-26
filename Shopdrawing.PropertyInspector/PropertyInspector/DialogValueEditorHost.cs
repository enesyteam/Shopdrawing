// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.DialogValueEditorHost
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Windows.Design.PropertyEditing;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class DialogValueEditorHost : Dialog
  {
    public DialogValueEditorHost(PropertyValue propertyValue, DataTemplate dialogContent)
    {
      this.DialogContent = (UIElement) FileTable.GetElement("Resources\\PropertyInspector\\DialogValueEditorHost.xaml");
      this.Title = StringTable.DialogValueEditorHostTitle;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      ContentControl contentControl = (ContentControl) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "ContentHost");
      contentControl.Content = propertyValue;
      contentControl.ContentTemplate = dialogContent;
      this.SetValue(DesignerProperties.IsInDesignModeProperty, false);
    }
  }
}
