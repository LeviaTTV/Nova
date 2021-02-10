using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Primitives;
using Nova.Common.Sprite;
using Nova.Objects.Character;
using Penumbra;

namespace Nova.Objects.Environment
{
    public class Tent : GameObject
    {
        private Sprite _sprite;
        private PenumbraComponent _penumbra;
        private PlayerCharacter _playerCharacter;
        private Hull _hull;

        private PrimitiveRectangle _rectangle1;
        private PrimitiveRectangle _rectangle2;

        public override bool CollisionEnabled => true;

        public Tent(GameServiceContainer services) : base(services)
        {
            _penumbra = services.GetService<PenumbraComponent>();
            _playerCharacter = services.GetService<PlayerCharacter>();
        }

        public override void LoadContent(ContentManager contentManager)
        {
            _sprite = contentManager.Load<SpriteSheet>("Environment/GreenTent")["14"];

            Width = _sprite.Width;
            Height = _sprite.Height;

            VisualBounds = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

            var height = VisualBounds.Height / 2;
            CollisionBounds = new Rectangle(VisualBounds.X + 14, VisualBounds.Y + height - 8, VisualBounds.Width - 30, height - 8);

            _hull = new Hull(
                new Vector2(10, 59),
                new Vector2(48, 21),
                new Vector2(85, 58),
                new Vector2(85, 115),
                new Vector2(10, 115)
            );
            _hull.Position = Position;

            _penumbra.Hulls.Add(_hull);

            var device = Services.GetService<GraphicsDevice>();
            _rectangle1 = new PrimitiveRectangle(device, Color.Yellow, false);
            _rectangle2 = new PrimitiveRectangle(device, Color.DarkGreen, false);
        }
        
        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float z = 0.7f;
            if (Position.Y + Height > _playerCharacter.Position.Y + 64f)
            {
                z = 0.4f;
            }

            z -= Position.Y * 0.00001f + Position.X * 0.00001f;


            _sprite.Draw(spriteBatch, Position, layerDepth: z);

            if (DebugTools.GenericDebugEnabled)
            {
                _rectangle1.Draw(spriteBatch, VisualBounds);
                _rectangle2.Draw(spriteBatch, CollisionBounds);
            }
        }
    }
}
