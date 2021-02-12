using Microsoft.Xna.Framework;
using Nova.Objects.Character;

namespace Nova.AI.BT
{
    public class AIExecutionContext
    {
        public LivingGameObject LivingGameObject { get; set; }
        public GameServiceContainer Services { get; set; }
    }
}
