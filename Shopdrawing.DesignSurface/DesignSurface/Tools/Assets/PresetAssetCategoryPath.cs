// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.PresetAssetCategoryPath
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal sealed class PresetAssetCategoryPath : AssetCategoryPath
  {
    private static Dictionary<PresetAssetCategory, PresetAssetCategoryPath> enumToInstanceMap = new Dictionary<PresetAssetCategory, PresetAssetCategoryPath>();
    private static Dictionary<string, PresetAssetCategoryPath> pathToInstanceMap = new Dictionary<string, PresetAssetCategoryPath>();

    public static PresetAssetCategoryPath ProjectRoot { get; private set; }

    public static PresetAssetCategoryPath ControlsRoot { get; private set; }

    public static PresetAssetCategoryPath ControlsAll { get; private set; }

    public static PresetAssetCategoryPath ControlsPanels { get; private set; }

    public static PresetAssetCategoryPath EffectsRoot { get; private set; }

    public static PresetAssetCategoryPath BehaviorsRoot { get; private set; }

    public static PresetAssetCategoryPath BehaviorsPrototype { get; private set; }

    public static PresetAssetCategoryPath StylesRoot { get; private set; }

    public static PresetAssetCategoryPath ShapesRoot { get; private set; }

    public static PresetAssetCategoryPath MediaRoot { get; private set; }

    public static PresetAssetCategoryPath CategoriesRoot { get; private set; }

    public static PresetAssetCategoryPath PrototypeRoot { get; private set; }

    public static PresetAssetCategoryPath PrototypeNavigations { get; private set; }

    public static PresetAssetCategoryPath PrototypeCompositions { get; private set; }

    public static PresetAssetCategoryPath PrototypeBehaviors { get; private set; }

    public static PresetAssetCategoryPath PrototypeStyles { get; private set; }

    public static PresetAssetCategoryPath PrototypeShapes { get; private set; }

    public static PresetAssetCategoryPath SketchShapes { get; private set; }

    public static PresetAssetCategoryPath LocationsRoot { get; private set; }

    public PresetAssetCategory PresetAssetCategory { get; private set; }

    static PresetAssetCategoryPath()
    {
      PresetAssetCategoryPath.ProjectRoot = new PresetAssetCategoryPath(PresetAssetCategory.Project, "Project", true);
      PresetAssetCategoryPath.ControlsRoot = new PresetAssetCategoryPath(PresetAssetCategory.Controls, "Controls", false);
      PresetAssetCategoryPath.ControlsAll = new PresetAssetCategoryPath(PresetAssetCategory.ControlsAll, "Controls/All", true);
      PresetAssetCategoryPath.ControlsPanels = new PresetAssetCategoryPath(PresetAssetCategory.ControlsPanels, "Controls/Panels", true);
      PresetAssetCategoryPath.EffectsRoot = new PresetAssetCategoryPath(PresetAssetCategory.Effects, "Effects", true);
      PresetAssetCategoryPath.BehaviorsRoot = new PresetAssetCategoryPath(PresetAssetCategory.Behaviors, "Behaviors", true);
      PresetAssetCategoryPath.BehaviorsPrototype = new PresetAssetCategoryPath(PresetAssetCategory.BehaviorsPrototype, "Behaviors/SketchFlow", true);
      PresetAssetCategoryPath.StylesRoot = new PresetAssetCategoryPath(PresetAssetCategory.Styles, "Styles", true);
      PresetAssetCategoryPath.ShapesRoot = new PresetAssetCategoryPath(PresetAssetCategory.Shapes, "Shapes", true);
      PresetAssetCategoryPath.MediaRoot = new PresetAssetCategoryPath(PresetAssetCategory.Media, "Media", true);
      PresetAssetCategoryPath.CategoriesRoot = new PresetAssetCategoryPath(PresetAssetCategory.Categories, "Categories", true);
      PresetAssetCategoryPath.PrototypeRoot = new PresetAssetCategoryPath(PresetAssetCategory.Prototype, "SketchFlow", true);
      PresetAssetCategoryPath.PrototypeNavigations = new PresetAssetCategoryPath(PresetAssetCategory.PrototypeNavigations, "SketchFlow/Navigation Screens", true);
      PresetAssetCategoryPath.PrototypeCompositions = new PresetAssetCategoryPath(PresetAssetCategory.PrototypeCompositions, "SketchFlow/Component Screens", true);
      PresetAssetCategoryPath.PrototypeBehaviors = new PresetAssetCategoryPath(PresetAssetCategory.PrototypeBehaviors, "SketchFlow/Behaviors", true);
      PresetAssetCategoryPath.PrototypeStyles = new PresetAssetCategoryPath(PresetAssetCategory.PrototypeStyles, "SketchFlow/Styles", true);
      PresetAssetCategoryPath.PrototypeShapes = new PresetAssetCategoryPath(PresetAssetCategory.PrototypeShapes, "SketchFlow/Sketch Shapes", false);
      PresetAssetCategoryPath.SketchShapes = new PresetAssetCategoryPath(PresetAssetCategory.SketchShapes, "Shapes/Sketch Shapes", false);
      PresetAssetCategoryPath.LocationsRoot = new PresetAssetCategoryPath(PresetAssetCategory.Locations, "Locations", true);
    }

    private PresetAssetCategoryPath(PresetAssetCategory presetCategory, string path, bool alwaysShow)
      : base(path, alwaysShow)
    {
      this.PresetAssetCategory = presetCategory;
      PresetAssetCategoryPath.enumToInstanceMap[presetCategory] = this;
      PresetAssetCategoryPath.pathToInstanceMap[path] = this;
    }

    public static PresetAssetCategoryPath FromPreset(PresetAssetCategory category)
    {
      return PresetAssetCategoryPath.enumToInstanceMap[category];
    }

    public static PresetAssetCategoryPath FromPath(AssetCategoryPath path)
    {
      PresetAssetCategoryPath assetCategoryPath;
      if (!PresetAssetCategoryPath.pathToInstanceMap.TryGetValue(path.FullPath, out assetCategoryPath))
        return (PresetAssetCategoryPath) null;
      return assetCategoryPath;
    }
  }
}
