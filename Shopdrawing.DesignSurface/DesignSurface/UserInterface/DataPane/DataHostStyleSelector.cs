// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataHostStyleSelector
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class DataHostStyleSelector : StyleSelector
  {
    private Style separatorStyle;

    public DataHostStyleSelector()
    {
      this.separatorStyle = new Style();
      ControlTemplate controlTemplate = new ControlTemplate();
      controlTemplate.VisualTree = new FrameworkElementFactory(typeof (Separator));
      controlTemplate.VisualTree.SetValue(UIElement.IsEnabledProperty, (object) false);
      this.separatorStyle.Setters.Add((SetterBase) new Setter(Control.TemplateProperty, (object) controlTemplate));
    }

    public override Style SelectStyle(object item, DependencyObject container)
    {
      if (item is SeparatorDataHost)
        return this.separatorStyle;
      return (Style) null;
    }
  }
}
