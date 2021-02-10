using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Primitives;
using Nova.Common.Sprite;
using Nova.Objects.Character;
using IUpdateable = Nova.Common.IUpdateable;

namespace Nova.Objects
{
    public class GenericGameObject<T> : GameObject where T : SpriteSheet
    {
        private readonly string _assetName;
        private readonly string _spriteName;

        private T _sheet;
        private Sprite _sprite;

        private readonly PlayerCharacter _playerCharacter;

        public override bool CollisionEnabled => true;

        private PrimitiveRectangle _rectangle;

        public GenericGameObject(GameServiceContainer services, string assetName, string spriteName = null) : base(services)
        {
            _playerCharacter = services.GetService<PlayerCharacter>();
            _assetName = assetName;
            _spriteName = spriteName;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            _sheet = contentManager.Load<T>(_assetName);
            
            if (_spriteName != null)
                _sprite = _sheet[_spriteName];

            _rectangle = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Yellow, false);

            if (_sprite != null)
            {
                Width = _sprite.Width;
                Height = _sprite.Height;
            }
            else
            {
                Width = _sheet.Width;
                Height = _sheet.Height;
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            VisualBounds = new Rectangle((int) Position.X, (int) Position.Y, Width, Height);
            CollisionBounds = VisualBounds;


            if (_sheet is IUpdateable updateable)
                updateable.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float z = 0.7f;
            if (Position.Y + Height > _playerCharacter.Position.Y + 32)
            {
                z = 0.4f;
            }

            z -= Position.Y * 0.00001f + Position.X * 0.00001f;


            if (_sprite != null)
            {
                _sprite.Draw(spriteBatch, Position, layerDepth: z);
            }
            else
            {
                _sheet.Draw(spriteBatch, Position, layerDepth: z);
            }

            if (DebugTools.GenericDebugEnabled)
                _rectangle.Draw(spriteBatch, VisualBounds);

        }
    }
}
