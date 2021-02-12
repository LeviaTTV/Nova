using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Nova.AI.BT;
using Nova.AI.BT.Base;

namespace Nova.AI
{
    public class WanderBehavior : Node
    {
        private Random _random;
        
        public WanderBehavior()
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
        }

        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            if (ctx.LivingGameObject.IsMoving)
                return NodeStatus.Success;

            if (ctx.LivingGameObject.ActiveActions.Any())
                return NodeStatus.Running;


            var chances = _random.Next(0, 5);
            if (chances > 1 && chances <= 3)
            {
                ctx.LivingGameObject.PerformAction("Eat");
                return NodeStatus.Success;
            }
            else if (chances > 3)
            {
                return NodeStatus.Success;
            }
            else if (chances == 0 || chances == 1)
            {
                ctx.LivingGameObject.MoveSpeedModifier = 0.03f;

                var x = (float)_random.Next(-48, 48);
                var y = (float)_random.Next(-48, 48);
                ctx.LivingGameObject.MoveTo(ctx.LivingGameObject.Position + new Vector2(x, y), false);
            }

            
            
            return NodeStatus.Success;
        }
    }
}
