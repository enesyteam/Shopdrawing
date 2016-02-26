using Microsoft.Windows.Design.Metadata;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
    internal static class WpfAttributeTableExtensions
    {
        internal static void AddCustomAttributes(this AttributeCallbackBuilder source, DependencyProperty property, params Attribute[] attributes)
        {
            string memberName = !(source.CallbackType.GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty) != (PropertyInfo)null) ? "Get" + property.Name : property.Name;
            source.AddCustomAttributes(memberName, attributes);
        }
    }
}