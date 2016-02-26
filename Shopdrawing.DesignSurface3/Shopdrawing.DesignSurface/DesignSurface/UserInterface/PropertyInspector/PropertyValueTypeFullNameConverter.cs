// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyValueTypeFullNameConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public sealed class PropertyValueTypeFullNameConverter : PropertyValueTypeConverterBase
  {
    protected override object Convert(Type type)
    {
      StackPanel stackPanel = new StackPanel()
      {
        Orientation = Orientation.Horizontal
      };
      TextBlock textBlock1 = new TextBlock()
      {
        Text = type.Namespace + "."
      };
      TextBlock textBlock2 = new TextBlock()
      {
        Text = type.Name,
        FontWeight = FontWeights.Bold
      };
      stackPanel.Children.Add((UIElement) textBlock1);
      stackPanel.Children.Add((UIElement) textBlock2);
      return (object) stackPanel;
    }
  }
}
