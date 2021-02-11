using System;
using System.Collections.Generic;
using System.Text;
using Nova.Common.AI.BT.Base;

namespace Nova.Common.AI.BT.Decorator
{
    public class RepeatForever : Base.Decorator
    {
        public override NodeStatus Execute()
        {
            Child?.Execute();
            return NodeStatus.Running;
        }
    }
}
