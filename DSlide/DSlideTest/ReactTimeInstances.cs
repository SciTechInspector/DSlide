using DSlide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSlideTest
{
    public abstract class ReactTimeInstance1 : DataSlideKeyBase
    {
        private string originalData1;
        private string originalData2;

        public abstract string NewDataSource { get; set; }

        public virtual string ResultingDataSource
        {
            get
            {
                return NewDataSource ?? originalData1;
            }
        }

        public string OriginalData2 => this.originalData2;

        public override void Initialize(Key key)
        {
            var data1 = key.keys[0] as string;
            var data2 = key.keys[1] as string;
            this.originalData1 = data1;
            this.originalData2 = data2;
        }
    }

    public abstract class ReactTimeInstance2 : DataSlideKeyBase
    {
        private DiamondTest referenceObject;

        public abstract string NewDataSource1 { get; set; }
        public abstract string NewDataSource2 { get; set; }


        public virtual string PivotedNewValue => this.referenceObject.IsMale ? NewDataSource1 : NewDataSource2;

        public DiamondTest DirectReference => referenceObject;

        // public string IllegalDirectReference => referenceObject.FirstName;

        public virtual string LegalDirectReference => referenceObject.FirstName;

        public override void Initialize(Key key)
        {
            this.referenceObject = (DiamondTest)key.keys[0];
        }
    }
}
