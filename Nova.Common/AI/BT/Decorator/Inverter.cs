using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.Common.AI.BT.Decorator
{
    public class Inverter : Base.Decorator
    {
        public override NodeStatus Execute()
        {
            if (Child == null)
                return NodeStatus.Failure;

            var status = Child.Execute();

            if (status == NodeStatus.Running)
                return NodeStatus.Running;

            return status == NodeStatus.Failure ? NodeStatus.Success : NodeStatus.Failure;
        }
    }
}
