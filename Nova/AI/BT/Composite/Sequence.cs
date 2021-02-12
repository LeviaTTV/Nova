using System.Linq;

namespace Nova.AI.BT.Composite
{
    public class Sequence : Base.Composite
    {
        public int _step = 0;

        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            if (!Children.Any())
                return NodeStatus.Success;

            var node = Children[_step];
            var status = node.Execute(ctx);
            switch (status)
            {
                case NodeStatus.Success:
                    ++_step;
                    if (_step >= Children.Count)
                    {
                        _step = 0;
                        return NodeStatus.Success;
                    }

                    return NodeStatus.Running;
                case NodeStatus.Running:
                    return NodeStatus.Running;
                case NodeStatus.Failure:
                    return NodeStatus.Failure;
                default:
                    return NodeStatus.Failure;
            }
        }
    }
}
