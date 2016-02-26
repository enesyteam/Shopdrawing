// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.CustomCategorySelector
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.Framework;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class CustomCategorySelector
  {
    public virtual SceneNodeCategory CreateSceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
    {
      return new SceneNodeCategory(canonicalName, localizedName, messageLogger);
    }
  }
}
