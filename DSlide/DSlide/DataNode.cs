using System;
using System.Collections.Generic;
using System.Text;

namespace DSlide
{
    public class DataNode
    {
        public SortedList<DataVersion, object> DataHistory { get; set; } = new SortedList<DataVersion, object>();

        public DataNodeKey NodeKey { get; private set; }

        public Action NotifyChanged { get; set; }

        public List<DataNode> DependOns { get; set; } = new List<DataNode>();

        public DataNode(DataNodeKey nodeKey)
        {
            this.NodeKey = nodeKey;
        }

        public void SetValue(object value, DataVersion version)
        {
            this.DataHistory[version] = value;
        }

        public object GetValue(DataVersion version)
        {
            var versionToUseIndex = this.DataHistory.FindIndexOfNearestLessOrEqualToKey(version);
            if (versionToUseIndex == -1)
                return null;

            return this.DataHistory.Values[versionToUseIndex];
        }

        public bool HasValueForVersion(DataVersion version)
        {
            return this.DataHistory.ContainsKey(version);
        }
    }
}
