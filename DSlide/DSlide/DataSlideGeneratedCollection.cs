using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace DSlide
{
    public class DataSlideGeneratedCollection<T> : DataSlideBase, INotifyCollectionChanged, IEnumerable<T>, IReadOnlyList<T>
    {
        private Func<List<T>> listGenerator = null;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public DataSlideGeneratedCollection(Func<List<T>> listGenerator)
        {
            this.listGenerator = listGenerator;
        }

        public T this[int index] => this.InternalList[index];

        public int Count => this.InternalList.Count;

        private void NotifyCollectionAndPropertyChanged()
        {
            SendNotification(new System.ComponentModel.PropertyChangedEventArgs(nameof(InternalList)));
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.InternalList.ToList()));
        }


        protected List<T> InternalList
        {
            get
            {
                return base.GetComputedValue<List<T>>(listGenerator, NotifyCollectionAndPropertyChanged);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.InternalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InternalList.GetEnumerator();
        }
    }

}
