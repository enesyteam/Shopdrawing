using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DaisleyHarrison.WPF.ComplexDataTemplates
{
    public class BindingGroup : IEnumerable, IBindingGroup
    {
        public string Parameter { get; private set; }
        public IEnumerable Items { get; private set; }

        public BindingGroup(IEnumerable items, string parameter)
        {
            this.Items = items;
            this.Parameter = parameter;
        }
        public Type ElementType
        {
            get 
            {
                return GetElementType(this.Items);
            }
        }
        public static Type GetElementType(IEnumerable enumerable)
        {
            Type enumerableType = enumerable.GetType();
            Type elementType = null;

            if (enumerableType.IsGenericType)
            {
                Type[] genericArguments = enumerableType.GetGenericArguments();
                if (genericArguments.Length > 0)
                {
                    elementType = genericArguments[0];
                }
            }

            if (elementType == null)
            {
                IEnumerator enumItems = enumerable.GetEnumerator();
                if (enumItems.MoveNext())
                {
                    if (enumItems.Current != null)
                    {
                        elementType = enumItems.Current.GetType();
                    }
                }
            }
            return elementType;
        }


        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }
        #endregion

        public override string ToString()
        {
            return string.Format("{{BindingGroup of {0}}}", ElementType.FullName); 
        }
    }
}
