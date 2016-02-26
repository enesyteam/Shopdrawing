// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.CodeMarkers
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.Performance
{
  internal sealed class CodeMarkers
  {
    public static readonly CodeMarkers Instance = new CodeMarkers();
    private const string AtomName = "VSCodeMarkersEnabled";
    private const string DllName = "Microsoft.Internal.Performance.CodeMarkers.dll";
    private bool fUseCodeMarkers;

    private CodeMarkers()
    {
      this.fUseCodeMarkers = (int) CodeMarkers.NativeMethods.FindAtom("VSCodeMarkersEnabled") != 0;
    }

    public void CodeMarker(CodeMarkerEvent nTimerID)
    {
      if (!this.fUseCodeMarkers)
        return;
      try
      {
        CodeMarkers.NativeMethods.DllPerfCodeMarker((int) nTimerID, (byte[]) null, 0);
      }
      catch (DllNotFoundException ex)
      {
        this.fUseCodeMarkers = false;
      }
    }

    public void CodeMarkerEx(CodeMarkerEvent nTimerID, byte[] aBuff)
    {
      if (aBuff == null)
        throw new ArgumentNullException("aBuff");
      if (!this.fUseCodeMarkers)
        return;
      try
      {
        CodeMarkers.NativeMethods.DllPerfCodeMarker((int) nTimerID, aBuff, aBuff.Length);
      }
      catch (DllNotFoundException ex)
      {
        this.fUseCodeMarkers = false;
      }
    }

    private static class NativeMethods
    {
      [DllImport("Microsoft.Internal.Performance.CodeMarkers.dll", EntryPoint = "PerfCodeMarker")]
      public static extern void DllPerfCodeMarker(int nTimerID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] aUserParams, int cbParams);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
      public static extern ushort FindAtom(string lpString);
    }
  }
}
