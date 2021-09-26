using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace DSlide
{

    public class DataSlideCollection<T> : DataSlideBase, INotifyCollectionChanged, IEnumerable<T>, IList<T>, IReadOnlyList<T>
    {
        private List<T> internalList = new List<T>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected IEnumerable<T> Enumerator 
        {
            get
            {
                return base.GetSourceValue<IEnumerable<T>>(NotifyCollectionAndPropertyChanged);
            }

            set
            {
                base.SetSourceValue<IEnumerable<T>>(value, NotifyCollectionAndPropertyChanged);
            }
        }


        public T this[int index]
        {
            get
            {
                var enumerator = this.Enumerator;
                if (enumerator != null)
                    return this.internalList[index];

                return default(T);
            }

            set
            {
                this.internalList[index] = value;
                this.Enumerator = this.makeNewEnumerator();
            }
        }

        public virtual int Count
        {
            get
            {
                var enumerator = this.Enumerator;
                if (enumerator != null)
                    return this.internalList.Count;

                return 0;
            }
        }

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            this.internalList.Add(item);
            this.Enumerator = this.makeNewEnumerator();
        }

        public void Clear()
        {
            this.Clear();
            this.Enumerator = this.makeNewEnumerator();
        }

        public bool Contains(T item)
        {
            var enumerator = this.Enumerator;
            if (enumerator != null)
                return this.internalList.Contains(item);

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var enumerator = this.Enumerator;
            if (enumerator != null)
                this.internalList.CopyTo(array, arrayIndex);
        }

        public IEnumerable<T> makeNewEnumerator()
        {
            foreach (var item in this.internalList)
                yield return item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Enumerator?.GetEnumerator() ?? Enumerable.Empty<T>().GetEnumerator();
        }

        public int IndexOf(T item)
        {
            var enumerator = this.Enumerator;
            if (enumerator != null)
                return this.internalList.IndexOf(item);

            return -1;
        }

        public void Insert(int index, T item)
        {
            this.internalList.Insert(index, item);
            this.Enumerator = this.makeNewEnumerator();
        }

        public bool Remove(T item)
        {
            var retVal = this.internalList.Remove(item);
            if (retVal)
                this.Enumerator = this.makeNewEnumerator();

            return retVal;
        }

        public void RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
            this.Enumerator = this.makeNewEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Enumerator.GetEnumerator();
        }

        private void NotifyCollectionAndPropertyChanged()
        {
            SendNotification(new System.ComponentModel.PropertyChangedEventArgs(nameof(Enumerator)));
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.internalList.ToList()));
        }

    }

}
