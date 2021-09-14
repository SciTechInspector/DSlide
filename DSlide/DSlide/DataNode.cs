using System;
using System.Collections.Generic;
using System.Text;

namespace DSlide
{
    public class DataNode
    {
        public SortedList<DataVersion, object> DataHistory { get; set; }

        public DataNodeKey NodeKey { get; private set; }

        public Action NotifyChanged { get; set; }

        public List<DataNode> DependOns { get; set; }

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
                return default(object);

            return this.DataHistory[versionToUse.Value];
        }
    }
}
