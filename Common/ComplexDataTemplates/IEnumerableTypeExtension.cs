using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;

namespace DaisleyHarrison.WPF.ComplexDataTemplates
{
    [MarkupExtensionReturnType(typeof(Type))]
    public class IEnumerableTypeExtension : TypeExtension
    {
        private static Type s_genericEnumerable = typeof(IEnumerable<object>).GetGenericTypeDefinition();
        public IEnumerableTypeExtension()
        { }
        public IEnumerableTypeExtension(string typeName)
            :base(typeName)
        {
        }
        public IEnumerableTypeExtension(Type type)
            : base(type)
        {
        }
        private Type parseType(IServiceProvider serviceProvider)
        {
            if (this.Type == null)
            {
                IXamlTypeResolver xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
                if (xamlTypeResolver != null)
                {
                    return xamlTypeResolver.Resolve(this.TypeName);
                }
            }
            else
            {
                return this.Type;
            }
            return typeof(IEnumerable<object>);
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return s_genericEnumerable.MakeGenericType(parseType(serviceProvider));
        }
    }
}
