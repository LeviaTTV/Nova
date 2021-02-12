using Nova.AI.BT.Base;

namespace Nova.AI.BT
{
    public class BehaviorTreeBuilder
    {
        public static NodeBuilder WithRootComposite(Base.Composite composite)
        {
            return new NodeBuilder(composite);
        }
    }

    public class NodeBuilder
    {
        private readonly Node _node;
        private readonly Node _parent;
        private readonly NodeBuilder _parentBuilder;

        public NodeBuilder(Node node, Node parent = null, NodeBuilder parentBuilder = null)
        {
            _node = node;
            _parent = parent;
            _parentBuilder = parentBuilder;

            if (_parent != null)
                _parent.Children.Add(_node);
            _node.Parent = _parent;
        }

        public NodeBuilder Add(Node child)
        {
            return new NodeBuilder(child, _node, this);
        }

        public NodeBuilder Close()
        {
            return _parentBuilder;
        }

        public Node Build()
        {
            if (_parent == null)
                return _node;

            return null;
        }
    }


}
