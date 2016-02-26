// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.RepeatDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.ValueEditors;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal class RepeatDialog : Dialog
  {
    public static RepeatConverter RepeatDoubleConverter = new RepeatConverter();
    private NumberEditor repeatCountNumberEditor;

    public double RepeatCount
    {
      get
      {
        return this.repeatCountNumberEditor.Value;
      }
      set
      {
        this.repeatCountNumberEditor.Value = value;
      }
    }

    public RepeatDialog()
    {
      this.DialogContent = (UIElement) FileTable.GetElement("Resources\\Timeline\\RepeatDialog.xaml");
      this.Title = StringTable.RepeatDialogTitle;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.repeatCountNumberEditor = ElementUtilities.FindElement((FrameworkElement) this, "RepeatCount") as NumberEditor;
    }
  }
}
