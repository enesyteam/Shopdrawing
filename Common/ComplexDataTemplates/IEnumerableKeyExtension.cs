using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;

namespace DaisleyHarrison.WPF.ComplexDataTemplates
{
    [MarkupExtensionReturnType(typeof(string))]
    public class IEnumerableKeyExtension : MarkupExtension
    {
        private static Type s_genericEnumerable = typeof(IEnumerable<object>).GetGenericTypeDefinition();
        public Type Type{ get; set;}
        public string TypeName { get; set; }
        public IEnumerableKeyExtension()
        { }
        public IEnumerableKeyExtension(string typeName)
        {
            this.TypeName = typeName;
        }
        public IEnumerableKeyExtension(Type type)
        {
            this.Type = type;
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
            return new DataTemplateKey(s_genericEnumerable.MakeGenericType(parseType(serviceProvider)));
        }
    }
}
