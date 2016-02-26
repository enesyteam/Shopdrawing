// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ExtendedModifierKeys
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.Tools
{
  [Flags]
  public enum ExtendedModifierKeys
  {
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Windows = 8,
    Space = 16,
    ControlSpace = Space | Control,
    ControlAltSpace = ControlSpace | Alt,
    ControlAlt = Control | Alt,
    ControlShift = Shift | Control,
    ControlAltShift = ControlShift | Alt,
    AltShift = Shift | Alt,
  }
}
