using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nova.Common.AI.BT.Composite
{
    public class Parallel : Base.Composite
    {
        public override NodeStatus Execute()
        {
            if (!Children.Any())
                return NodeStatus.Success;

            bool allSuccessful = true;

            foreach (var node in Children)
            {
                var status = node.Execute();

                if (status == NodeStatus.Failure)
                    return NodeStatus.Failure;

                if (status == NodeStatus.Running)
                    allSuccessful = false;
            }

            if (allSuccessful)
                return NodeStatus.Success;

            return NodeStatus.Running;
        }
    }
}
