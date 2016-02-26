using System;

namespace Microsoft.Expression.Framework
{
    public interface IExternalChanges
    {
        bool IsDelayed
        {
            get;
        }

        IDisposable DelayNotification();
    }
}