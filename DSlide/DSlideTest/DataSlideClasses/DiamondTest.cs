using DSlide;
using System;
using System.ComponentModel;

namespace DSlideTest
{
    public abstract class DiamondTest : DataSlideBase
    {
        public abstract string FirstName { get; set; }

        public abstract string LastName { get; set; }

        public abstract Boolean IsMale { get; set; }

        public virtual string FullName => $"{this.FirstName} {this.LastName}";

        public virtual string PoliteAddress => (this.IsMale ? "Lord" : "Lady") + this.LastName;

        public virtual string MangledName => FullName + PoliteAddress;
    }
}
