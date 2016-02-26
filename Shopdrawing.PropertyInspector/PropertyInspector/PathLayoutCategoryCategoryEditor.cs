// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PathLayoutCategoryCategoryEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal class PathLayoutCategoryCategoryEditor : SceneNodeCategoryEditor
  {
    private static readonly IPropertyId[] pathLayoutProperties = new IPropertyId[6]
    {
      PathListBoxElement.WrapItemsProperty,
      PathListBoxElement.StartItemIndexProperty,
      PathListBoxElement.LayoutPathsProperty,
      PathPanelElement.WrapItemsProperty,
      PathPanelElement.StartItemIndexProperty,
      PathPanelElement.LayoutPathsProperty
    };

    public virtual DataTemplate EditorTemplate
    {
      get
      {
        DataTemplate dataTemplate = new DataTemplate();
        dataTemplate.VisualTree = new FrameworkElementFactory(typeof (PathLayoutCategoryCategoryEditorControl));
        return dataTemplate;
      }
    }

    public virtual string TargetCategory
    {
      get
      {
        return CategoryLocalizationHelper.GetLocalizedCategoryName(CategoryLocalizationHelper.CategoryName.LayoutPaths);
      }
    }

    public virtual object GetImage(Size desiredSize)
    {
      return null;
    }

    public static bool IsPathLayoutProperty(ReferenceStep referenceStep)
    {
      return Array.IndexOf<IPropertyId>(PathLayoutCategoryCategoryEditor.pathLayoutProperties, (IPropertyId) referenceStep) != -1;
    }

    public virtual bool ConsumesProperty(PropertyEntry propertyEntry)
    {
      return PathLayoutCategoryCategoryEditor.IsPathLayoutProperty(((PropertyReferenceProperty) propertyEntry).Reference.LastStep);
    }
  }
}
