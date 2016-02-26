// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.RelativePoint
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System.Windows;

namespace Microsoft.Windows.Design.Interaction
{
  public struct RelativePoint
  {
    private RelativePosition _position;
    private double _x;
    private double _y;

    public RelativePosition Position
    {
      get
      {
        if (this._position == (RelativePosition) null)
          this._position = RelativePositions.TopLeft;
        return this._position;
      }
      set
      {
        this._position = value;
      }
    }

    public double X
    {
      get
      {
        return this._x;
      }
      set
      {
        this._x = value;
      }
    }

    public double Y
    {
      get
      {
        return this._y;
      }
      set
      {
        this._y = value;
      }
    }

    public RelativePoint(RelativePosition position, double x, double y)
    {
      this._position = position;
      this._x = x;
      this._y = y;
    }

    public RelativePoint(RelativePosition position, Point point)
    {
      this._position = position;
      this._x = point.X;
      this._y = point.Y;
    }

    public static implicit operator RelativePoint(Point point)
    {
      return new RelativePoint(RelativePositions.TopLeft, point);
    }

    public static bool operator ==(RelativePoint point1, RelativePoint point2)
    {
      if (point1.Position == point2.Position && point1.X == point2.X)
        return point1.Y == point2.Y;
      return false;
    }

    public static bool operator !=(RelativePoint point1, RelativePoint point2)
    {
      if (!(point1.Position != point2.Position) && point1.X == point2.X)
        return point1.Y != point2.Y;
      return true;
    }

    public override bool Equals(object obj)
    {
      if (obj is RelativePoint)
        return this.Equals((RelativePoint) obj);
      return false;
    }

    public bool Equals(RelativePoint value)
    {
      if (this.Position == value.Position && this.X == value.X)
        return this.Y == value.Y;
      return false;
    }

    public static RelativePoint FromPoint(Point point)
    {
      return new RelativePoint(RelativePositions.TopLeft, point);
    }

    public override int GetHashCode()
    {
      return this.X.GetHashCode() ^ this.Y.GetHashCode();
    }
  }
}
