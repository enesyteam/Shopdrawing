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

        public void Dispose()
        {
            Mouse.OverrideCursor = this.oldCursor;
        }

        public static IDisposable SetCursor(Cursor cursor)
        {
            if (cursor == Mouse.OverrideCursor)
            {
                return null;
            }
            return new TemporaryCursor(cursor);
        }

        public static IDisposable SetWaitCursor()
        {
            return TemporaryCursor.SetCursor(Cursors.Wait);
        }
    }
}