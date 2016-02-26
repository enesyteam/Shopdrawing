// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.CategoryFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Windows.Design.PropertyEditing;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public static class CategoryFactory
  {
    private static Dictionary<CategoryLocalizationHelper.CategoryName, CustomCategorySelector> categoryNameLookup = new Dictionary<CategoryLocalizationHelper.CategoryName, CustomCategorySelector>();

    static CategoryFactory()
    {
      CategoryFactory.categoryNameLookup.Add(CategoryLocalizationHelper.CategoryName.Layout, (CustomCategorySelector) new CategoryFactory.LayoutPositionCategorySelector());
      CategoryFactory.categoryNameLookup.Add(CategoryLocalizationHelper.CategoryName.Text, (CustomCategorySelector) new CategoryFactory.TextCategorySelector());
      CategoryFactory.categoryNameLookup.Add(CategoryLocalizationHelper.CategoryName.Camera, (CustomCategorySelector) new CategoryFactory.CameraCategorySelector());
      CategoryFactory.categoryNameLookup.Add(CategoryLocalizationHelper.CategoryName.Light, (CustomCategorySelector) new CategoryFactory.LightCategorySelector());
      CategoryFactory.categoryNameLookup.Add(CategoryLocalizationHelper.CategoryName.Brushes, (CustomCategorySelector) new CategoryFactory.BrushCategorySelector());
      CategoryFactory.categoryNameLookup.Add(CategoryLocalizationHelper.CategoryName.Triggers, (CustomCategorySelector) new CategoryFactory.TriggersCategorySelector());
      CategoryFactory.categoryNameLookup.Add(CategoryLocalizationHelper.CategoryName.Easing, (CustomCategorySelector) new CategoryFactory.EasingCategorySelector());
      CategoryFactory.categoryNameLookup.Add(CategoryLocalizationHelper.CategoryName.Conditions, (CustomCategorySelector) new CategoryFactory.ExpressionConditionBehaviorCategorySelector());
      CategoryFactory.categoryNameLookup.Add(CategoryLocalizationHelper.CategoryName.LayoutPaths, (CustomCategorySelector) new CategoryFactory.PathLayoutCategorySelector());
    }

    public static CustomCategorySelector GetCustomCategorySelector(CategoryLocalizationHelper.CategoryName category)
    {
      return CategoryFactory.GetCategorySelectorByCategoryName(category) ?? new CustomCategorySelector();
    }

    private static CustomCategorySelector GetCategorySelectorByCategoryName(CategoryLocalizationHelper.CategoryName category)
    {
      CustomCategorySelector categorySelector;
      if (CategoryFactory.categoryNameLookup.TryGetValue(category, out categorySelector))
        return categorySelector;
      return (CustomCategorySelector) null;
    }

    public static CustomCategorySelector GetCustomCategorySelector(PropertyReferenceProperty property)
    {
      CustomCategorySelector selectorByCategoryName;
      if ((selectorByCategoryName = CategoryFactory.GetCategorySelectorByCategoryName(CategoryLocalizationHelper.GetCanonicalCategoryName(((PropertyEntry) property).get_CategoryName()))) != null)
        return selectorByCategoryName;
      if (BehaviorHelper.IsPropertyBehaviorCommand(property))
        return (CustomCategorySelector) new CategoryFactory.BehaviorCommandCategorySelector();
      return new CustomCategorySelector();
    }

    private class LayoutPositionCategorySelector : CustomCategorySelector
    {
      public override SceneNodeCategory CreateSceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
      {
        return (SceneNodeCategory) new LayoutPositionCategory(localizedName, messageLogger);
      }
    }

    private class TriggersCategorySelector : CustomCategorySelector
    {
      public override SceneNodeCategory CreateSceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
      {
        return (SceneNodeCategory) new TriggerCategory(localizedName, messageLogger);
      }
    }

    private class CameraCategorySelector : CustomCategorySelector
    {
      public override SceneNodeCategory CreateSceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
      {
        return (SceneNodeCategory) new CameraCategory(localizedName, messageLogger);
      }
    }

    private class LightCategorySelector : CustomCategorySelector
    {
      public override SceneNodeCategory CreateSceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
      {
        return (SceneNodeCategory) new LightCategory(localizedName, messageLogger);
      }
    }

    private class TextCategorySelector : CustomCategorySelector
    {
      public override SceneNodeCategory CreateSceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
      {
        return (SceneNodeCategory) new TextCategory(localizedName, messageLogger);
      }
    }

    private class BrushCategorySelector : CustomCategorySelector
    {
      public override SceneNodeCategory CreateSceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
      {
        return (SceneNodeCategory) new BrushCategory(localizedName, messageLogger);
      }
    }

    private class ExpressionConditionBehaviorCategorySelector : CustomCategorySelector
    {
      public override SceneNodeCategory CreateSceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
      {
        return (SceneNodeCategory) new ConditionalExpressionBehaviorCategory(localizedName, messageLogger);
      }
    }

    private class BehaviorCommandCategorySelector : CustomCategorySelector
    {
      public override SceneNodeCategory CreateSceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
      {
        return (SceneNodeCategory) new BehaviorCommandCategory(localizedName, messageLogger);
      }
    }

    private class EasingCategorySelector : CustomCategorySelector
    {
      public override SceneNodeCategory CreateSceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
      {
        return (SceneNodeCategory) new EasingCategoryCollection(localizedName, messageLogger);
      }
    }

    private class PathLayoutCategorySelector : CustomCategorySelector
    {
      public override SceneNodeCategory CreateSceneNodeCategory(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, IMessageLoggingService messageLogger)
      {
        return (SceneNodeCategory) new PathLayoutCategory(localizedName, messageLogger);
      }
    }
  }
}
