// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.AnnotationAdornerSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  internal sealed class AnnotationAdornerSet : AdornerSet
  {
    private AnnotationEditor AnnotationEditor
    {
      get
      {
        return this.Element.ViewModel.AnnotationEditor;
      }
    }

    public AnnotationAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
      : base(toolContext, adornedElement)
    {
    }

    protected override void CreateAdorners()
    {
      foreach (AnnotationSceneNode annotation in this.AnnotationEditor.GetAttachedAnnotations(this.Element))
        this.AddAdorner((Adorner) new AnnotationAdorner((AdornerSet) this, annotation));
    }
  }
}
