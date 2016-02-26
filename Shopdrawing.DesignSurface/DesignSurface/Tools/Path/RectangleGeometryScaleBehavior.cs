// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.RectangleGeometryScaleBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal class RectangleGeometryScaleBehavior : ScaleBehavior
  {
    private RectangleGeometry startGeometry;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitScaleClippingRectangle;
      }
    }

    protected override bool UseSnappingEngine
    {
      get
      {
        return false;
      }
    }

    public RectangleGeometryScaleBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void Initialize()
    {
      this.startGeometry = this.EditingElement.GetLocalValueAsWpf(Base2DElement.ClipProperty) as RectangleGeometry;
      if (this.startGeometry == null)
        return;
      this.StartBounds = this.startGeometry.Rect;
    }

    protected override void ApplyScale(Vector scale, Point center)
    {
      Rect rect1 = this.startGeometry.Rect;
      double x = center.X - (center.X - rect1.X) * scale.X;
      double y = center.Y - (center.Y - rect1.Y) * scale.Y;
      double num1 = rect1.Width * scale.X;
      double num2 = rect1.Height * scale.Y;
      Rect rect2 = new Rect(new Point(x, y), new Point(x + num1, y + num2));
      RectangleGeometry rectangleGeometry = this.startGeometry.Clone();
      rectangleGeometry.Rect = RoundingHelper.RoundRect(rect2);
      this.EditingElement.SetLocalValueAsWpf(Base2DElement.ClipProperty, (object) rectangleGeometry);
    }

    protected override Matrix ComputeElementToDocumentTransform()
    {
      Matrix documentTransform = base.ComputeElementToDocumentTransform();
      documentTransform.Prepend(RectangleGeometryAdornerSetBase.GetRectangleClipGeometryTransform(this.EditingElement));
      return documentTransform;
    }
  }
}
