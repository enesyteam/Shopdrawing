// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SnapDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.ValueEditors;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal class SnapDialog : Dialog
  {
    private NumberEditor snapEntryNumberEditor;

    private double FramesPerSecond
    {
      get
      {
        return this.snapEntryNumberEditor.Value;
      }
      set
      {
        this.snapEntryNumberEditor.Value = value;
      }
    }

    private SnapDialog()
    {
      this.DialogContent = (UIElement) FileTable.GetElement("Resources\\Timeline\\SnapDialog.xaml");
      this.Title = StringTable.SnapDialogTitle;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.snapEntryNumberEditor = (NumberEditor) ElementUtilities.FindElement((FrameworkElement) this, "SnapResolution");
    }

    public static bool GetNewSnapFramesPerSecond(double initialFramesPerSecond, out double newFramesPerSecond)
    {
      bool flag = false;
      SnapDialog snapDialog = new SnapDialog();
      snapDialog.FramesPerSecond = initialFramesPerSecond;
      bool? nullable = snapDialog.ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) != 0)
      {
        flag = true;
        newFramesPerSecond = snapDialog.FramesPerSecond;
      }
      else
        newFramesPerSecond = 0.0;
      return flag;
    }
  }
}
