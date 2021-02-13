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

namespace Nova.Objects.Character
{
    public class PlayerCharacter : LivingGameObject
    {
        private readonly MapService _mapService;

        private Tile _collisionCheckTile;
        private PrimitiveRectangle _rectangle;
        private PrimitiveRectangle _rectangle2;
        
        private readonly GameObjectManager _gameObjectManager;
        private readonly Camera2D _camera;


        private AnimationSet _walkingAnimationSet;
        private Animation _currentAnimation;

        public PlayerCharacter(GameServiceContainer services) : base(services)
        {
            _mapService = services.GetService<MapService>();
            _gameObjectManager = services.GetService<GameObjectManager>();
            _camera = services.GetService<Camera2D>();
        }

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            
            // Move to content json
            _walkingAnimationSet = new AnimationSetBuilder(contentManager)
                .WithName("Walk")
                .WithAsset("Character/Female/BodyFemaleLight")
                .WithAsset("Character/Hair/HairLongPink")
                .WithAsset("Character/Torso/FemaleBlackCorset")
                .WithAsset("Character/Legs/FemaleRedPants")
                .WithFrameCount(8)
                .WithIndex(Orientation.Left, 118)
                .WithIndex(Orientation.Top, 105)
                .WithIndex(Orientation.Bottom, 131)
                .WithIndex(Orientation.Right, 144)
                .Build();
            
            _rectangle = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Blue, false);
            _rectangle2 = new PrimitiveRectangle(Services.GetService<GraphicsDevice>(), Color.Green, false);

            Origin = new Vector2(32, 64);

            Width = 32;
            Height = 64;
        }

        private AnimatedSpriteSheet GenerateAnimatedSpriteSheet(IEnumerable<Sprite> sprites)
        {
            var sheet = new AnimatedSpriteSheet();
            sheet.TimeBetweenSprites = 83;
            foreach (var sprite in sprites)
                sheet.Sprites[sprite.Name] = sprite;

            return sheet;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var state = Keyboard.GetState();

            float moveSpeed = 250f * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f);

            bool anyDown = false;
            var newPosition = Position;
            if (state.IsKeyDown(Keys.W))
            {
                newPosition.Y += -moveSpeed;
                Orientation = Orientation.Top;
                anyDown = true;
            }

            if (state.IsKeyDown(Keys.A))
            {
                newPosition.X += -moveSpeed;
                Orientation = Orientation.Left;
                anyDown = true;
            }

            if (state.IsKeyDown(Keys.D))
            {
                newPosition.X += moveSpeed;
                Orientation = Orientation.Right;
                anyDown = true;
            }

            if (state.IsKeyDown(Keys.S))
            {
                newPosition.Y += moveSpeed;
                Orientation = Orientation.Bottom;
                anyDown = true;
            }


            // Collision detection
            if (anyDown)
            {
                var tile = _mapService.GetTileAt(newPosition);
                _collisionCheckTile = tile;

                CollisionBounds = new Rectangle((int)(newPosition.X - Origin.X / 2) + 4, (int)(newPosition.Y - Origin.Y) + 32 + 8, 32 - 8, 24);

                var gameObjects = _gameObjectManager.GetCollidingFoliageGameObjects(this, CollisionBounds);

                if (tile != null && tile.Traversable && !gameObjects.Any())
                    Position = newPosition;
            }
            
            _currentAnimation = _walkingAnimationSet.GetAnimationForOrientation(Orientation);
            foreach (var sheets in _currentAnimation.AnimatedSpriteSheets)
            {
                sheets.Update(gameTime);

                if (!anyDown)
                    sheets.Reset();
                else
                    sheets.Update(gameTime);
            }

            _camera.Position = new Vector2((int)Position.X, (int)Position.Y);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            int count = 0;
            
            foreach (var sheets in _currentAnimation.AnimatedSpriteSheets)
            {
                ++count;
                sheets.Draw(spriteBatch, Position, 0f, Origin, SpriteEffects.None, 0.5f - 0.01f * count);
            }

            if (DebugTools.GenericDebugEnabled)
            {
                if (_collisionCheckTile != null)
                    _rectangle.Draw(spriteBatch, new Vector2(_collisionCheckTile.X * 32f, _collisionCheckTile.Y * 32), 32, 32);
                
                _rectangle2.Draw(spriteBatch, CollisionBounds);

                _rectangle.Draw(spriteBatch, new Vector2(Position.X, Position.Y), 2, 2);
            }
        }
    }
}
