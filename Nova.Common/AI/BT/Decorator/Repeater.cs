using System;
using System.Collections.Generic;
using System.Text;
using Nova.Common.AI.BT.Base;

namespace Nova.Common.AI.BT.Decorator
{
    public class Repeater : Base.Decorator
    {
        private int _count = 0;

        public int Amount { get; set; }

        public override NodeStatus Execute()
        {
            if (Child == null)
                return NodeStatus.Success;

            if (_count + 1 >= Amount)
                return Child.Execute();
            
            var status = Child.Execute();
            if (status == NodeStatus.Failure)
                return NodeStatus.Failure;

            ++_count;

            return NodeStatus.Running;
        }
    }
}
