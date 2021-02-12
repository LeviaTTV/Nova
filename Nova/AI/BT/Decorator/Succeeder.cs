namespace Nova.AI.BT.Decorator
{
    public class Succeeder : Base.Decorator
    {
        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            if (Child == null)
                return NodeStatus.Success;

            Child.Execute(ctx);

            return NodeStatus.Success;
        }
    }
}
