using System;
using System.Collections.Generic;
using System.Text;
using Nova.Common.AI.BT.Base;

namespace Nova.Common.AI.BT.Actions
{
    public class FuncNode : Node
    {
        private readonly Func<NodeStatus> _action;

        public FuncNode(Func<NodeStatus> action)
        {
            _action = action;
        }

        public override NodeStatus Execute()
        {
            return _action();
        }
    }
}
