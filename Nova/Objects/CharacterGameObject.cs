using Microsoft.Xna.Framework;

namespace Nova.Objects
{
    public class CharacterGameObject : GameObject
    {
        public CharacterGameObject(GameServiceContainer services) : base(services)
        {
        }

        public override bool CollisionEnabled => true;
    }
}