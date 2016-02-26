// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.CategoryEditorInstanceFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public static class CategoryEditorInstanceFactory
  {
    private static List<CategoryEditorFactory> CategoryEditorFactories = new List<CategoryEditorFactory>();

    static CategoryEditorInstanceFactory()
    {
      CategoryEditorInstanceFactory.RegisterFactories();
    }

    public static CategoryEditorSet GetEditors(ITypeId selectedType, SceneNodeCategory category)
    {
      CategoryEditorSet categoryEditorSet = new CategoryEditorSet();
      foreach (CategoryEditorFactory categoryEditorFactory in CategoryEditorInstanceFactory.CategoryEditorFactories)
        categoryEditorSet = categoryEditorSet.Union(categoryEditorFactory.GetCategoryEditors(selectedType, category));
      return categoryEditorSet;
    }

    private static void RegisterFactories()
    {
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new LayoutCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new BrushCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new TriggerCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new CameraCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new LightCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new MaterialsCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new TextCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new TransformCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new AppearanceCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new EasingCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new MiscCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new BehaviorCommandCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new ConditionalExpressionBehaviorCategoryEditorFactory());
      CategoryEditorInstanceFactory.CategoryEditorFactories.Add((CategoryEditorFactory) new ItemsLayoutCategoryEditorFactory());
    }
  }
}
