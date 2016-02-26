// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ChildPropertyInsertionPointCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface
{
  internal class ChildPropertyInsertionPointCreator : IInsertionPointCreator
  {
    public IProperty ChildProperty { get; private set; }

    public SceneElement Element { get; private set; }

    public ChildPropertyInsertionPointCreator(SceneElement element, IProperty childProperty)
    {
      this.Element = element;
      this.ChildProperty = childProperty;
    }

    public ISceneInsertionPoint Create(object data)
    {
      if (this.Element != null || this.ChildProperty != null)
        return (ISceneInsertionPoint) new PropertySceneInsertionPoint(this.Element, this.ChildProperty);
      return (ISceneInsertionPoint) null;
    }
  }
}
