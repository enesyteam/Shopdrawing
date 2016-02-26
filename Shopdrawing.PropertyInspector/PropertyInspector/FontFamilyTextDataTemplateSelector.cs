// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FontFamilyTextDataTemplateSelector
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal class FontFamilyTextDataTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      SourcedFontFamilyItem sourcedFontFamilyItem = item as SourcedFontFamilyItem;
      FrameworkElement frameworkElement = container as FrameworkElement;
      if (frameworkElement == null)
        return (DataTemplate) null;
      if (sourcedFontFamilyItem != null)
      {
        if (!sourcedFontFamilyItem.IsFontReadable)
          return frameworkElement.TryFindResource((object) "UnreadableFontFamilyDataTemplate") as DataTemplate;
        if (sourcedFontFamilyItem.IsFontDamaged)
          return frameworkElement.TryFindResource((object) "DamagedFontFamilyDataTemplate") as DataTemplate;
      }
      return frameworkElement.TryFindResource((object) "ReadableFontFamilyDataTemplate") as DataTemplate;
    }
  }
}
