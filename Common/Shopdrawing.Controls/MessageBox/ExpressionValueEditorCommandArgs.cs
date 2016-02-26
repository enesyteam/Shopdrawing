using System;
using System.Windows;

namespace Microsoft.Expression.Framework.ValueEditors
{
    public sealed class ExpressionValueEditorCommandArgs
    {
        private IInputElement inputElement;

        private object parameter;

        public IInputElement InputElement
        {
            get
            {
                return this.inputElement;
            }
        }

        public object Parameter
        {
            get
            {
                return this.parameter;
            }
        }

        public ExpressionValueEditorCommandArgs(IInputElement inputElement, object parameter)
        {
            this.inputElement = inputElement;
            this.parameter = parameter;
        }
    }
}