using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nova.Common.Primitives;
using Nova.Common.Sprite;
using Nova.Environment;
using Nova.Services;
using Penumbra;

namespace Nova.Objects.Character
{
    public class PlayerCharacter : CharacterGameObject
    {
        private List<DirectionCharacterParts> _characterParts = new List<DirectionCharacterParts>();

        private Orientation _facingOrientation = Orientation.Bottom;

        private DirectionCharacterParts _characterPartsDrawable;
        private MapService _mapService;

        private Tile _collisionCheckTile;
        private PrimitiveRectangle _rectangle;
        private PrimitiveRectangle _rectangle2;

        private Vector2 _origin;
        private GameObjectManager _gameObjectManager;
        private PenumbraComponent _penumbra;

        private Hull _hull;

        public PlayerCharacter(GameServiceContainer services) : base(services)
        {
            _mapService = services.GetService<MapService>();
            _gameObjectManager = services.GetService<GameObjectManager>();
            _penumbra = services.GetService<PenumbraComponent>();
        }

        public override void LoadContent(ContentManager contentManager)
        {
            var parts = new[] {
                contentManager.Load<SpriteSheet>("Character/Female/BodyFemaleLight"),
                contentManager.Load<SpriteSheet>("Character/Hair/HairLongPink"),
                contentManager.Load<SpriteSheet>("Character/Torso/FemaleBlackCorset"),
                contentManager.Load<SpriteSheet>("Character/Feet/FemaleBrownLongboots"),
                contentManager.Load<SpriteSheet>("Character/Legs/FemaleRedPants")
            };


            var directionDict = new Dictionary<Orientation, int>()
            {
                { Orientation.Bottom, 131 },
                { Orientation.Top, 105 },
                { Orientation.Left, 118 },
                { Orientation.Right, 144 },
            };

            foreach (var direction in Enum.GetValues(typeof(Orientation)).Cast<Orientation>())
            {
                var directionParts = new DirectionCharacterParts()
                {
                    Orientation = direction
                };

                foreach (var sheet in parts)
                {
                    directionParts.SpriteSheets.Add(GenerateAnimatedSpriteSheet(sheet.GetSprites(directionDict[direction], 8)));
                }


                _characterParts.Add(directionParts);
            }

            _rectangle = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Blue, false);
            _rectangle2 = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Green, false);

            _origin = new Vector2(32, 64);

            Width = 32;
            Height = 64;

            _hull = new Hull(new Vector2(0, 0) - _origin, new Vector2(Width, 0) - _origin, new Vector2(Width, Height) - _origin, new Vector2(0, Height) - _origin);
            _hull.Position = Position;
            _hull.Enabled = false;
            _penumbra.Hulls.Add(_hull);
        }

        private AnimatedSpriteSheet GenerateAnimatedSpriteSheet(IEnumerable<Sprite> sprites)
        {
            var sheet = new AnimatedSpriteSheet();
            sheet.TimeBetweenSprites = 83;
            foreach (var sprite in sprites)
            {
                sheet.Sprites[sprite.Name] = sprite;
            }

            return sheet;
        }

        public override void Update(GameTime gameTime)
        {
            float moveSpeed = 250f * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f);

            var state = Keyboard.GetState();

            bool anyDown = state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.A) || state.IsKeyDown(Keys.S) || state.IsKeyDown(Keys.D);

            var newPosition = Position;

            if (state.IsKeyDown(Keys.W))
            {
                newPosition.Y += -moveSpeed;
                _facingOrientation = Orientation.Top;
            }

            if (state.IsKeyDown(Keys.A))
            {
                newPosition.X += -moveSpeed;
                _facingOrientation = Orientation.Left;
            }

            if (state.IsKeyDown(Keys.D))
            {
                newPosition.X += moveSpeed;
                _facingOrientation = Orientation.Right;
            }

            if (state.IsKeyDown(Keys.S))
            {
                newPosition.Y += moveSpeed;
                _facingOrientation = Orientation.Bottom;
            }


            // Collision detection
            if (anyDown)
            {
                var tile = _mapService.GetTileAt(newPosition);
                _collisionCheckTile = tile;

                CollisionBounds = new Rectangle((int)(newPosition.X - _origin.X / 2) + 4, (int)(newPosition.Y - _origin.Y) + 32 + 8, 32 - 8, 24);

                var gameObjects = _gameObjectManager.GetCollidingFoliageGameObjects(CollisionBounds);

                if (tile != null && tile.Traversable && !gameObjects.Any())
                    Position = newPosition;
            }


            _characterPartsDrawable = _characterParts.FirstOrDefault(x => x.Orientation == _facingOrientation);
            foreach (var sheets in _characterPartsDrawable.SpriteSheets)
            {
                sheets.Update(gameTime);

                if (!anyDown)
                    sheets.Reset();
                else
                    sheets.Update(gameTime);
            }

            _hull.Position = Position;
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            int count = 0;
            
            foreach (var sheets in _characterPartsDrawable.SpriteSheets)
            {
                ++count;
                sheets.Draw(spriteBatch, Position, 0f, _origin, SpriteEffects.None, 0.5f - 0.01f * count);
            }

            if (DebugTools.GenericDebugEnabled)
            {
                if (_collisionCheckTile != null)
                    _rectangle.Draw(spriteBatch, new Vector2(_collisionCheckTile.X * 32f, _collisionCheckTile.Y * 32), 32, 32);
                
                _rectangle2.Draw(spriteBatch, CollisionBounds);
            }
        }
    }
}
