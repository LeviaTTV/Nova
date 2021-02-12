namespace Nova.AI.BT.Decorator
{
    public class Repeater : Base.Decorator
    {
        private int _count = 0;

        public int Amount { get; set; }

        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            if (Child == null)
                return NodeStatus.Success;

            if (_count + 1 >= Amount)
                return Child.Execute(ctx);
            
            var status = Child.Execute(ctx);
            if (status == NodeStatus.Failure)
                return NodeStatus.Failure;

            ++_count;

            return NodeStatus.Running;
        }
    }
}
