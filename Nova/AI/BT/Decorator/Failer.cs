namespace Nova.AI.BT.Decorator
{
    public class Failer : Base.Decorator
    {
        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            Child?.Execute(ctx);
            return NodeStatus.Failure;
        }
    }
}
