using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Expression.Framework
{
    public static class WebPageLauncher
    {
        public static bool Navigate(Uri uri, IMessageDisplayService messageDisplayService)
        {
            bool flag;
            try
            {
                Process.Start(uri.OriginalString);
                return true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (!(exception is ObjectDisposedException) && !(exception is Win32Exception) && !(exception is ArgumentException) && !(exception is InvalidOperationException))
                {
                    throw;
                }
                if (messageDisplayService != null)
                {
                    CultureInfo currentCulture = CultureInfo.CurrentCulture;
                    string webNavigationFailedMessage = StringTable.WebNavigationFailedMessage;
                    object[] originalString = new object[] { uri.OriginalString };
                    messageDisplayService.ShowError(string.Format(currentCulture, webNavigationFailedMessage, originalString));
                }
                flag = false;
            }
            return flag;
        }
    }
}