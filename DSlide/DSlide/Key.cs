using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSlide
{
    public class Key : IEquatable<Key>
    {
        public object[] keys { get; set; }

        private Type objectType;

        public Type ObjectType
        { 
            get
            {
                return this.objectType;
            }

            set
            {
                this.objectType = value;
                this.hashCode = generateHashCode();
            }
        }

        public int hashCode { get; set; }

        public Key(params object[] keys)
        {
            this.keys = keys;
            this.hashCode = generateHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Key);
        }

        public bool Equals(Key other)
        {
            if (!(other != null &&
                   EqualityComparer<Type>.Default.Equals(objectType, other.objectType)))
                return false;

            return this.keys.SequenceEqual(other.keys);
        }
        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public int generateHashCode()
        {
            var hashCode = objectType?.GetHashCode() ?? 31;
            foreach (var key in this.keys)
                hashCode = HashCode.Combine(hashCode, key);

            return hashCode;
        }
    }
}
