// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetCategoryPath
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  public class AssetCategoryPath
  {
    private static Dictionary<string, string> LocalizedNameTable = new Dictionary<string, string>()
    {
      {
        "Project",
        StringTable.ProjectAssetCategoryName
      },
      {
        "Controls",
        StringTable.ControlsAssetCategoryName
      },
      {
        "Controls/All",
        StringTable.ControlsAllAssetCategoryName
      },
      {
        "Effects",
        StringTable.EffectsAssetCategoryName
      },
      {
        "Behaviors",
        StringTable.BehaviorsAssetCategoryName
      },
      {
        "Styles",
        StringTable.StylesAssetCategoryName
      },
      {
        "Shapes",
        StringTable.ShapesAssetCategoryName
      },
      {
        "Media",
        StringTable.MediaAssetCategoryName
      },
      {
        "Categories",
        StringTable.CategoriesAssetCategoryName
      },
      {
        "Locations",
        StringTable.LocationsAssetCategoryName
      },
      {
        "SketchFlow",
        StringTable.PrototypeAssetCategoryName
      },
      {
        "SketchFlow/Navigation Screens",
        StringTable.PrototypeNavigationsAssetCategoryName
      },
      {
        "SketchFlow/Component Screens",
        StringTable.PrototypeCompositionsAssetCategoryName
      },
      {
        "SketchFlow/Behaviors",
        StringTable.PrototypeBehaviorsAssetCategoryName
      },
      {
        "Behaviors/SketchFlow",
        StringTable.PrototypeAssetCategoryName
      },
      {
        "SketchFlow/Styles",
        StringTable.StylesAssetCategoryName
      },
      {
        "SketchFlow/Sketch Shapes",
        StringTable.SketchShapesAssetCategoryName
      },
      {
        "Shapes/Sketch Shapes",
        StringTable.SketchShapesAssetCategoryName
      },
      {
        "Controls/Panels",
        StringTable.ControlsPanelsAssetCategoryName
      },
      {
        "Categories/Panels",
        StringTable.ControlsPanelsAssetCategoryName
      },
      {
        "Controls/Basic Controls",
        StringTable.ControlsBasicControlsAssetCategoryName
      },
      {
        "Categories/Basic Controls",
        StringTable.ControlsBasicControlsAssetCategoryName
      },
      {
        "Controls/Control Parts",
        StringTable.ControlsControlPartsAssetCategoryName
      },
      {
        "Categories/Control Parts",
        StringTable.ControlsControlPartsAssetCategoryName
      },
      {
        "Controls/Data",
        StringTable.ControlsDataAssetCategoryName
      },
      {
        "Categories/Data",
        StringTable.ControlsDataAssetCategoryName
      },
      {
        "Controls/Data/Basic Controls",
        StringTable.ControlsDataBasicControlsAssetCategoryName
      },
      {
        "Categories/Data/Basic Controls",
        StringTable.ControlsDataBasicControlsAssetCategoryName
      },
      {
        "Controls/Data/Control Parts",
        StringTable.ControlsDataControlPartsAssetCategoryName
      },
      {
        "Categories/Data/Control Parts",
        StringTable.ControlsDataControlPartsAssetCategoryName
      },
      {
        "Styles/Simple Styles",
        StringTable.StylesSimpleStylesAssetCategoryName
      },
      {
        "Styles/SketchStyles",
        StringTable.StylesSketchStylesAssetCategoryName
      },
      {
        "SketchFlow/Styles/SketchStyles",
        StringTable.StylesSketchStylesAssetCategoryName
      }
    };
    private static readonly AssetCategoryPath root = new AssetCategoryPath();
    protected const string SketchShapesCategoryName = "Sketch Shapes";
    private string[] steps;
    private AssetCategoryPath parentPath;
    private string displayName;

    public static AssetCategoryPath Root
    {
      get
      {
        return AssetCategoryPath.root;
      }
    }

    public string FullPath { get; private set; }

    public bool AlwaysShow { get; private set; }

    internal string[] Steps
    {
      get
      {
        if (this.steps == null)
          this.steps = AssetCategoryPath.AssetCategoryPathHelper.SplitSteps(this.FullPath);
        return this.steps;
      }
    }

    public string LastStep
    {
      get
      {
        if (this.Steps.Length <= 0)
          return string.Empty;
        return this.Steps[this.Steps.Length - 1];
      }
    }

    public AssetCategoryPath Parent
    {
      get
      {
        if (this.parentPath == null)
        {
          if (this.Steps.Length <= 1)
          {
            this.parentPath = AssetCategoryPath.Root;
          }
          else
          {
            this.parentPath = new AssetCategoryPath(AssetCategoryPath.AssetCategoryPathHelper.RemovePath(this.FullPath, this.LastStep), this.AlwaysShow);
            this.parentPath = (AssetCategoryPath) PresetAssetCategoryPath.FromPath(this.parentPath) ?? this.parentPath;
          }
        }
        return this.parentPath;
      }
    }

    public string DisplayName
    {
      get
      {
        if (this.displayName == null && !AssetCategoryPath.LocalizedNameTable.TryGetValue(this.FullPath, out this.displayName))
          this.displayName = this.LastStep;
        return this.displayName;
      }
    }

    public bool IsRoot
    {
      get
      {
        return AssetCategoryPath.AssetCategoryPathHelper.IsRootPath(this);
      }
    }

    private AssetCategoryPath()
    {
      this.FullPath = string.Empty;
    }

    protected AssetCategoryPath(string fullPath, bool alwaysShow)
    {
      if (fullPath == null)
        throw new ArgumentNullException("fullPath");
      this.FullPath = fullPath;
      this.AlwaysShow = alwaysShow;
    }

    public AssetCategoryPath Append(string relativePath, bool alwaysShow)
    {
      string fullPath = AssetCategoryPath.AssetCategoryPathHelper.CombinePath(this.FullPath, relativePath);
      if (string.IsNullOrEmpty(fullPath))
        return AssetCategoryPath.Root;
      AssetCategoryPath path = new AssetCategoryPath(fullPath, alwaysShow);
      return (AssetCategoryPath) PresetAssetCategoryPath.FromPath(path) ?? path;
    }

    public AssetCategoryPath Append(CustomAssetCategoryPath customPath)
    {
      return this.Append(customPath.CategoryPath, customPath.AlwaysShows);
    }

    public bool Contains(AssetCategoryPath path)
    {
      return AssetCategoryPath.AssetCategoryPathHelper.Contains(this, path);
    }

    public override bool Equals(object obj)
    {
      AssetCategoryPath assetCategoryPath = obj as AssetCategoryPath;
      if (assetCategoryPath != null)
        return this.FullPath.Equals(assetCategoryPath.FullPath);
      return false;
    }

    public override int GetHashCode()
    {
      return this.FullPath.GetHashCode();
    }

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(this.FullPath))
        return this.FullPath.ToString();
      return "[Root]";
    }

    public int CompareTo(AssetCategoryPath other)
    {
      PresetAssetCategoryPath assetCategoryPath1 = this as PresetAssetCategoryPath;
      PresetAssetCategoryPath assetCategoryPath2 = other as PresetAssetCategoryPath;
      if (assetCategoryPath1 != null && assetCategoryPath2 != null)
        return assetCategoryPath1.PresetAssetCategory.CompareTo((object) assetCategoryPath2.PresetAssetCategory);
      int num1 = this.Steps.Length.CompareTo(other.Steps.Length);
      if (num1 != 0)
        return num1;
      int num2 = string.Compare(this.DisplayName, other.DisplayName, StringComparison.OrdinalIgnoreCase);
      if (num2 != 0)
        return num2;
      return string.Compare(this.LastStep, other.LastStep, StringComparison.OrdinalIgnoreCase);
    }

    internal static class AssetCategoryPathHelper
    {
      public static string NormalizePath(string path)
      {
        return Regex.Replace(AssetCategoryPath.AssetCategoryPathHelper.SingleLine(path), "^[/|\\s]+|[/|\\s]+$|(?<=/)[/|\\s]+|\\s+(?=/)|(?<=\\s)\\s+", "");
      }

      private static string SingleLine(string path)
      {
        return Regex.Replace(path, "[\\r\\n]+", " ");
      }

      public static string[] SplitSteps(string normalizedPath)
      {
        if (!string.IsNullOrEmpty(normalizedPath))
          return Regex.Split(normalizedPath, "/");
        return new string[0];
      }

      public static bool IsRootPath(AssetCategoryPath path)
      {
        return string.IsNullOrEmpty(path.FullPath);
      }

      public static string CombinePath(string fullPath, string subPath)
      {
        if (string.IsNullOrEmpty(fullPath))
          return subPath;
        if (string.IsNullOrEmpty(subPath))
          return fullPath;
        return fullPath + "/" + subPath;
      }

      public static bool Contains(AssetCategoryPath parentPath, AssetCategoryPath childPath)
      {
        if (AssetCategoryPath.AssetCategoryPathHelper.IsRootPath(parentPath))
          return true;
        if (AssetCategoryPath.AssetCategoryPathHelper.IsRootPath(childPath))
          return false;
        int num = string.Compare(parentPath.FullPath, childPath.FullPath, StringComparison.OrdinalIgnoreCase);
        if (num == 0)
          return true;
        if (num > 0)
          return false;
        int length1 = parentPath.Steps.Length;
        int length2 = childPath.Steps.Length;
        if (length1 >= length2)
          return false;
        for (int index = 0; index < length1; ++index)
        {
          if (parentPath.Steps[index] != childPath.Steps[index])
            return false;
        }
        return true;
      }

      internal static string RemovePath(string fullPath, string lastStep)
      {
        if (string.IsNullOrEmpty(lastStep))
          return fullPath;
        return fullPath.Remove(fullPath.Length - lastStep.Length - 1);
      }
    }
  }
}
