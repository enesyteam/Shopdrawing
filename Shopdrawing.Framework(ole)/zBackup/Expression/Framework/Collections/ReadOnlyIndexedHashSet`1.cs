using Microsoft.Expression.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.Framework.Collections
{
    public sealed class ReadOnlyIndexedHashSet<T> : IIndexedHashSet<T>, ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>, ICollection, IEnumerable
    where T : class
    {
        private IIndexedHashSet<T> hashSet;

        public int Count
        {
            get
            {
                return ((ICollection)(this.hashSet)).Count; //xxxxxxxxxxxxxxxxxxxxxxx this.hashSet.Count
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public T this[int hash]
        {
            get
            {
                return this.hashSet[hash];
            }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get
            {
                return this.hashSet.IsSynchronized;
            }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get
            {
                return this.hashSet.SyncRoot;
            }
        }

        private ReadOnlyIndexedHashSet()
        {
        }

        public ReadOnlyIndexedHashSet(IIndexedHashSet<T> hashSet)
        {
            if (hashSet == null)
            {
                throw new ArgumentNullException("hashSet");
            }
            this.hashSet = hashSet;
        }

        public void Add(T item)
        {
            throw new InvalidOperationException("Read only collection.");
        }

        public void Clear()
        {
            throw new InvalidOperationException("Read only collection.");
        }

        public bool Contains(T item)
        {
            return this.hashSet.Contains(item);
        }

        public void CopyTo(Array array, int index)
        {
            this.hashSet.CopyTo(array, index);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.hashSet.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new InvalidOperationException("Read only collection.");
        }

        IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
        {
            return this.hashSet.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.hashSet.GetEnumerator();
        }
    }
}