// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ReferencesFoundDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public sealed class ReferencesFoundDialog : Dialog
  {
    private static readonly string AcceptButtonName = "AcceptButton";
    private static readonly string CancelButtonName = "CancelButton";
    private static readonly string ResourceNameTextBlock = "Label_OtherReferences";
    private ReferencesFoundModel model;

    internal ReferencesFoundDialog(ReferencesFoundModel model)
    {
      this.model = model;
      FrameworkElement element = FileTable.GetElement("Resources\\ResourcePane\\ReferencesFoundDialog.xaml");
      element.DataContext = (object) model;
      this.DialogContent = (UIElement) element;
      this.Title = StringTable.ReferencesFoundDialogTitle;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      Button button1 = (Button) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, ReferencesFoundDialog.AcceptButtonName);
      if (button1 != null)
        button1.IsDefault = false;
      Button button2 = (Button) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, ReferencesFoundDialog.CancelButtonName);
      if (button2 != null)
        button2.IsDefault = true;
      TextBlock textBlock = (TextBlock) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, ReferencesFoundDialog.ResourceNameTextBlock);
      if (textBlock == null)
        return;
      textBlock.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ReferencesFoundDialogText, new object[1]
      {
        this.model.ResourceEntryNode.Key
      });
    }
  }
}
