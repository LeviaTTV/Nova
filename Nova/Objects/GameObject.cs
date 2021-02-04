using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Environment;
using Nova.Environment.Foliage;
using IDrawable = Nova.Common.Sprite.IDrawable;
using IUpdateable = Nova.Common.Sprite.IUpdateable;

namespace Nova.Objects
{
    public class GameObject
    {
        public virtual void LoadContent(ContentManager contentManager)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }

    public class FoliageGameObject : GameObject
    {
        protected readonly GraphicsDevice GraphicsDevice;
        public readonly FoliageType FoliageType;
        public readonly Tile Tile;

        public FoliageGameObject(GraphicsDevice graphicsDevice, FoliageType foliageType, Tile tile)
        {
            GraphicsDevice = graphicsDevice;
            FoliageType = foliageType;
            Tile = tile;
        }
    }
}
