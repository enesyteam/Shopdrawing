// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LayoutSettings
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public class LayoutSettings
  {
    private GridBox gridBox;
    private HorizontalAlignment horizontalAlignment;
    private VerticalAlignment verticalAlignment;
    private double width;
    private double height;
    private Thickness margin;
    private LayoutOverrides layoutOverrides;

    public GridBox GridBox
    {
      get
      {
        return this.gridBox;
      }
      set
      {
        this.gridBox = value;
      }
    }

    public HorizontalAlignment HorizontalAlignment
    {
      get
      {
        return this.horizontalAlignment;
      }
      set
      {
        this.horizontalAlignment = value;
      }
    }

    public VerticalAlignment VerticalAlignment
    {
      get
      {
        return this.verticalAlignment;
      }
      set
      {
        this.verticalAlignment = value;
      }
    }

    public double Width
    {
      get
      {
        return this.width;
      }
      set
      {
        this.width = value;
      }
    }

    public double Height
    {
      get
      {
        return this.height;
      }
      set
      {
        this.height = value;
      }
    }

    public Thickness Margin
    {
      get
      {
        return this.margin;
      }
      set
      {
        this.margin = value;
      }
    }

    public LayoutOverrides LayoutOverrides
    {
      get
      {
        return this.layoutOverrides;
      }
      set
      {
        this.layoutOverrides = value;
      }
    }
  }
}
