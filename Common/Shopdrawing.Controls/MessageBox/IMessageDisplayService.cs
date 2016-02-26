using Microsoft.Expression.Framework.Controls;
using System;
using System.Windows;

namespace Microsoft.Expression.Framework
{
    public interface IMessageDisplayService
    {
        void ShowError(string message);

        void ShowError(string message, string caption);

        void ShowError(string message, Exception exception, string caption);

        void ShowError(ErrorArgs args);

        MessageBoxResult ShowMessage(string message);

        MessageBoxResult ShowMessage(string message, string caption);

        MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button);

        MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage image);

        MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage image, MessageChoice defaultChoice);

        MessageBoxResult ShowMessage(Window owner, string message, string caption, MessageBoxButton button, MessageBoxImage image);

        MessageBoxResult ShowMessage(Window owner, string message, string caption, MessageBoxButton button, MessageBoxImage image, MessageChoice defaultChoice);

        MessageBoxResult ShowMessage(MessageBoxArgs args);

        MessageBoxResult ShowMessage(MessageBoxArgs args, out bool doNotAskAgain);
    }
}