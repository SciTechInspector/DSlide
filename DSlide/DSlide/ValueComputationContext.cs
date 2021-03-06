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

            foreach (var parent in node.Parents)
                parent.Children.Remove(node);

            node.Parents.Clear();
            node.Height = 1;

            this.ComputedValuesStack.Add(node);
        }

        public void ExitDataNodeComputation(DataNode node)
        {
            Debug.Assert(this.ComputedValuesStack[this.ComputedValuesStack.Count - 1] == node);
            this.ComputedValuesStack.RemoveAt(this.ComputedValuesStack.Count - 1);

            node.Height = node.Parents.Count == 0 ? 1 : node.Parents.Max(x => x.Height) + 1;
        }

        public void RegisterDependency(DataNode dataNode)
        {
            if (ComputedValuesStack.Count == 0)
                return;

            var curNode = ComputedValuesStack[this.ComputedValuesStack.Count - 1];
            curNode.Parents.Add(dataNode);
            dataNode.Children.Add(curNode);
        }
    }
}
