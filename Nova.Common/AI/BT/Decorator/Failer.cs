using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.Common.AI.BT.Decorator
{
    public class Failer : Base.Decorator
    {
        public override NodeStatus Execute()
        {
            Child?.Execute();
            return NodeStatus.Failure;
        }
    }
}
