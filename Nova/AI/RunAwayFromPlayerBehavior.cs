using System.Linq;
using Microsoft.Xna.Framework;
using Nova.AI.BT;
using Nova.AI.BT.Base;
using Nova.Objects.Character;
using Nova.Services;

namespace Nova.AI
{
    public class RunAwayFromPlayerBehavior : Node
    {
        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            var obj = ctx.Services.GetService<GameObjectManager>();

            if (ctx.LivingGameObject.IsMoving)
                return NodeStatus.Running;



            var player = obj.GameObjects.OfType<PlayerCharacter>().FirstOrDefault();

            if (Vector2.Distance(player.Position, ctx.LivingGameObject.Position) >= 256f)
            {
                return NodeStatus.Success;
            }

            var direction = player.Position - ctx.LivingGameObject.Position;
            direction *= 2;
            ctx.LivingGameObject.MoveSpeedModifier = 0.3f;
            ctx.LivingGameObject.MoveTo(ctx.LivingGameObject.Position - direction, false);


            return NodeStatus.Running;
        }
    }
}
