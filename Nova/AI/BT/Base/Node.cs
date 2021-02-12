﻿using System.Collections.Generic;

namespace Nova.AI.BT.Base
{
    public abstract class Node
    {
        public string Name { get; set; }
        public Node Parent { get; set; }

        public List<Node> Children { get; private set; } = new List<Node>();
        public string AppliesTo { get; set; }

        public virtual NodeStatus Execute(AIExecutionContext ctx)
        {
            return NodeStatus.Success;
        }
    }
}
