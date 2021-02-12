using System.Linq;

namespace Nova.AI.BT.Composite
{
    public class Selector : Base.Composite
    {
        private int _current = 0;

        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            if (!Children.Any())
                return NodeStatus.Success;

            var child = Children[_current];
            var status = child.Execute(ctx);

            switch (status)
            {
                case NodeStatus.Success:
                    _current = 0;
                    return NodeStatus.Success;
                case NodeStatus.Running:
                    return NodeStatus.Running;
                case NodeStatus.Failure:

                    ++_current;
                    if (_current >= Children.Count)
                        return NodeStatus.Failure;

                    return NodeStatus.Running;
                default:
                    return NodeStatus.Failure;
            }
        }
    }
}
