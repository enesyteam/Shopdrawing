// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LayoutLockAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public abstract class LayoutLockAdorner : LayoutAdorner, IClickable
  {
    protected abstract ImageSource LockImage { get; }

    protected abstract ImageSource UnlockImage { get; }

    protected virtual ImageSource AutoImage
    {
      get
      {
        return (ImageSource) null;
      }
    }

    protected abstract LayoutLockState LayoutLockState { get; }

    protected abstract double Value { get; }

    protected abstract bool ParentRelative { get; }

    protected LayoutLockAdorner(AdornerSet adornerSet, bool isX)
      : base(adornerSet, isX)
    {
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return this.GetCenter(matrix);
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      if (this.ParentRelative && this.Element.ParentElement == null || this.Element.Visual == null)
        return;
      ImageSource imageSource = (ImageSource) null;
      switch (this.LayoutLockState)
      {
        case LayoutLockState.Locked:
          imageSource = this.LockImage;
          break;
        case LayoutLockState.Unlocked:
          imageSource = this.UnlockImage;
          break;
        case LayoutLockState.Neither:
          imageSource = this.AutoImage;
          break;
      }
      Point point = this.GetCenter(matrix);
      if (PresentationSource.FromVisual((Visual) this) != null)
        point = this.PointFromScreen(this.PointToScreen(point));
      if (this.ParentRelative)
        matrix = this.Element.GetComputedTransformFromVisualParent() * matrix;
      Matrix matrix1 = this.TruncateMatrix(matrix);
      matrix1.OffsetX = point.X;
      matrix1.OffsetY = point.Y;
      MatrixTransform matrixTransform = new MatrixTransform(matrix1);
      matrixTransform.Freeze();
      drawingContext.PushTransform((Transform) matrixTransform);
      drawingContext.DrawImage(imageSource, new Rect(-imageSource.Width / 2.0, -imageSource.Height / 2.0, imageSource.Width, imageSource.Height));
      drawingContext.Pop();
    }

    protected abstract Point GetCenter(Matrix matrix);
  }
}
