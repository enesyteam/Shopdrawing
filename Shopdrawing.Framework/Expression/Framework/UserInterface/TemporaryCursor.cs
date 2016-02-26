// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.TemporaryCursor
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.UserInterface
{
  public sealed class TemporaryCursor : IDisposable
  {
    private Cursor oldCursor;

    private TemporaryCursor(Cursor cursor)
    {
      this.oldCursor = Mouse.OverrideCursor;
      Mouse.OverrideCursor = cursor;
    }

    public static IDisposable SetCursor(Cursor cursor)
    {
      if (cursor == Mouse.OverrideCursor)
        return (IDisposable) null;
      return (IDisposable) new TemporaryCursor(cursor);
    }

    public static IDisposable SetWaitCursor()
    {
      return TemporaryCursor.SetCursor(Cursors.Wait);
    }

    public void Dispose()
    {
      Mouse.OverrideCursor = this.oldCursor;
    }
  }
}
