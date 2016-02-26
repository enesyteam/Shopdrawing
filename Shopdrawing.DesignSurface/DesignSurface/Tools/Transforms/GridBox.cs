// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.GridBox
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public class GridBox
  {
    private int colBegin;
    private int colEnd;
    private int rowBegin;
    private int rowEnd;

    public int ColBegin
    {
      get
      {
        return this.colBegin;
      }
      set
      {
        this.colBegin = value;
      }
    }

    public int ColEnd
    {
      get
      {
        return this.colEnd;
      }
      set
      {
        this.colEnd = value;
      }
    }

    public int ColSpan
    {
      get
      {
        return Math.Max(1, this.colEnd - this.colBegin);
      }
      set
      {
        this.colEnd = this.colBegin + value;
      }
    }

    public int RowBegin
    {
      get
      {
        return this.rowBegin;
      }
      set
      {
        this.rowBegin = value;
      }
    }

    public int RowEnd
    {
      get
      {
        return this.rowEnd;
      }
      set
      {
        this.rowEnd = value;
      }
    }

    public int RowSpan
    {
      get
      {
        return Math.Max(1, this.rowEnd - this.rowBegin);
      }
      set
      {
        this.rowEnd = this.rowBegin + value;
      }
    }

    public GridBox(int colBegin, int colEnd, int rowBegin, int rowEnd)
    {
      this.colBegin = colBegin;
      this.colEnd = colEnd;
      this.rowBegin = rowBegin;
      this.rowEnd = rowEnd;
    }
  }
}
