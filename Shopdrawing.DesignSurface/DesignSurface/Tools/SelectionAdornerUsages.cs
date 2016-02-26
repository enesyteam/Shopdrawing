// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SelectionAdornerUsages
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.Tools
{
  [Flags]
  public enum SelectionAdornerUsages
  {
    None = 0,
    ShowGeometry = 1,
    ShowBoundingBox = 2,
    ShowDimension = 4,
    ShowClipping = 8,
    ShowMotionPath = 16,
    UseBoundingBox = 32,
    UseFullAdorners = 64,
  }
}
