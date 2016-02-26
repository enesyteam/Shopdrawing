// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.RECT
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI
{
  [Serializable]
  internal struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public Point Position
    {
      get
      {
        return new Point((double) this.Left, (double) this.Top);
      }
    }

    public Size Size
    {
      get
      {
        return new Size((double) this.Width, (double) this.Height);
      }
    }

    public int Height
    {
      get
      {
        return this.Bottom - this.Top;
      }
      set
      {
        this.Bottom = this.Top + value;
      }
    }

    public int Width
    {
      get
      {
        return this.Right - this.Left;
      }
      set
      {
        this.Right = this.Left + value;
      }
    }

    public Rect WPFRectValue
    {
      get
      {
        return new Rect((double) this.Left, (double) this.Top, (double) this.Width, (double) this.Height);
      }
    }

    public RECT(Rect rect)
    {
      this.Left = (int) rect.Left;
      this.Top = (int) rect.Top;
      this.Right = (int) rect.Right;
      this.Bottom = (int) rect.Bottom;
    }
  }
}
