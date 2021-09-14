using System;
using System.Collections.Generic;
using System.Text;

namespace DSlide
{
    public class DataManager
    {
        DataVersion currentVersion;

        Dictionary<DataNodeKey, DataNode> allDataNodes = new Dictionary<DataNodeKey, DataNode>();
        List<DataNode> sourceDataNodes = new List<DataNode>();

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

            dataNode.SetValue(newValue, currentVersion);
        }

        public T GetSourceValue<T>(object container, string propertyName, Action notifier) 
        {
            DataNode dataNode;
            var dataNodeKey = new DataNodeKey(container, propertyName);
            if (!this.allDataNodes.TryGetValue(dataNodeKey, out dataNode))
            {
                return default(T);
            }

            var retrievedValue = dataNode.GetValue(currentVersion);
            if (retrievedValue == null)
                return default(T);

            return (T)retrievedValue;
        }

        public T GetComputedValue<T>(Func<T> computer, object container, string propertyName, Action notifier) 
        {
            return default(T);
        }
    }
}
