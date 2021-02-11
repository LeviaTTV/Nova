using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nova.Common.AI.BT;
using Nova.Common.AI.BT.Base;

namespace Nova.AI
{
    public class DetectLureBehavior : Node
    {
        public override NodeStatus Execute()
        {
            return NodeStatus.Failure;
        }
    }
}
