namespace Nova.AI.BT.Decorator
{
    public class RepeatForever : Base.Decorator
    {
        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            Child?.Execute(ctx);
            return NodeStatus.Running;
        }
    }
}
