using Microsoft.Xna.Framework;
using Nova.Environment;

namespace Nova.Objects
{
    public abstract class FoliageGameObject : GameObject
    {
        public readonly Tile Tile;

        public override bool CollisionEnabled => false;

        public FoliageGameObject(GameServiceContainer services, Tile tile)
            : base(services)
        {
            Tile = tile;

            Position = new Vector2(Tile.X * 32, Tile.Y * 32);
        }
    }
}