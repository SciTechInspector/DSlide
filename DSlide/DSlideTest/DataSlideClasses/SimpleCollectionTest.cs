using DSlide;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DSlideTest
{

    public abstract class SimpleCollectionTest : DataSlideBase
    {
        public abstract DataSlideCollection<string> Things { get; set; }

        public virtual int SumLengths
        {
            get
            {
                return Things.Sum(x => x.Length);
            }
        }

        public abstract string Filter { get; set; }

        public virtual DataSlideGeneratedCollection<string> FilteredCollection
        {
            get
            {
                return new DataSlideGeneratedCollection<string>(() => this.Things.Where(x => x.Contains(this.Filter ?? "")).ToList());
            }
        }

        public abstract bool UseFilter { get; set; }


        public virtual IReadOnlyList<string> PivotedCollection
        {
            get
            {
                return this.UseFilter ? this.FilteredCollection : this.Things;
            }
        }
    }
}