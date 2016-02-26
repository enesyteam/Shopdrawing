using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.ValueEditors
{
    public static class ValueEditorUtils
    {
        public readonly static DependencyProperty HandlesCommitKeysProperty;

        static ValueEditorUtils()
        {
            ValueEditorUtils.HandlesCommitKeysProperty = DependencyProperty.RegisterAttached("HandlesCommitKeys", typeof(bool), typeof(ValueEditorUtils), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
        }

        public static bool ExecuteCommand(ICommand command, IInputElement element, object parameter)
        {
            RoutedCommand routedCommand = command as RoutedCommand;
            if (routedCommand != null)
            {
                if (routedCommand.CanExecute(parameter, element))
                {
                    routedCommand.Execute(parameter, element);
                    return true;
                }
            }
            else if (command is ExpressionPropertyValueEditorCommand)
            {
                ExpressionValueEditorCommandArgs expressionValueEditorCommandArg = new ExpressionValueEditorCommandArgs(element, parameter);
                if (command.CanExecute(expressionValueEditorCommandArg))
                {
                    command.Execute(expressionValueEditorCommandArg);
                    return true;
                }
            }
            else if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
                return true;
            }
            return false;
        }

        public static bool GetHandlesCommitKeys(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(ValueEditorUtils.HandlesCommitKeysProperty);
        }

        public static double RoundToDoublePrecision(double value, int digits)
        {
            double num = Math.Abs(value);
            if (num >= 1)
            {
                digits = digits - (int)Math.Ceiling(Math.Log10(num));
            }
            if (digits >= 0)
            {
                value = Math.Round(value, digits);
            }
            return value;
        }

        public static void SetHandlesCommitKeys(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(ValueEditorUtils.HandlesCommitKeysProperty, value);
        }

        public static void UpdateBinding(FrameworkElement element, DependencyProperty property, bool updateSource)
        {
            ValueEditorUtils.UpdateBinding(element, property, (updateSource ? UpdateBindingType.Both : UpdateBindingType.Target));
        }

        public static void UpdateBinding(FrameworkElement element, DependencyProperty property, UpdateBindingType updateType)
        {
            BindingExpression bindingExpression = element.GetBindingExpression(property);
            if (bindingExpression != null)
            {
                if (updateType == UpdateBindingType.Source || updateType == UpdateBindingType.Both)
                {
                    bindingExpression.UpdateSource();
                }
                if (updateType == UpdateBindingType.Target || updateType == UpdateBindingType.Both)
                {
                    bindingExpression.UpdateTarget();
                }
            }
        }
    }
}