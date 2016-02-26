// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SceneNodeCategoryEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  internal abstract class SceneNodeCategoryEditor : CategoryEditor
  {
    public SceneNodeCategoryEditor()
    {
      base.\u002Ector();
    }

    public virtual object GetHighlightedImage(Size size)
    {
      return this.GetImage(size);
    }
  }
}
