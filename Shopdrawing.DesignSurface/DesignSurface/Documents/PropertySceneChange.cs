// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.PropertySceneChange
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal abstract class PropertySceneChange : SceneChange
  {
    public SceneNode Target
    {
      get
      {
        return this.Parent;
      }
    }

    public abstract IPropertyId PropertyKey { get; }

    public PropertySceneChange(SceneNode parent)
      : base(parent)
    {
    }
  }
}
