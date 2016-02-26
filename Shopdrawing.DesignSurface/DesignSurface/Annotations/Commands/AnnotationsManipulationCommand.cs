// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.Commands.AnnotationsManipulationCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Annotations.Commands
{
  internal abstract class AnnotationsManipulationCommand : AnnotationsCommand
  {
    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled && this.AnnotationService.ShowAnnotations)
          return Enumerable.Any<AnnotationSceneNode>(this.Annotations);
        return false;
      }
    }

    protected AnnotationsManipulationCommand(SceneView view)
      : base(view)
    {
    }
  }
}
