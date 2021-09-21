using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DSlide
{

    public class DataSlideCollection<T> : DataSlideBase, IEnumerable<T>, IList<T>, IReadOnlyList<T>
    {
        private List<T> internalList = new List<T>();

        protected IEnumerable<T> Enumerator 
        {
            get
            {
                return base.GetSourceValue<IEnumerable<T>>();
            }

            set
            {
                base.SetSourceValue<IEnumerable<T>>(value);
            }
        }


        public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

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
            return this.Enumerator.GetEnumerator();
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
    }

}
