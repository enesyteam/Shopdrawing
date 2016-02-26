// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyValueToTypeIdConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class PropertyValueToTypeIdConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      PropertyReferencePropertyValue referencePropertyValue = value as PropertyReferencePropertyValue;
      if (referencePropertyValue != null)
      {
        SceneNodeProperty sceneNodeProperty = referencePropertyValue.OwnerProperty as SceneNodeProperty;
        if (sceneNodeProperty != null)
        {
          IProjectContext projectContext = sceneNodeProperty.SceneNodeObjectSet.ProjectContext;
          if (projectContext == null)
            return null;
          Type runtimeType1 = projectContext.ResolveType(ProjectNeutralTypes.BehaviorEventTriggerBase).RuntimeType;
          Type runtimeType2 = projectContext.ResolveType(ProjectNeutralTypes.BehaviorTargetedTriggerAction).RuntimeType;
          Type type = sceneNodeProperty.ObjectSet.ObjectType;
          while (type != (Type) null && type.BaseType != (Type) null && (!type.BaseType.Equals(runtimeType1) && !type.BaseType.Equals(runtimeType2)))
            type = type.BaseType;
          Type[] genericArguments = type.GetGenericArguments();
          if (genericArguments.Length > 0)
            return (object) projectContext.GetType(genericArguments[0]);
          return (object) type;
        }
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
