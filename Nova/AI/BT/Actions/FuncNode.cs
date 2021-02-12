using System;
using Nova.AI.BT.Base;

namespace Nova.AI.BT.Actions
{
    public class FuncNode : Node
    {
        private readonly Func<NodeStatus> _action;

        public FuncNode(Func<NodeStatus> action)
        {
            _action = action;
        }

        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            return _action();
        }
    }
}
