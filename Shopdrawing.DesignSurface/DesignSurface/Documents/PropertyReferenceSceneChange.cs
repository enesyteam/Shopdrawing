// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.PropertyReferenceSceneChange
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal class PropertyReferenceSceneChange : PropertySceneChange
  {
    private PropertyReference propertyChanged;

    public PropertyReference PropertyChanged
    {
      get
      {
        return this.propertyChanged;
      }
    }

    public override IPropertyId PropertyKey
    {
      get
      {
        return (IPropertyId) this.propertyChanged[0];
      }
    }

    public PropertyReferenceSceneChange(SceneNode parent, PropertyReference propertyChanged)
      : base(parent)
    {
      this.propertyChanged = propertyChanged;
    }
  }
}
