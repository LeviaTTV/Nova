using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nova.GUIEngine.Base;
using IUpdateable = Nova.Common.IUpdateable;

namespace Nova.GUIEngine
{
    /// <summary>
    /// My mom told me to write some more documentation.
    /// This is the UI manager which is basically that class that managed all windows. I've tried to make this system flexible by having a
    ///  Visual base class that already handles children, drawing, loading content and such for you. If more content needs to be loaded,
    ///  one simply overrides the LoadContent method.
    ///
    /// The entire system uses a 'relative' unit system with at its core the UDim struct. It is able to represent both a relative value (0.0 - 1.0)
    ///  and an absolute offset. For example, Position = new UVector(new UDim(1f, -25f), UDim.Absolute(5f)) which would indicate right side parent
    ///  anchoring (if no parent, calculations based off of screen resolution of course) minus 25 pixels. BTW it doesn't actually 'anchor' stuff
    ///  when changing sizes etc. That feature seemed unnecessary for this project.
    ///
    /// Panels can have interaction triggers, currently only one is available that allows you to perform an action when a key is pressed (showing/hiding).
    /// Visuals receive mouse events by default and can receive keyboard events if they register a key into the InterestedKeys list. The UIManager deals
    ///  with duplicate events being fired - because of the nature of key handling there is only 'down' and 'up' states. If you were not to track the states,
    ///  you end up with multiple Update()s coming in (causing multiple events) because the Update comes around 60 times a second (and you're probably fat
    ///  fingering that button for longer than 16.67ms).
    ///
    /// Nothing is being scaled unless Visuals use relative sizing (through USize). It's not recommended though because art very quickly gets messed up and I
    ///  don't quite have the budget for Autodesk Scaleform...
    /// </summary>
    public class UIManager
    {
        private readonly GameServiceContainer _services;
        private readonly ContentManager _contentManager;

        public bool IsVisible { get; set; } = true;

        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                _scaleTransformMatrix = Matrix.CreateScale(_scale, _scale, 1f);
                _context.Scale = value;
            }
        }

        private Matrix _scaleTransformMatrix;
        private SpriteBatch _uiSpriteBatch;
        private float _scale;

        private readonly List<Visual> _visuals = new List<Visual>();

        private readonly List<Visual> _receivingLeftMouseDownEvent = new List<Visual>();
        private List<(Visual, Keys)> _receivingKeyDownEvent = new List<(Visual, Keys)>();

        private GraphicsDevice _graphicsDevice;

        private DrawingContext _context;

        public UIManager(GameServiceContainer services, ContentManager contentManager)
        {
            _services = services;
            _contentManager = contentManager;
        }

        public void Initialize()
        {
            _graphicsDevice = _services.GetService<GraphicsDevice>();
            _uiSpriteBatch = new SpriteBatch(_graphicsDevice);

            _context = new DrawingContext()
            {
                GraphicsDevice = _graphicsDevice,
                SpriteBatch = _uiSpriteBatch
            };

            Scale = 1f;
        }

        public void AddVisual(Visual visual)
        {
            _visuals.Add(visual);
            visual.LoadContent(_contentManager);

            foreach (var child in visual.Children)
                child.LoadContent(_contentManager);
        }
        
        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.OemPlus))
                Scale += 0.005f;

            if (keyboardState.IsKeyDown(Keys.OemMinus))
                Scale -= 0.005f;

            // Mouse events
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                var position = mouseState.Position.ToVector2();
                var localPosition = Vector2.Transform(position, Matrix.Invert(_scaleTransformMatrix));

                foreach (var visual in _visuals)
                {
                    if (localPosition.X >= visual.ActualPosition.X.Absolute() && localPosition.Y >= visual.ActualPosition.Y.Absolute() &&
                        localPosition.X <= visual.ActualPosition.X.Absolute() + visual.ActualSize.Width.Absolute() && localPosition.Y <= visual.ActualPosition.X.Absolute() + visual.ActualSize.Height.Absolute())
                    {
                        visual.OnLeftMouseDown(localPosition);
                        _receivingLeftMouseDownEvent.Add(visual);

                        foreach (var child in visual.Children)
                        {
                            if (localPosition.X >= child.ActualPosition.X.Absolute() && localPosition.Y >= child.ActualPosition.Y.Absolute() &&
                                localPosition.X <= child.ActualPosition.X.Absolute() + child.ActualSize.Width.Absolute() && localPosition.Y <= child.ActualPosition.X.Absolute() + child.ActualSize.Height.Absolute())
                            {
                                child.OnLeftMouseDown(localPosition);
                                _receivingLeftMouseDownEvent.Add(child);
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                if (_receivingLeftMouseDownEvent.Any())
                {
                    var position = mouseState.Position.ToVector2();
                    var localPosition = Vector2.Transform(position, Matrix.Invert(_scaleTransformMatrix));

                    foreach (var visual in _receivingLeftMouseDownEvent)
                        visual.OnLeftMouseUp(localPosition);

                    _receivingLeftMouseDownEvent.Clear();
                }
            }

            // Keyboard events
            var currentPressedKeys = keyboardState.GetPressedKeys();

            var ls = new List<(Visual, Keys)>();
            foreach (var interestedParty in _receivingKeyDownEvent)
            {
                var visual = interestedParty.Item1;
                var key = interestedParty.Item2;

                if (currentPressedKeys.Contains(key))
                    ls.Add(interestedParty);
                else
                    visual.OnInterestedKeyUp(key);
            }

            _receivingKeyDownEvent = ls;

            foreach (var key in currentPressedKeys)
            {
                var visualInterested = _visuals.Where(x => x.InterestedKeys.Contains(key));

                foreach (var interested in visualInterested)
                {
                    interested.OnInterestedKeyDown(key);
                    _receivingKeyDownEvent.Add((interested, key));
                }
            }

            // IUpdateables
            foreach (var visual in _visuals.OfType<IUpdateable>())
                visual.Update(gameTime);
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            _uiSpriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp, transformMatrix: _scaleTransformMatrix);

            foreach (var visual in _visuals.Where(x => !x.IsHidden))
            {
                _context.InitializeVisualDraw(visual);
                visual.Draw(_context);

                foreach (var child in visual.Children.Where(x => !x.IsHidden))
                {
                    _context.InitializeVisualDraw(child);
                    child.Draw(_context);
                }
            }

            _uiSpriteBatch.End();
        }
    }
}
