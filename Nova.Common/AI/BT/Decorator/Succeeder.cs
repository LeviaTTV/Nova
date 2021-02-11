using System;
using System.Collections.Generic;
using System.Text;

namespace Nova.Common.AI.BT.Decorator
{
    public class Succeeder : Base.Decorator
    {
        public override NodeStatus Execute()
        {
            if (Child == null)
                return NodeStatus.Success;

            Child.Execute();

            return NodeStatus.Success;
        }
    }
}
