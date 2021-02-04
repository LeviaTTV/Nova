using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nova.Common.Sprite;
using Nova.Environment;
using Penumbra;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Nova
{
    public class NovaGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Map _map;
        private MapRenderer _mapRenderer;

        private Camera2D _camera2D;

        private PenumbraComponent _penumbra;

        public NovaGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferHalfPixelOffset = true;
            Content.RootDirectory = "Content";
            
            IsMouseVisible = true;

            _penumbra = new PenumbraComponent(this);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferMultiSampling = true;
            _graphics.ApplyChanges();

            _penumbra.Initialize();
            _penumbra.AmbientColor = Color.FromNonPremultiplied(255, 255, 255, 255);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _camera2D = new Camera2D(GraphicsDevice.Viewport);
            
            var mapGenerator = new MapGenerator(GraphicsDevice, Guid.NewGuid().GetHashCode(), Content);
            _map = mapGenerator.Generate(400, 400);

            foreach (var gameObj in _map.GameObjects)
                gameObj.LoadContent(Content);

            _mapRenderer = new MapRenderer(GraphicsDevice, _map, _camera2D);
            _mapRenderer.LoadContent(Content);

            _ds = Content.Load<SpriteSheet>("treesTest");
        }

        private SpriteSheet _ds;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _camera2D.UpdateCamera(GraphicsDevice.Viewport);

            _mapRenderer.DebugMode = Keyboard.GetState().IsKeyDown(Keys.E);
            _mapRenderer.DoNotRenderTransitions = Keyboard.GetState().IsKeyDown(Keys.R);
            
            _penumbra.Transform = _camera2D.Transform;
        }

        protected override void Draw(GameTime gameTime)
        {
            _penumbra.BeginDraw();
            
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp, transformMatrix: _camera2D.Transform);
            _mapRenderer.Draw(_spriteBatch);
            _ds["Tree1"].Draw(_spriteBatch, Vector2.One);
            _spriteBatch.End();


            _penumbra.Draw(gameTime);
        }
    }
}
