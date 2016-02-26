// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.TabGroupBorderThicknessConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System.Globalization;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class TabGroupBorderThicknessConverter : ValueConverter<Thickness, Thickness>
  {
    protected override Thickness Convert(Thickness value, object parameter, CultureInfo culture)
    {
      TabGroupBorderType tabGroupBorderType = (TabGroupBorderType) parameter;
      Thickness thickness = new Thickness();
      switch (tabGroupBorderType)
      {
        case TabGroupBorderType.HeaderBorder:
          thickness = new Thickness()
          {
            Left = value.Left,
            Right = value.Right,
            Top = value.Top,
            Bottom = 0.0
          };
          break;
        case TabGroupBorderType.ContentBorder:
          thickness = new Thickness()
          {
            Left = value.Left,
            Right = value.Right,
            Top = 0.0,
            Bottom = value.Bottom
          };
          break;
      }
      return thickness;
    }
  }
}
