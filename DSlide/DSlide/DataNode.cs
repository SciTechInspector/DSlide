using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DSlide
{
    public class DataNode
    {
        public SortedList<DataVersion, object> DataHistory { get; set; } = new SortedList<DataVersion, object>();

        public DataNodeKey NodeKey { get; private set; }

        public Action NotifyChanged { get; set; }

        public long Height { get; set; }

        public List<DataNode> Children { get; set; } = new List<DataNode>();
        public List<DataNode> Parents { get; set; } = new List<DataNode>();

        public DataVersion UpdatedUpToVersion;

        public Func<object> Computer;

        public DataNode(DataNodeKey nodeKey, Action notifyChanged)
        {
            this.NodeKey = nodeKey;
            this.Height = 0;
            this.NotifyChanged = notifyChanged;
        }

        public DataNode(DataNodeKey nodeKey, Func<object> computer, Action notifyChanged)
        {
            this.NodeKey = nodeKey;
            this.Height = 0;
            this.Computer = computer;
            this.NotifyChanged = notifyChanged;
        }

        public void SetValue(object newValue, DataVersion version)
        {
            Debug.Assert(this.DataHistory.Count == 0 || this.DataHistory.Keys[this.DataHistory.Count - 1] <= version);

            if (this.DataHistory.Count != 0)
            {
                if (this.DataHistory.Count > 1 && this.DataHistory.Keys[this.DataHistory.Count - 1] == version)
                {
                    var previousValue = this.DataHistory.Values[this.DataHistory.Count - 2];
                    if (ExtensionUtilities.AreSame(previousValue, newValue))
                        this.DataHistory.RemoveAt(this.DataHistory.Count - 1);
                    else
                        this.DataHistory[version] = newValue;
                }
                else if (this.DataHistory.Keys[this.DataHistory.Count - 1] == version)
                {
                    this.DataHistory[version] = newValue;
                }
                else
                {
                    var previousValue = this.DataHistory.Values[this.DataHistory.Count - 1];
                    if (!ExtensionUtilities.AreSame(previousValue, newValue))
                        this.DataHistory[version] = newValue;
                }
            }
            else
            {
                this.DataHistory[version] = newValue;
            }
        }

        public object GetValue(DataVersion version)
        {
            var versionToUseIndex = this.DataHistory.FindIndexOfNearestLessOrEqualToKey(version);
            if (versionToUseIndex == -1)
                return null;

            return this.DataHistory.Values[versionToUseIndex];
        }

        /// <summary>
        /// Checks if this value has a value specifically for the given version.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool HasValueForVersion(DataVersion version)
        {
            return this.DataHistory.ContainsKey(version);
        }

        public bool HasValue()
        {
            return this.DataHistory.Count != 0;
        }

        public bool IsUpToDate(DataVersion version)
        {
            if (this.UpdatedUpToVersion == null)
                return false;

            return this.UpdatedUpToVersion.VersionNumber <= version.VersionNumber;
        }

        public bool IsComputedData()
        {
            return this.Computer != null;
        }

        public override string ToString()
        {
            if (this.DataHistory.Count > 0)
                return this.NodeKey.ToString() + ": " + this.DataHistory.Values.Last().ToString();

            return this.NodeKey.ToString() + ": <no value>";
        }
    }
}
