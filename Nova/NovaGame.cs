using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nova.Common.Primitives;
using Nova.Environment;
using Nova.Environment.Generation;
using Nova.GUI;
using Nova.GUIEngine;
using Nova.GUIEngine.Units;
using Nova.Objects.Animals;
using Nova.Objects.Character;
using Nova.Services;
using Penumbra;

namespace Nova
{
    public class NovaGame : Game
    {
        private const int AIUpdateIntervalMilliseconds = 500;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Map _map;
        private MapRenderer _mapRenderer;
        private Camera2D _camera2D;
        private readonly PenumbraComponent _penumbra;

        private GameObjectManager _gameObjectManager;

        private PlayerCharacter _character;
        private TimeService _timeService;
        private UIManager _uiManager;
        private AIService _aiService;

        private PathFindingService _pathFindingService;
        private Chicken _chicken;

        private PrimitiveRectangle _temp;

        private double _aiUpdateAccumulator;

        public NovaGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;
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
            _graphics.IsFullScreen = false;
            _graphics.HardwareModeSwitch = false;
            _graphics.ApplyChanges();

            _penumbra.Initialize();
            _penumbra.AmbientColor = Color.FromNonPremultiplied(255, 255, 255, 255);
        }

        protected override void LoadContent()
        {
            _camera2D = new Camera2D(GraphicsDevice.Viewport);
            Services.AddService(_camera2D);

            Services.AddService(_penumbra);
            Services.AddService(_timeService = new TimeService(Services));
            Services.AddService(GraphicsDevice);
            Services.AddService(_aiService = new AIService(Services, Content));

            var mapService = new MapService();
            Services.AddService(mapService);

            _gameObjectManager = new GameObjectManager();
            Services.AddService(_gameObjectManager);

            _pathFindingService = new PathFindingService(mapService, _gameObjectManager);

            _character = new PlayerCharacter(Services);
            _character.LoadContent(Content);
            Services.AddService(_character);
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Services.AddService(_uiManager = new UIManager(Services, Content));

            //var mapGenerator = new MapGenerator(Services, GraphicsDevice, Guid.NewGuid().GetHashCode(), Content);
            var mapGenerator = new MapGenerator(Services, GraphicsDevice, -425262119, Content);
            //_map = mapGenerator.Generate(400, 400);
            _map = mapGenerator.Generate(100, 100);

            mapService.Map = _map;
            _character.Position = _map.StartPosition;

            _gameObjectManager.LoadAll(Content);
            
            _pathFindingService = new PathFindingService(mapService, _gameObjectManager);
            _pathFindingService.Initialize();
            Services.AddService(_pathFindingService);

            
            _mapRenderer = new MapRenderer(Services, _map);
            _mapRenderer.LoadContent(Content);
            
            
            _uiManager.Initialize();
            _uiManager.AddVisual(new HotbarPanel());
            _uiManager.AddVisual(new TimePanel(Services)
            {
                Position = new UVector2(UDim.Relative(0.01f), UDim.Relative(0.5f))
            });

            _chicken = new Chicken(Services);
            _chicken.LoadContent(Content);
            _chicken.Position = _character.Position + new Vector2(4f * 32f, 4f * 32f);

            _gameObjectManager.AddGameObject(_chicken);
            _gameObjectManager.AddGameObject(_character);

            _temp = new PrimitiveRectangle(GraphicsDevice, Color.Black, false);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _camera2D.UpdateCamera(GraphicsDevice.Viewport);

            DebugTools.GenericDebugEnabled = Keyboard.GetState().IsKeyDown(Keys.E);
            DebugTools.DoNotRenderTileTransitions = Keyboard.GetState().IsKeyDown(Keys.R);
            DebugTools.IncreaseSpeedOfTime = Keyboard.GetState().IsKeyDown(Keys.T);

            _timeService.OneMinutePassesEveryXMilliseconds = DebugTools.IncreaseSpeedOfTime ? 10 : 1000;
            
            _penumbra.Transform = _camera2D.Transform;

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                var screenLoc = Mouse.GetState().Position.ToVector2();
                var worldLoc = Vector2.Transform(screenLoc, Matrix.Invert(_camera2D.Transform));
                _character.Position = worldLoc;
                _camera2D.Position = _character.Position;
            }

            _timeService.Update(gameTime);

            foreach (var gameObject in _gameObjectManager.GameObjects)
                gameObject.Update(gameTime);

            UpdateAI(gameTime);

            _uiManager.Update(gameTime);
        }

        private void UpdateAI(GameTime gameTime)
        {
            _aiUpdateAccumulator += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_aiUpdateAccumulator >= AIUpdateIntervalMilliseconds)
            {
                _aiService.Update();
                _aiUpdateAccumulator = 0;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            _penumbra.BeginDraw();
            
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp, transformMatrix: _camera2D.Transform);
            _mapRenderer.Draw(_spriteBatch);
            

            DrawGameObjects();


            if (DebugTools.GenericDebugEnabled)
            {
                foreach (var bla in _pathFindingService.NotIncludedTiles)
                {
                    _temp.Draw(_spriteBatch, new Rectangle(bla.X * 32, bla.Y * 32, 32, 32));
                }
            }
            

            _spriteBatch.End();
            
            _penumbra.Draw(gameTime);

            _uiManager.Draw();
        }

        private void DrawGameObjects()
        {
            var rectBounds = _camera2D.VisibleArea;
            rectBounds.X -= 32;
            rectBounds.Y -= 32;
            rectBounds.Width += 32;
            rectBounds.Height += 32;

            int endY = rectBounds.Y + rectBounds.Height;
            int endX = rectBounds.X + rectBounds.Width;

            foreach (var gameObject in _gameObjectManager.FoliageGameObjects.Values)
            {
                int objectX = gameObject.Tile.X * 32;
                int objectY = gameObject.Tile.Y * 32;
                if (objectX + gameObject.Width >= rectBounds.X && objectY + gameObject.Height >= rectBounds.Y && objectX - gameObject.Width <= endX && objectY - gameObject.Height <= endY)
                    gameObject.Draw(_spriteBatch);
            }

            foreach (var gameObject in _gameObjectManager.GameObjects)
            {
                if (gameObject.Position.X + gameObject.Width >= rectBounds.X && gameObject.Position.Y + gameObject.Height >= rectBounds.Y && gameObject.Position.X - gameObject.Width <= endX && gameObject.Position.Y - gameObject.Height <= endY)
                    gameObject.Draw(_spriteBatch);
            }
        }
    }
}
