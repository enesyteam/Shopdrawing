// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SnapLine
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class SnapLine
  {
    private Point p1;
    private Point p2;
    private SnapLineOrientation orientation;
    private SnapLineLocation location;
    private bool isContainerLine;
    private SnapLineLocation locationRelativeToTarget;
    private double offsetRelativeToTarget;

    public SnapLineOrientation Orientation
    {
      get
      {
        return this.orientation;
      }
    }

    public SnapLineLocation Location
    {
      get
      {
        return this.location;
      }
    }

    public bool IsContainerLine
    {
      get
      {
        return this.isContainerLine;
      }
    }

    public SnapLineLocation LocationRelativeToTarget
    {
      get
      {
        return this.locationRelativeToTarget;
      }
      set
      {
        this.locationRelativeToTarget = value;
      }
    }

    public double OffsetRelativeToTarget
    {
      get
      {
        return this.offsetRelativeToTarget;
      }
      set
      {
        this.offsetRelativeToTarget = value;
      }
    }

    public Point P1
    {
      get
      {
        return this.p1;
      }
    }

    public Point P2
    {
      get
      {
        return this.p2;
      }
    }

    public SnapLine(Point p1, Point p2, SnapLineOrientation orientation, SnapLineLocation location, bool isContainerLine)
    {
      this.p1 = p1;
      this.p2 = p2;
      this.orientation = orientation;
      this.location = location;
      this.isContainerLine = isContainerLine;
    }

    public bool RangeOverlaps(Point p1, Point p2)
    {
      if (this.orientation == SnapLineOrientation.Horizontal)
      {
        if (this.P1.X <= p2.X)
          return p1.X <= this.P2.X;
        return false;
      }
      if (this.P1.Y <= p2.Y)
        return p1.Y <= this.P2.Y;
      return false;
    }

    public double GetSignedDistance(Point point)
    {
      if (this.orientation == SnapLineOrientation.Horizontal)
        return this.P1.Y - point.Y;
      return this.P1.X - point.X;
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SnapLine: {0} {1} ({2}) - ({3})", (object) this.Orientation, (object) this.Location, (object) this.P1, (object) this.P2);
    }
  }
}
