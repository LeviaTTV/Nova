using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nova.Common.AI.BT.Base
{
    public abstract class Decorator : Node
    {
        public Node Child => Children.FirstOrDefault();
    }
}
