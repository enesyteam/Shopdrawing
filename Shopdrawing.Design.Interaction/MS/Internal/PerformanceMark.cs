// Decompiled with JetBrains decompiler
// Type: MS.Internal.PerformanceMark
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Internal.Performance;

namespace MS.Internal
{
  internal struct PerformanceMark
  {
    private string _description;
    private CodeMarkerEvent _beginEvent;
    private CodeMarkerEvent _endEvent;

    public string Description
    {
      get
      {
        return this._description;
      }
    }

    public CodeMarkerEvent BeginEvent
    {
      get
      {
        return this._beginEvent;
      }
    }

    public CodeMarkerEvent EndEvent
    {
      get
      {
        return this._endEvent;
      }
    }

    public PerformanceMark(string description, CodeMarkerEvent beginEvent, CodeMarkerEvent endEvent)
    {
      this._description = description;
      this._beginEvent = beginEvent;
      this._endEvent = endEvent;
    }
  }
}
