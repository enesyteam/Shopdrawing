// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathPartAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal abstract class PathPartAdorner : Adorner
  {
    private int segmentIndex = -1;
    private int figureIndex;
    private int partIndex;
    private int segmentPointIndex;

    public PathAdornerSet PathAdornerSet
    {
      get
      {
        return (PathAdornerSet) this.AdornerSet;
      }
    }

    public BaseFrameworkElement Element
    {
      get
      {
        return this.PathAdornerSet.Element;
      }
    }

    public PathGeometry PathGeometry
    {
      get
      {
        return this.PathAdornerSet.PathGeometry;
      }
    }

    public Matrix PathGeometryTransformMatrix
    {
      get
      {
        Transform transform = this.PathGeometry.Transform;
        if (transform != null)
          return transform.Value;
        return Matrix.Identity;
      }
    }

    public int FigureIndex
    {
      get
      {
        return this.figureIndex;
      }
    }

    public int PartIndex
    {
      get
      {
        return this.partIndex;
      }
    }

    public int SegmentIndex
    {
      get
      {
        return this.segmentIndex;
      }
    }

    public int SegmentPointIndex
    {
      get
      {
        return this.segmentPointIndex;
      }
    }

    public bool IsValid
    {
      get
      {
        PathGeometry pathGeometry = this.PathGeometry;
        if (pathGeometry == null || this.figureIndex >= pathGeometry.Figures.Count || this.segmentIndex >= pathGeometry.Figures[this.figureIndex].Segments.Count)
          return false;
        if (this.segmentIndex != -1)
          return this.segmentPointIndex < PathSegmentUtilities.GetPointCount(pathGeometry.Figures[this.figureIndex].Segments[this.segmentIndex]);
        return true;
      }
    }

    protected Pen ThinPathPen
    {
      get
      {
        switch (this.PathAdornerSet.PathEditorTarget.PathEditMode)
        {
          case PathEditMode.MotionPath:
            return FeedbackHelper.GetThinPen(AdornerType.MotionPath);
          case PathEditMode.ClippingPath:
            return FeedbackHelper.GetThinPen(AdornerType.ClipPath);
          default:
            return this.ThinPen;
        }
      }
    }

    protected Pen ThickPathPen
    {
      get
      {
        switch (this.PathAdornerSet.PathEditorTarget.PathEditMode)
        {
          case PathEditMode.MotionPath:
            return FeedbackHelper.GetThickPen(AdornerType.MotionPath);
          case PathEditMode.ClippingPath:
            return FeedbackHelper.GetThickPen(AdornerType.ClipPath);
          default:
            return this.ThickPen;
        }
      }
    }

    protected Pen ThinPathSegmentPen
    {
      get
      {
        switch (this.PathAdornerSet.PathEditorTarget.PathEditMode)
        {
          case PathEditMode.MotionPath:
            return FeedbackHelper.GetThinPen(AdornerType.MotionPathSegment);
          case PathEditMode.ClippingPath:
            return FeedbackHelper.GetThinPen(AdornerType.ClipPathSegment);
          default:
            return this.ThinPen;
        }
      }
    }

    protected Pen ThickPathSegmentPen
    {
      get
      {
        switch (this.PathAdornerSet.PathEditorTarget.PathEditMode)
        {
          case PathEditMode.MotionPath:
            return FeedbackHelper.GetThickPen(AdornerType.MotionPathSegment);
          case PathEditMode.ClippingPath:
            return FeedbackHelper.GetThickPen(AdornerType.ClipPathSegment);
          default:
            return this.ThickPen;
        }
      }
    }

    public bool IsHighlighted
    {
      get
      {
        if (this.IsHighlightedOverride && this.PathAdornerSet.RenderHighlight)
          return this.PathAdornerSet.HighlightFigureIndex == this.figureIndex;
        return false;
      }
    }

    protected abstract bool IsHighlightedOverride { get; }

    public PathPartAdorner(PathAdornerSet pathAdornerSet, int figureIndex, int partIndex, int segmentIndex, int segmentPointIndex)
      : base((AdornerSet) pathAdornerSet)
    {
      this.Initialize(figureIndex, partIndex, segmentIndex, segmentPointIndex);
    }

    public void Initialize(int figureIndex, int partIndex, int segmentIndex, int segmentPointIndex)
    {
      this.figureIndex = figureIndex;
      this.partIndex = partIndex;
      this.segmentIndex = segmentIndex;
      this.segmentPointIndex = segmentPointIndex;
    }

    protected override void OnIsActiveChanged()
    {
    }

    public abstract PathPart ToPathPart();
  }
}
