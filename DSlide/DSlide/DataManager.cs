using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSlide
{
    public class DataManager
    {
        private List<Action> changeNotificationsToSend;
        private Dictionary<DataNodeKey, DataNode> allDataNodes = new Dictionary<DataNodeKey, DataNode>();
        private HashSet<DataNode> modifiedSourceDataNodes = new HashSet<DataNode>();
        private ValueComputationContext computationContext = new ValueComputationContext();

        private DataContainerFactory containerFactory;

        private Dictionary<Key, DataSlideKeyBase> cachedGeneratedObjects = new Dictionary<Key, DataSlideKeyBase>();

        [ThreadStatic]
        private static DataManager ts_DataManager;

        public static DataManager Current
        {
            get
            {
                if (DataManager.ts_DataManager == null)
                {
                    DataManager.ts_DataManager = new DataManager();
                }

                return DataManager.ts_DataManager;
            }
        }

        public DataVersion ReadVersion { get; private set; }
        public DataVersion EditVersion { get; private set; }

        public DataManager()
        {
            this.ReadVersion = new DataVersion(1, this);
            this.EditVersion = null;
            this.containerFactory = new DataContainerFactory();
            this.containerFactory.CreateDerivedClasses();
        }

        public void SetSourceValue<T>(T newValue, object container, string propertyName, Action notifier)
        {
            if (this.EditVersion == null)
                throw new InvalidOperationException("Cannot modify a source value ouside of edit mode.");

            DataNode dataNode;
            var dataNodeKey = new DataNodeKey(container, propertyName);
            if (!this.allDataNodes.TryGetValue(dataNodeKey, out dataNode))
            {
                dataNode = new DataNode(dataNodeKey, notifier);
                this.allDataNodes[dataNodeKey] = dataNode;
            }

            this.modifiedSourceDataNodes.Add(dataNode);

            dataNode.SetValue(newValue, this.EditVersion);
        }

        public object GetSourceValue(object container, string propertyName, Action notifier)
        {
            DataNode dataNode;
            var dataNodeKey = new DataNodeKey(container, propertyName);
            if (!this.allDataNodes.TryGetValue(dataNodeKey, out dataNode))
            {
                dataNode = new DataNode(dataNodeKey, notifier);
                this.allDataNodes[dataNodeKey] = dataNode;
            }

            this.computationContext.RegisterDependency(dataNode);

            var retrievedValue = dataNode.GetValue(ReadVersion);
            if (retrievedValue == null)
                return default(object);

            return retrievedValue;
        }

        public object GetComputedValue(Func<object> computer, object container, string propertyName, Action notifier)
        {
            DataNode dataNode;
            var dataNodeKey = new DataNodeKey(container, propertyName);
            if (!this.allDataNodes.TryGetValue(dataNodeKey, out dataNode))
            {
                dataNode = new DataNode(dataNodeKey, computer, notifier);
                this.allDataNodes[dataNodeKey] = dataNode;
            }

            this.computationContext.RegisterDependency(dataNode);

            if (dataNode.HasValue())
            {
                var retrievedValue = dataNode.GetValue(ReadVersion);
                if (retrievedValue == null)
                    return default(object);

                return retrievedValue;
            }

            this.computationContext.EnterDataNodeComputation(dataNode);
            var newValue = computer();
            this.computationContext.ExitDataNodeComputation(dataNode);

            dataNode.SetValue(newValue, ReadVersion);
            return newValue;
        }

        public void EnterEditMode()
        {
            this.EditVersion = new DataVersion(this.ReadVersion.VersionNumber + 1, this);
        }

        public void ExitEditMode()
        {
            this.ReadVersion = this.EditVersion;
            this.EditVersion = null;

            SortedList<long, HashSet<DataNode>> dataNodesToProcess = new SortedList<long, HashSet<DataNode>>();

            dataNodesToProcess[0] = this.modifiedSourceDataNodes;
            this.modifiedSourceDataNodes = new HashSet<DataNode>();

            this.changeNotificationsToSend = new List<Action>();

            while (dataNodesToProcess.Count != 0)
            {
                var firstHeightNodes = dataNodesToProcess.Values[0];
                var processNode = firstHeightNodes.First();

                if (firstHeightNodes.Count == 1)
                    dataNodesToProcess.RemoveAt(0);
                else
                    firstHeightNodes.Remove(processNode);

                if (processNode.IsComputedData())
                {
                    this.computationContext.EnterDataNodeComputation(processNode);
                    var newValue = processNode.Computer();
                    this.computationContext.ExitDataNodeComputation(processNode);
                    processNode.SetValue(newValue, this.ReadVersion);
                }

                if (!processNode.HasValueForVersion(this.ReadVersion))
                    continue;

                // TODO: Prepare/send change notification
                changeNotificationsToSend.Add(processNode.NotifyChanged);

                foreach (var dependOnNode in processNode.Children)
                {
                    HashSet<DataNode> nodesToProcessOfDependOnHeight;
                    if (!dataNodesToProcess.TryGetValue(dependOnNode.Height, out nodesToProcessOfDependOnHeight))
                    {
                        nodesToProcessOfDependOnHeight = new HashSet<DataNode>();
                        dataNodesToProcess[dependOnNode.Height] = nodesToProcessOfDependOnHeight;
                    }

                    nodesToProcessOfDependOnHeight.Add(dependOnNode);
                }
            }
        }

        public void SendChangeNotifications()
        {
            var notifications = this.changeNotificationsToSend;
            this.changeNotificationsToSend = null;

            foreach (var notificationToSend in notifications)
                notificationToSend();
        }

        public T CreateInstance<T>() where T : DataSlideBase
        {
            return this.containerFactory.CreateDataContainerInstance<T>();
        }


        public T CreateInstance<T>(Key key, Persistence persistence) where T : DataSlideKeyBase
        {
            key.ObjectType = typeof(T);

            DataSlideKeyBase retVal;
            if (!this.cachedGeneratedObjects.TryGetValue(key, out retVal))
            {
                retVal = this.containerFactory.CreateDataContainerInstance<T>();
                retVal.Initialize(key);
                this.cachedGeneratedObjects[key] = retVal;
            }

            return (T)retVal;
        }
    }
}
