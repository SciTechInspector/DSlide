﻿using System;
using System.ComponentModel;

namespace DSlide
{
    public class Class1 : BaseClass
    {

        public string FirstName
        {
            get
            {
                return base.GetSourceValue<string>();
            }

            set
            {
                base.SetSourceValue<string>(value);
            }
        }

        public string LastName
        {
            get
            {
                return base.GetSourceValue<string>();
            }

            set
            {
                base.SetSourceValue<string>(value);
            }
        }

        public Boolean IsMale { get; set; }

        public string FullName { get { return base.GetComputedValue<string>(() => this.FirstName + this.LastName); } }

        public string PoliteAddress { get { return base.GetComputedValue<string>(() => (this.IsMale ? "Lord" : "Lady") + this.LastName); } }

        public string MangledName { get { return base.GetComputedValue<string>(() => FullName + PoliteAddress); } }

        //
        //          FirstName           LastName            IsMale
        //                 \             /    \              /
        //                  \           /      \            /
        //                  FullName            PoliteAddress
        //                          \           /
        //                          MangledName
        //
        //
        //
        //
    }
}