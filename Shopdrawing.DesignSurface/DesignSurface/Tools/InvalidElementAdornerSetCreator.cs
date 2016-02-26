// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.InvalidElementAdornerSetCreator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class InvalidElementAdornerSetCreator : IAdornerSetCreator
  {
    public IAdornerSet CreateAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
    {
      return (IAdornerSet) new InvalidElementAdornerSetCreator.InvalidElementAdornerSet(toolContext, adornedElement);
    }

    private class InvalidElementAdornerSet : AdornerSet
    {
      private List<Size> cachedSizes;
      private List<Matrix> cachedTransforms;

      public InvalidElementAdornerSet(ToolBehaviorContext toolContext, SceneElement adornedElement)
        : base(toolContext, adornedElement)
      {
      }

      protected override void CreateAdorners()
      {
        InvalidElementAdorner invalidElementAdorner = new InvalidElementAdorner((AdornerSet) this);
        this.AddAdorner((Adorner) invalidElementAdorner);
        this.cachedSizes = new List<Size>(invalidElementAdorner.TargetElements.Count);
        this.cachedTransforms = new List<Matrix>(invalidElementAdorner.TargetElements.Count);
        for (int index = 0; index < invalidElementAdorner.TargetElements.Count; ++index)
        {
          Size size;
          Matrix matrix;
          this.GetElementLayout(invalidElementAdorner.TargetElements[index], out size, out matrix);
          this.cachedSizes.Add(size);
          this.cachedTransforms.Add(matrix);
        }
      }

      protected override void OnRedrawing()
      {
        InvalidElementAdorner invalidElementAdorner = this.AdornerList == null || this.AdornerList.Count <= 0 ? (InvalidElementAdorner) null : (InvalidElementAdorner) this.AdornerList[0];
        if (invalidElementAdorner == null)
          return;
        SceneView defaultView = this.Element.ViewModel.DefaultView;
        bool flag = this.cachedSizes == null || this.cachedTransforms == null;
        for (int index = 0; !flag && index < invalidElementAdorner.TargetElements.Count; ++index)
        {
          SceneElement sceneElement = invalidElementAdorner.TargetElements[index];
          if (defaultView.IsInArtboard(sceneElement))
          {
            Size size;
            Matrix matrix;
            this.GetElementLayout(sceneElement, out size, out matrix);
            flag = !size.Equals(this.cachedSizes[index]) || matrix != this.cachedTransforms[index];
          }
          else
            flag = true;
        }
        if (!flag)
          return;
        this.InvalidateStructure();
      }

      private void GetElementLayout(SceneElement sceneElement, out Size size, out Matrix matrix)
      {
        size = sceneElement.RenderSize;
        matrix = sceneElement.TransformToArtboard;
      }
    }
  }
}
