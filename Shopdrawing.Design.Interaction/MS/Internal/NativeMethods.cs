// Decompiled with JetBrains decompiler
// Type: MS.Internal.NativeMethods
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

namespace MS.Internal
{
  internal static class NativeMethods
  {
    internal const int SM_CXDOUBLECLK = 36;
    internal const int SM_CYDOUBLECLK = 37;
    internal const int SPI_GETMOUSEHOVERWIDTH = 98;
    internal const int SPI_GETMOUSEHOVERHEIGHT = 100;
    internal const int SPI_GETMOUSEHOVERTIME = 102;
    internal const int GWL_STYLE = -16;
    internal const int GWL_EXSTYLE = -20;
    internal const int WS_EX_LAYOUTRTL = 4194304;

    internal struct RECT
    {
      public int left;
      public int top;
      public int right;
      public int bottom;
    }
  }
}
