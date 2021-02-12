using System.Linq;

namespace Nova.AI.BT.Composite
{
    public class Parallel : Base.Composite
    {
        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            if (!Children.Any())
                return NodeStatus.Success;

            bool allSuccessful = true;

            foreach (var node in Children)
            {
                var status = node.Execute(ctx);

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
