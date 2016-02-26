using Microsoft.Expression.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Microsoft.Expression.Framework
{
    public class MessageBoxArgs
    {
        private MessageBoxButton button;

        private MessageBoxImage image;

        public string AutomationId
        {
            get;
            set;
        }

        public MessageBoxButton Button
        {
            get
            {
                return this.button;
            }
            set
            {
                this.button = value;
            }
        }

        public string CheckBoxMessage
        {
            get;
            set;
        }

        public string HyperlinkMessage
        {
            get;
            set;
        }

        public Uri HyperlinkUri
        {
            get;
            set;
        }

        public MessageBoxImage Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
            }
        }

        public string Message
        {
            get;
            set;
        }

        public Window Owner
        {
            get;
            set;
        }

        public IDictionary<MessageChoice, string> TextOverrides
        {
            get;
            set;
        }

        public MessageBoxArgs()
        {
        }
    }
}