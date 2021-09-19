using DSlide;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace DSlideTest
{
    public abstract class PivotDataTest : DataSlideBase
    {
        public abstract string Data1 { get; set; }

        public abstract string Data2 { get; set; }

        public abstract bool PivotTo2 { get; set; }

        public virtual string SimplePivot => (this.PivotTo2 ? this.Data2 : this.Data1);

        public abstract bool PivotToDeep { get; set; }

        public abstract DiamondTest DiamondTest1 { get; set; }
        public abstract DiamondTest DiamondTest2 { get; set; }

        public virtual string ComputeDeep1 => this.Data1 + 1 + (PivotToDeep ? "d" : "s");
        public virtual string ComputeDeep2 => this.ComputeDeep1 + 2 + (PivotToDeep ? "d" : "s");
        public virtual string ComputeDeep3 => this.ComputeDeep2 + 3 + (PivotToDeep ? "d" : "s");
        public virtual string ComputeDeep4 => this.ComputeDeep3 + 4 + (PivotToDeep ? "d" : "s");
        public virtual string ComputeDeep5 => this.ComputeDeep4 + 5 + (PivotToDeep ? "d" : "s");

        public virtual string ComputeShallow => this.Data1 + "now";

        public virtual string TrickyPivot => (this.PivotToDeep ? this.ComputeDeep5 : this.ComputeShallow);

        public virtual DataSlideBase MajorPivot
        {
            get
            {
                DataSlideBase retVal;
                if (PivotToDeep && PivotTo2)
                {
                    var key = new Key(this.Data1, this.Data2);
                    retVal = base.dataManager.CreateInstance<ReactTimeInstance1>(key, Persistence.Permanent);
                }
                else if (!PivotToDeep && PivotTo2)
                {
                    var key = new Key(this.Data2, this.Data1);
                    retVal = base.dataManager.CreateInstance<ReactTimeInstance1>(key, Persistence.Permanent);
                }
                else if (PivotToDeep && !PivotTo2)
                {
                    var key = new Key(DiamondTest1);
                    retVal = base.dataManager.CreateInstance<ReactTimeInstance2>(key, Persistence.Permanent);
                }
                else 
                {
                    Debug.Assert(!PivotToDeep && !PivotTo2);
                    var key = new Key(DiamondTest2);
                    retVal = base.dataManager.CreateInstance<ReactTimeInstance2>(key, Persistence.Permanent);
                }

                return retVal;
            }
        }
    }
}
