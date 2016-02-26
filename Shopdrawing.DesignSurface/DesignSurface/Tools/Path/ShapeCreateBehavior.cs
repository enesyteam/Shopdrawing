// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.ShapeCreateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class ShapeCreateBehavior : TypeElementDragCreateBehavior
  {
    internal ShapeCreateBehavior(ToolBehaviorContext toolContext, ITypeId elementType)
      : base(toolContext, elementType, false)
    {
    }

    protected override void OnApplyAmbientProperty(SceneNode node)
    {
      using (this.ActiveSceneViewModel.ForceBaseValue())
        this.ToolBehaviorContext.AmbientPropertyManager.ApplyAmbientProperties(node);
    }
  }
}
