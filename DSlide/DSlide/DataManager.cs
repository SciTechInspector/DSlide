using System;
using System.Collections.Generic;
using System.Text;

namespace DSlide
{
    public class DataManager
    {
        public DataVersion CurrentVersion { get; private set; }

        private Dictionary<DataNodeKey, DataNode> allDataNodes = new Dictionary<DataNodeKey, DataNode>();
        private List<DataNode> sourceDataNodes = new List<DataNode>();

        public DataManager()
        {
            this.CurrentVersion = new DataVersion(1, this);
        }

        public void SetSourceValue<T>(T newValue, object container, string propertyName, Action notifier)
        {
            DataNode dataNode;
            var dataNodeKey = new DataNodeKey(container, propertyName);
            if (!this.allDataNodes.TryGetValue(dataNodeKey, out dataNode))
            {
                dataNode = new DataNode(dataNodeKey);
                this.allDataNodes[dataNodeKey] = dataNode;
                this.sourceDataNodes.Add(dataNode);
            }

            dataNode.SetValue(newValue, CurrentVersion);
        }

        public T GetSourceValue<T>(object container, string propertyName, Action notifier) 
        {
            DataNode dataNode;
            var dataNodeKey = new DataNodeKey(container, propertyName);
            if (!this.allDataNodes.TryGetValue(dataNodeKey, out dataNode))
            {
                return default(T);
            }

            var retrievedValue = dataNode.GetValue(CurrentVersion);
            if (retrievedValue == null)
                return default(T);

            return (T)retrievedValue;
        }

        public T GetComputedValue<T>(Func<T> computer, object container, string propertyName, Action notifier) 
        {
            DataNode dataNode;
            var dataNodeKey = new DataNodeKey(container, propertyName);
            if (!this.allDataNodes.TryGetValue(dataNodeKey, out dataNode))
            {
                dataNode = new DataNode(dataNodeKey);
                this.allDataNodes[dataNodeKey] = dataNode;
            }

            if (dataNode.HasValueForVersion(this.CurrentVersion))
            {
                var retrievedValue = dataNode.GetValue(CurrentVersion);
                if (retrievedValue == null)
                    return default(T);

                return (T)retrievedValue;
            }

            var newValue = computer();
            dataNode.SetValue(newValue, CurrentVersion);
            return newValue;
        }
    }
}
