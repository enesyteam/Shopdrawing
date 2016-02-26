// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PropertySceneInsertionPointMarker
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class PropertySceneInsertionPointMarker
  {
    private DocumentNodeMarker marker;
    private IProperty property;

    public bool IsDeleted
    {
      get
      {
        return this.marker.IsDeleted;
      }
    }

    public PropertySceneInsertionPointMarker(PropertySceneInsertionPoint insertionPoint)
    {
      this.marker = insertionPoint.SceneNode.DocumentNode.Marker;
      this.property = insertionPoint.Property;
    }

    public PropertySceneInsertionPoint GetInsertionPoint(SceneViewModel sceneViewModel)
    {
      SceneElement sceneElement = SceneNode.FromMarker<SceneNode>(this.marker, sceneViewModel) as SceneElement;
      if (sceneElement == null)
        return (PropertySceneInsertionPoint) null;
      return new PropertySceneInsertionPoint(sceneElement, this.property);
    }
  }
}
