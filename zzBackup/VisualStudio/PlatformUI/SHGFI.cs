// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SHGFI
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.VisualStudio.PlatformUI
{
  [Flags]
  internal enum SHGFI : uint
  {
    Icon = 256U,
    DisplayName = 512U,
    TypeName = 1024U,
    Attributes = 2048U,
    IconLocation = 4096U,
    ExeType = 8192U,
    SysIconIndex = 16384U,
    LinkOverlay = 32768U,
    Selected = 65536U,
    Attr_Specified = 131072U,
    LargeIcon = 0U,
    SmallIcon = 1U,
    OpenIcon = 2U,
    ShellIconSize = 4U,
    PIDL = 8U,
    UseFileAttributes = 16U,
    AddOverlays = 32U,
    OverlayIndex = 64U,
  }
}
