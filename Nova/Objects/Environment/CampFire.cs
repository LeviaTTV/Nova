using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Primitives;
using Nova.Common.Sprite;
using Nova.Objects.Character;
using Penumbra;

namespace Nova.Objects.Environment
{
    public class CampFire : GameObject
    {
        private AnimatedSpriteSheet _sheet;
        private readonly PenumbraComponent _penumbra;
        private PointLight _light;


        private bool _expand;
        private double _accumulator;
        private readonly PlayerCharacter _playerCharacter;

        public override bool CollisionEnabled => false;

        private PrimitiveRectangle _rectangle;

        public CampFire(GameServiceContainer services) : base(services)
        {
            _penumbra = services.GetService<PenumbraComponent>();
            _playerCharacter = services.GetService<PlayerCharacter>();
        }

        public override void LoadContent(ContentManager contentManager)
        {
            _sheet = contentManager.Load<AnimatedSpriteSheet>("Environment/CampFire");

            _light = new PointLight()
            {
                CastsShadows = true,
                Scale = new Vector2(800f),
                ShadowType = ShadowType.Illuminated
            };

            _penumbra.Lights.Add(_light);

            Height = _sheet.Sprites.FirstOrDefault().Value.Height;
            Width = _sheet.Sprites.FirstOrDefault().Value.Width;

            _rectangle = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Red, false);
        }

        public override void Update(GameTime gameTime)
        {
            _light.Position = Position + new Vector2(32f, 56f);
            _accumulator += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_accumulator >= 50)
            {
                if (_expand)
                    _light.Scale += new Vector2(4);
                else
                    _light.Scale -= new Vector2(4);

                _expand = !_expand;
                _accumulator = 0;
            }

            _sheet.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _sheet.Draw(spriteBatch, Position);

            if (DebugTools.GenericDebugEnabled)
            {
                _rectangle.Draw(spriteBatch, Position + new Vector2(0, Height), 2, 2);
            }
        }
    }
}
