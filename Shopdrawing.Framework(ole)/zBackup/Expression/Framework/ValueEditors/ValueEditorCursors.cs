// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ValueEditorCursors
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public static class ValueEditorCursors
  {
    private static Cursor eyedropperCursor;
    private static Cursor numericalAdjustCursor;

    public static Cursor EyedropperCursor
    {
      get
      {
        return ValueEditorCursors.GetCachedCursor("Resources\\Cursors\\EyedropperCursor.cur", ref ValueEditorCursors.eyedropperCursor);
      }
    }

    public static Cursor NumericalAdjustCursor
    {
      get
      {
        return ValueEditorCursors.GetCachedCursor("Resources\\Cursors\\cursor_numericalAdjust.cur", ref ValueEditorCursors.numericalAdjustCursor);
      }
    }

    private static Cursor GetCachedCursor(string resourceName, ref Cursor cursorCache)
    {
      if (cursorCache == null)
        cursorCache = FileTable.GetCursor(resourceName);
      return cursorCache;
    }
  }
}
