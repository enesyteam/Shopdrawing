using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Framework
{
    public class ErrorArgs
    {
        public string AutomationId
        {
            get;
            set;
        }

        public Exception Exception
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public ErrorArgs()
        {
        }
    }
}