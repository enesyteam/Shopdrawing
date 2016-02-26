// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.EventTrace
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Security;

namespace Microsoft.Expression.Framework.Diagnostics
{
  internal sealed class EventTrace
  {
    internal static readonly Guid INFOGUID = new Guid("{0982540a-1035-45ba-b1a3-a80e16959c28}");
    internal static readonly TraceProvider EventProvider = (TraceProvider) null;
    private const int INFOIDOFFSET = 100;
    internal const int LOADHWNDDISPATCHER = 101;
    internal const int DISPATCHERIDLE = 102;
    internal const int INPUTKEYQUEUE = 103;
    internal const int FONTCACHELIMIT = 104;
    internal const int GLYPHCACHEMISS = 105;
    internal const int GLYPHRUNINFO = 106;
    internal const int CONNECTEDDATA = 107;
    internal const int GENERICSTRING = 108;
    internal const int UIAUTOMATIONINFO = 109;
    internal const int REQUESTCONTENT = 110;
    internal const int RENEWABLECACHEFULL = 111;

    [SecuritySafeCritical]
    static EventTrace()
    {
      EventTrace.EventProvider = new TraceProvider("YourProvider", new Guid("{...}"));
    }

    private EventTrace()
    {
    }

    internal static bool IsEnabled(EventTrace.Flags flag, EventTrace.Level level)
    {
      return EventTrace.EventProvider != null && EventTrace.EventProvider.IsEnabled && (Convert.ToBoolean((uint) (flag & (EventTrace.Flags) EventTrace.EventProvider.Flags)) && (uint) level <= EventTrace.EventProvider.Level);
    }

    [System.Flags]
    internal enum Flags
    {
      debugging = 1,
      performance = 2,
      stress = 4,
      security = 8,
      uiautomation = 16,
      response = 32,
      trace = 64,
      all = 2147483647,
    }

    internal enum Level
    {
      fatal = 1,
      error = 2,
      warning = 3,
      normal = 4,
      verbose = 5,
    }
  }
}
