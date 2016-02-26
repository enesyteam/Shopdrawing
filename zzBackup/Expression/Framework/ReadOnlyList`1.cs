using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.Framework
{
    public class ReadOnlyList<T> : ReadOnlyCollection<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, ICollection, IEnumerable
    {
        public ReadOnlyList(IList<T> list)
            : base(list)
        {
        }

        //T Microsoft.Expression.Framework.IReadOnlyList<T>.get_Item(int num)
        //{
        //    return base[num];
        //}
    }
}