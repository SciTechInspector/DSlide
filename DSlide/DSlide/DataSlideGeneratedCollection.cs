using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DSlide
{

    public class DataSlideGeneratedCollection<T> : DataSlideBase, IEnumerable<T>, IReadOnlyList<T>
    {
        private Func<List<T>> listGenerator = null;

        public DataSlideGeneratedCollection(Func<List<T>> listGenerator)
        {
            this.listGenerator = listGenerator;
        }

        public T this[int index] => this.InternalList[index];

        public int Count => this.InternalList.Count;

        protected List<T> InternalList
        {
            get
            {
                return base.GetComputedValue<List<T>>(listGenerator);
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
