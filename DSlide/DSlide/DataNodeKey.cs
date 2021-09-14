using System;
using System.Collections.Generic;
using System.Text;

namespace DSlide
{
    public struct DataNodeKey : IEquatable<DataNodeKey>
    {
        public object Container { get; private set; }
        public string PropertyName { get; private set; }

        public DataNodeKey(object container, string propertyName)
        {
            this.Container = container;
            this.PropertyName = propertyName;
        }

        public override bool Equals(object obj)
        {
            return obj is DataNodeKey key && Equals(key);
        }

        public bool Equals(DataNodeKey other)
        {
            return EqualityComparer<object>.Default.Equals(Container, other.Container) &&
                   PropertyName == other.PropertyName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Container, PropertyName);
        }
    }
}
