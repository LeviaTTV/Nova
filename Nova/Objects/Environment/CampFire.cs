using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Sprite;
using Nova.Objects.Character;
using Penumbra;

namespace Nova.Objects.Environment
{
    public class CampFire : GameObject
    {
        private AnimatedSpriteSheet _sheet;
        private PenumbraComponent _penumbra;
        private PointLight _light;


        private bool _expand;
        private double _accumulator;
        private PlayerCharacter _playerCharacter;

        public override bool CollisionEnabled => false;

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
            float z = 0.7f;
            if (Position.Y + Height + 8f > _playerCharacter.Position.Y + 32)
                z = 0.4f;

            z -= Position.Y * 0.00001f + Position.X * 0.00001f;

            _sheet.Draw(spriteBatch, Position, layerDepth: z);
        }
    }
}
