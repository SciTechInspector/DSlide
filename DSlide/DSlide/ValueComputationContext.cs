using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DSlide
{
    public class ValueComputationContext
    {
        private List<DataNode> ComputedValuesStack;

        public ValueComputationContext()
        {
            this.ComputedValuesStack = new List<DataNode>();
        }

        public void EnterDataNodeComputation(DataNode node)
        {
            Debug.Assert(!this.ComputedValuesStack.Contains(node));

            foreach (var dependency in node.DataNodesThatDependOnThisNode)
                dependency.DataNodesThatThisNodeDependsOn.Remove(node);

            node.DataNodesThatDependOnThisNode.Clear();
            node.Height = 1;

            this.ComputedValuesStack.Add(node);
        }

        public void ExitDataNodeComputation(DataNode node)
        {
            Debug.Assert(this.ComputedValuesStack[this.ComputedValuesStack.Count - 1] == node);
            this.ComputedValuesStack.RemoveAt(this.ComputedValuesStack.Count - 1);
        }

        public void RegisterDependency(DataNode dataNode)
        {
            if (ComputedValuesStack.Count == 0)
                return;

            var curNode = ComputedValuesStack[this.ComputedValuesStack.Count - 1];
            curNode.DataNodesThatThisNodeDependsOn.Add(dataNode);
            dataNode.DataNodesThatDependOnThisNode.Add(curNode);

            if (dataNode.Height >= curNode.Height)
            {
                curNode.Height = dataNode.Height + 1;
            }
        }
    }
}
