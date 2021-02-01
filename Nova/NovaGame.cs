using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nova.Common.Sprite;
using Nova.Environment;
using Nova.Primitives;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Nova
{
    public class NovaGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Map _map;
        private MapRenderer _mapRenderer;

        private int _cumulativeScrollWheelValue = 0;

        private Camera2D _camera2D;

        public NovaGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferMultiSampling = true;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _camera2D = new Camera2D(GraphicsDevice.Viewport, int.MaxValue, int.MaxValue, 1f);



            // Generate a chunk in our new map
            var mapGenerator = new MapGenerator(GraphicsDevice, 8);
            _map = new Map()
            {
                ChunkSize = mapGenerator.ChunkSize
            };

            _mapRenderer = new MapRenderer(GraphicsDevice, _map, mapGenerator, _camera2D);
            _mapRenderer.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouseState = Mouse.GetState();

            int mouseWheelValue = mouseState.ScrollWheelValue;
            if (mouseWheelValue != _cumulativeScrollWheelValue)
            {
                if (mouseWheelValue < _cumulativeScrollWheelValue)
                    //_scale -= 0.1f;
                    _camera2D.Zoom += 0.1f;
                else
                    //_scale += 0.1f;
                    _camera2D.Zoom -= 0.1f;
            }

            _cumulativeScrollWheelValue = mouseWheelValue;

            var keyboardState = Keyboard.GetState();

            Vector2 movement = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.A))
                movement.X--;
            if (keyboardState.IsKeyDown(Keys.D))
                movement.X++;
            if (keyboardState.IsKeyDown(Keys.W))
                movement.Y--;
            if (keyboardState.IsKeyDown(Keys.S))
                movement.Y++;

            _camera2D.Pos += movement * 20;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(sortMode: SpriteSortMode.Texture, samplerState: SamplerState.PointClamp, transformMatrix: _camera2D.GetTransformation());

            _mapRenderer.Draw(_spriteBatch);

            _spriteBatch.End();
        }
    }
}
