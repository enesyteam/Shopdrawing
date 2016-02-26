// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.MaterialEditor.EmissiveMaterialEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.MaterialEditor
{
  public class EmissiveMaterialEditor : MaterialBaseEditor
  {
    public override string Name
    {
      get
      {
        return StringTable.EmissiveMaterialName;
      }
    }

    public override string ColorName
    {
      get
      {
        return StringTable.EmissiveMaterialColorName;
      }
    }

    public override Type Type
    {
      get
      {
        return typeof (EmissiveMaterial);
      }
    }

    public EmissiveMaterialEditor(SceneNodeObjectSet objectSet, PropertyReference materialReference)
      : base(objectSet, materialReference)
    {
      this.ColorProperty = objectSet.CreateSceneNodeProperty(materialReference.Append(MaterialNode.EmissiveColorProperty), (AttributeCollection) null);
      this.BrushProperty = objectSet.CreateSceneNodeProperty(materialReference.Append(MaterialNode.EmissiveBrushProperty), (AttributeCollection) null);
    }
  }
}
