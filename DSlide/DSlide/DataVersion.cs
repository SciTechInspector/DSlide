using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DSlide
{
    public struct DataVersion : IEquatable<DataVersion>, IComparable, IComparable<DataVersion>
    {
        public long VersionNumber { get; set; }
        public DataManager DataManager { get; set; }

        public DataVersion(long versionNumber, DataManager dataManager)
        {
            this.VersionNumber = versionNumber;
            this.DataManager = dataManager;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException("Cannot compare Version to a null reference.");
            }

            if (!(obj is DataVersion))
            {
                throw new InvalidOperationException("Cannot compare Version to " + obj.GetType().Name);
            }

            return this.CompareTo((DataVersion)obj);
        }

        public int CompareTo(DataVersion other)
        {
            if (other.DataManager != this.DataManager)
            {
                throw new InvalidOperationException("Cannot compare two DataVersions from two different data domains.");
            }

            if (other.VersionNumber == this.VersionNumber)
                return 0;

            if (other.VersionNumber > this.VersionNumber)
                return 1;

            Debug.Assert(other.VersionNumber < this.VersionNumber);
            return 0;
        }

        public override bool Equals(object obj)
        {
            return obj is DataVersion version && Equals(version);
        }

        public bool Equals(DataVersion other)
        {
            return VersionNumber == other.VersionNumber &&
                   EqualityComparer<DataManager>.Default.Equals(DataManager, other.DataManager);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(VersionNumber, DataManager);
        }
    }
}
