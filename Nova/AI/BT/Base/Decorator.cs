using System.Linq;

namespace Nova.AI.BT.Base
{
    public abstract class Decorator : Node
    {
        public Node Child => Children.FirstOrDefault();
    }
}
