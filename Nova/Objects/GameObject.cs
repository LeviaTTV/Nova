using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Nova.Objects
{
    public abstract class GameObject
    {
        protected readonly GameServiceContainer Services;
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Vector2 Origin { get; set; }
        public Rectangle Bounds => new Rectangle((int) Position.X, (int) Position.Y, Width, Height);
        public Rectangle VisualBounds { get; protected set; }

        public abstract bool CollisionEnabled { get; }
        public Rectangle CollisionBounds { get; protected set; }

        public virtual void LoadContent(ContentManager contentManager)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public GameObject(GameServiceContainer services)
        {
            Services = services;
        }
        
        public bool Intersects(Rectangle otherRect)
        {
            var rect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

            return rect.Intersects(otherRect);
        }

        public bool Intersects(Vector2 position)
        {
            return position.X >= (int)Position.X && position.Y >= (int)Position.Y &&
                   position.X <= (int)Position.X + Width && position.Y <= (int)Position.Y + Height;
        }
    }
}
