namespace Nova.AI.BT.Decorator
{
    public class Inverter : Base.Decorator
    {
        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            if (Child == null)
                return NodeStatus.Failure;

            var status = Child.Execute(ctx);

            if (status == NodeStatus.Running)
                return NodeStatus.Running;

            return status == NodeStatus.Failure ? NodeStatus.Success : NodeStatus.Failure;
        }
    }
}
