// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.RelativePositions
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

namespace Microsoft.Windows.Design.Interaction
{
  public static class RelativePositions
  {
    private static RelativePosition _topLeft;
    private static RelativePosition _topSide;
    private static RelativePosition _topRight;
    private static RelativePosition _rightSide;
    private static RelativePosition _bottomRight;
    private static RelativePosition _bottomSide;
    private static RelativePosition _bottomLeft;
    private static RelativePosition _leftSide;
    private static RelativePosition _center;
    private static RelativePosition[] _bounds;
    private static RelativePosition _internalLeftSide;
    private static RelativePosition _internalTopSide;
    private static RelativePosition _internalBottomSide;
    private static RelativePosition _internalRightSide;
    private static RelativePosition _internalTopLeft;
    private static RelativePosition _internalTopRight;
    private static RelativePosition _internalBottomLeft;
    private static RelativePosition _internalBottomRight;
    private static RelativePosition _externalLeftSide;
    private static RelativePosition _externalTopSide;
    private static RelativePosition _externalBottomSide;
    private static RelativePosition _externalRightSide;
    private static RelativePosition _externalTopLeft;
    private static RelativePosition _externalTopRight;
    private static RelativePosition _externalBottomLeft;
    private static RelativePosition _externalBottomRight;

    public static RelativePosition InternalLeftSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._internalLeftSide, "InternalLeftSide");
      }
    }

    public static RelativePosition InternalTopSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._internalTopSide, "InternalTopSide");
      }
    }

    public static RelativePosition InternalBottomSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._internalBottomSide, "InternalBottomSide");
      }
    }

    public static RelativePosition InternalRightSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._internalRightSide, "InternalRightSide");
      }
    }

    public static RelativePosition InternalBottomLeft
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._internalBottomLeft, "InternalBottomLeft", RelativePositions.InternalBottomSide, RelativePositions.InternalLeftSide);
      }
    }

    public static RelativePosition InternalBottomRight
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._internalBottomRight, "InternalBottomRight", RelativePositions.InternalBottomSide, RelativePositions.InternalRightSide);
      }
    }

    public static RelativePosition InternalTopLeft
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._internalTopLeft, "InternalTopLeft", RelativePositions.InternalTopSide, RelativePositions.InternalLeftSide);
      }
    }

    public static RelativePosition InternalTopRight
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._internalTopRight, "InternalTopRight", RelativePositions.InternalTopSide, RelativePositions.InternalRightSide);
      }
    }

    public static RelativePosition ExternalLeftSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._externalLeftSide, "ExternalLeftSide");
      }
    }

    public static RelativePosition ExternalTopSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._externalTopSide, "ExternalTopSide");
      }
    }

    public static RelativePosition ExternalBottomSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._externalBottomSide, "ExternalBottomSide");
      }
    }

    public static RelativePosition ExternalRightSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._externalRightSide, "ExternalRightSide");
      }
    }

    public static RelativePosition ExternalBottomLeft
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._externalBottomLeft, "ExternalBottomLeft", RelativePositions.ExternalBottomSide, RelativePositions.ExternalLeftSide);
      }
    }

    public static RelativePosition ExternalBottomRight
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._externalBottomRight, "ExternalBottomRight", RelativePositions.ExternalBottomSide, RelativePositions.ExternalRightSide);
      }
    }

    public static RelativePosition ExternalTopLeft
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._externalTopLeft, "ExternalTopLeft", RelativePositions.ExternalTopSide, RelativePositions.ExternalLeftSide);
      }
    }

    public static RelativePosition ExternalTopRight
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._externalTopRight, "ExternalTopRight", RelativePositions.ExternalTopSide, RelativePositions.ExternalRightSide);
      }
    }

    public static RelativePosition[] Bounds
    {
      get
      {
        if (RelativePositions._bounds == null)
          RelativePositions._bounds = new RelativePosition[4]
          {
            RelativePositions.LeftSide,
            RelativePositions.TopSide,
            RelativePositions.RightSide,
            RelativePositions.BottomSide
          };
        return RelativePositions._bounds;
      }
    }

    public static RelativePosition BottomLeft
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._bottomLeft, "BottomLeft", RelativePositions.BottomSide, RelativePositions.LeftSide);
      }
    }

    public static RelativePosition BottomRight
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._bottomRight, "BottomRight", RelativePositions.BottomSide, RelativePositions.RightSide);
      }
    }

    public static RelativePosition BottomSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._bottomSide, "BottomSide");
      }
    }

    public static RelativePosition Center
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._center, "Center");
      }
    }

    public static RelativePosition LeftSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._leftSide, "LeftSide");
      }
    }

    public static RelativePosition RightSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._rightSide, "RightSide");
      }
    }

    public static RelativePosition TopLeft
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._topLeft, "TopLeft", RelativePositions.TopSide, RelativePositions.LeftSide);
      }
    }

    public static RelativePosition TopRight
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._topRight, "TopRight", RelativePositions.TopSide, RelativePositions.RightSide);
      }
    }

    public static RelativePosition TopSide
    {
      get
      {
        return RelativePositions.GetPosition(ref RelativePositions._topSide, "TopSide");
      }
    }

    private static RelativePosition GetPosition(ref RelativePosition pos, string name, params RelativePosition[] values)
    {
      if (object.ReferenceEquals((object) pos, (object) null))
        pos = new RelativePosition(name, values);
      return pos;
    }
  }
}
