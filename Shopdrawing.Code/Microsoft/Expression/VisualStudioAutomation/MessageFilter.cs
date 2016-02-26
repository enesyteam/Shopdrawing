// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.VisualStudioAutomation.MessageFilter
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using System;

namespace Microsoft.Expression.VisualStudioAutomation
{
  internal sealed class MessageFilter : IDisposable
  {
    private NativeMethods.IOleMessageFilter messageFilter;

    public MessageFilter()
    {
      NativeMethods.CoRegisterMessageFilter((NativeMethods.IOleMessageFilter) new OleMessageFilter(), out this.messageFilter);
    }

    public void Dispose()
    {
      NativeMethods.CoRegisterMessageFilter(this.messageFilter, out this.messageFilter);
    }
  }
}
