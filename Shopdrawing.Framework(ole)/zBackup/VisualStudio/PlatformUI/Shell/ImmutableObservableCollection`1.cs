using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
    internal class ImmutableObservableCollection<T> : ReadOnlyObservableCollection<T>, IObservableCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, INotifyPropertyChanged, INotifyCollectionChanged
    {
        T Microsoft.VisualStudio.PlatformUI.Shell.IObservableCollection<T>.this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                ((IList<T>)this)[index] = value;
            }
        }

        public ImmutableObservableCollection(ObservableCollection<T> observedCollection)
            : base(observedCollection)
        {
        }

        void Microsoft.VisualStudio.PlatformUI.Shell.IObservableCollection<T>.Clear()
        {
            ((ICollection<T>)this).Clear();
        }
    }
}