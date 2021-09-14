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

        internal object GetValue(DataVersion currentVersion)
        {
            var versionToUse = this.DataHistory.FindNearestLessOrEqualKey(currentVersion);
            if (versionToUse == null)
                return null;

            return this.DataHistory[versionToUse.Value];
        }
    }
}
