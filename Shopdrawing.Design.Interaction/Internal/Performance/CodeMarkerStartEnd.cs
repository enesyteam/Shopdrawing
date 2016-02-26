// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.CodeMarkerStartEnd
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;

namespace Microsoft.Internal.Performance
{
  internal sealed class CodeMarkerStartEnd : IDisposable
  {
    private CodeMarkerEvent _end;

    public CodeMarkerStartEnd(CodeMarkerEvent begin, CodeMarkerEvent end)
    {
      CodeMarkers.Instance.CodeMarker(begin);
      this._end = end;
    }

    public void Dispose()
    {
      if (this._end == (CodeMarkerEvent) 0)
        return;
      CodeMarkers.Instance.CodeMarker(this._end);
      this._end = (CodeMarkerEvent) 0;
    }
  }
}
