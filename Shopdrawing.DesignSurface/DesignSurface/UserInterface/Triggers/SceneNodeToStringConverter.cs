// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.SceneNodeToStringConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  public class SceneNodeToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) SceneNodeToStringConverter.ConvertToString(value as SceneNode);
    }

    public static string ConvertToString(SceneNode sceneNode)
    {
      if (sceneNode == null)
        return "[Null]";
      if (sceneNode is FrameworkTemplateElement)
        return StringTable.TriggerSourceTemplate;
      if (sceneNode is StyleNode)
        return StringTable.TriggerSourceStyle;
      SceneElement sceneElement = sceneNode as SceneElement;
      if (sceneElement != null)
        return sceneElement.DisplayName;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.ElementTimelineItemBracketedName, new object[1]
      {
        (object) sceneNode.TargetType.Name
      });
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) null;
    }
  }
}
