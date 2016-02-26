// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.CategoryLocalizationHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using System;
using System.Globalization;
using System.Resources;

namespace Microsoft.Expression.DesignSurface.Properties
{
  public static class CategoryLocalizationHelper
  {
    private static CategoryLocalizationHelper.CategoryTableEntry[] categoryEntries = new CategoryLocalizationHelper.CategoryTableEntry[16]
    {
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Brushes, CategoryNames.CategoryBrushes, "Brushes"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Materials, CategoryNames.CategoryMaterials, "Materials"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Appearance, CategoryNames.CategoryAppearance, "Appearance"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Layout, CategoryNames.CategoryLayout, "Layout"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.LayoutPaths, CategoryNames.CategoryLayoutPaths, "Layout Paths"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.CommonProperties, CategoryNames.CategoryCommonProperties, "Common Properties"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.DataVisualization, CategoryNames.CategoryDataVisualization, "Data Visualization"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Text, CategoryNames.CategoryText, "Text"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Transform, CategoryNames.CategoryTransform, "Transform"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Media, CategoryNames.CategoryMedia, "Media"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Camera, CategoryNames.CategoryCamera, "Camera"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Light, CategoryNames.CategoryLight, "Light"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Easing, CategoryNames.CategoryEasing, "Easing"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Misc, CategoryNames.CategoryMisc, "Default"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Triggers, CategoryNames.CategoryTrigger, "Trigger"),
      new CategoryLocalizationHelper.CategoryTableEntry(CategoryLocalizationHelper.CategoryName.Conditions, CategoryNames.CategoryConditions, "Conditions")
    };
    private static ResourceManager resources;

    public static CategoryLocalizationHelper.CategoryName GetCanonicalCategoryName(string categoryName)
    {
      foreach (CategoryLocalizationHelper.CategoryTableEntry categoryTableEntry in CategoryLocalizationHelper.categoryEntries)
      {
        if (categoryName == categoryTableEntry.LocalizedName || categoryName == categoryTableEntry.UnlocalizedName || categoryName == categoryTableEntry.CanonicalLocalizedName)
          return categoryTableEntry.CanonicalName;
      }
      return CategoryLocalizationHelper.CategoryName.Unknown;
    }

    public static string GetLocalizedCategoryName(CategoryLocalizationHelper.CategoryName categoryName)
    {
      foreach (CategoryLocalizationHelper.CategoryTableEntry categoryTableEntry in CategoryLocalizationHelper.categoryEntries)
      {
        if (categoryTableEntry.CanonicalName == categoryName)
          return categoryTableEntry.LocalizedName;
      }
      throw new InvalidOperationException(ExceptionStringTable.UnableToGetLocalizedCategoryNamesForNonBuiltInCategoriesThroughThisFunction);
    }

    public static string GetLocalizedCategory(string canonicalCategory)
    {
      if (CategoryLocalizationHelper.resources == null)
        CategoryLocalizationHelper.resources = new ResourceManager("System", typeof (Uri).Assembly);
      return (string) CategoryLocalizationHelper.resources.GetObject("PropertyCategory" + canonicalCategory, CultureInfo.CurrentUICulture);
    }

    public enum CategoryName
    {
      Unknown = -1,
      Triggers = 0,
      Conditions = 1,
      BehaviorCommand = 2,
      Brushes = 3,
      Materials = 4,
      Appearance = 5,
      Layout = 6,
      LayoutPaths = 7,
      Media = 8,
      CommonProperties = 9,
      DataVisualization = 10,
      Text = 11,
      Transform = 12,
      Camera = 13,
      Light = 14,
      Easing = 15,
      Misc = 16,
    }

    private struct CategoryTableEntry
    {
      private CategoryLocalizationHelper.CategoryName canonicalName;
      private string localizedName;
      private string unlocalizedName;
      private string canonicalLocalizedName;

      public CategoryLocalizationHelper.CategoryName CanonicalName
      {
        get
        {
          return this.canonicalName;
        }
      }

      public string LocalizedName
      {
        get
        {
          return this.localizedName;
        }
      }

      public string UnlocalizedName
      {
        get
        {
          return this.unlocalizedName;
        }
      }

      public string CanonicalLocalizedName
      {
        get
        {
          return this.canonicalLocalizedName;
        }
      }

      public CategoryTableEntry(CategoryLocalizationHelper.CategoryName canonicalName, string localizedName, string unlocalizedName)
      {
        this.canonicalName = canonicalName;
        this.localizedName = localizedName;
        this.unlocalizedName = unlocalizedName;
        this.canonicalLocalizedName = CategoryLocalizationHelper.GetLocalizedCategory(unlocalizedName);
      }
    }
  }
}
