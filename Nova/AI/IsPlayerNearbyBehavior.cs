using System.Linq;
using Microsoft.Xna.Framework;
using Nova.AI.BT;
using Nova.AI.BT.Base;
using Nova.Objects.Character;
using Nova.Services;

namespace Nova.AI
{
    public class IsPlayerNearbyBehavior : Node
    {
        public override NodeStatus Execute(AIExecutionContext ctx)
        {
            var obj = ctx.Services.GetService<GameObjectManager>();

            var player = obj.GameObjects.OfType<PlayerCharacter>().FirstOrDefault();

            if (Vector2.Distance(player.Position, ctx.LivingGameObject.Position) <= 256f)
            {
                return NodeStatus.Success;
            }
            return NodeStatus.Failure;
        }
    }
}
