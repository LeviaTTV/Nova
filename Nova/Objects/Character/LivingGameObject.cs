using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.AI.BT;
using Nova.Common.AI.BT.Base;
using Nova.Common.Extensions;
using Nova.Common.Primitives;
using Nova.Services;

namespace Nova.Objects.Character
{
    public abstract class LivingGameObject : GameObject
    {
        public override bool CollisionEnabled => true;
        public Orientation Orientation { get; set; }
        public NodeStatus LastAINodeStatus { get; set; }
        
        private List<Vector2> _walkingToList;
        private Vector2 _targetPoint;
        private PrimitiveLine _line;

        protected AIService AIService;
        protected Node BehaviorTree;

        protected LivingGameObject(GameServiceContainer services) : base(services)
        {
            AIService = Services.GetService<AIService>();
        }

        public override void LoadContent(ContentManager contentManager)
        {
            var device = Services.GetService<GraphicsDevice>();
            
            var behaviorTreeService = Services.GetService<AIService>();

            BehaviorTree = behaviorTreeService.GetBehaviorTreeThatAppliesTo(this.GetType().Name);
            behaviorTreeService.Register(this);

            _line = new PrimitiveLine(device, Color.Red);
        }

        public virtual void SuspendAI()
        {
            AIService.Unregister(this);
        }

        public virtual void ResumeAI()
        {
            AIService.Register(this);
        }

        public void UpdateAI()
        {
            if (BehaviorTree == null)
                return;

            LastAINodeStatus = BehaviorTree.Execute();
        }

        public void MoveTo(Point point)
        { 
            var pathFindingService =  Services.GetService<PathFindingService>();

            _walkingToList = pathFindingService.FindPath((Position / 32f).ToPoint(), point).Select(z => z.ToVector2() * 32f).ToList();

            if (_walkingToList.Any())
                SuspendAI();

            _targetPoint = _walkingToList.FirstOrDefault();
        }
        
        public override void Update(GameTime gameTime)
        {
            if (_walkingToList == null || !_walkingToList.Any())
                return;

            if (Vector2.Distance(Position, _targetPoint) < 5f)
            {
                _walkingToList.Remove(_targetPoint);
                _targetPoint = _walkingToList.FirstOrDefault();
            }
            
            var currentTargetPoint = _targetPoint;
            var elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            var distance = Vector2.Distance(Position, currentTargetPoint);
            var interpolatedVector = Vector2.Lerp(Position, currentTargetPoint, 0.1f / distance * elapsed);
            
            float xDif = interpolatedVector.X - Position.X;
            float yDif = interpolatedVector.Y - Position.Y;
            if (Math.Abs(xDif) > Math.Abs(yDif))
            {
                Orientation = xDif < 0 ? Orientation.Left : Orientation.Right;
            }
            else
            {
                Orientation = yDif < 0 ? Orientation.Top : Orientation.Bottom;
            }
            
            Position = interpolatedVector;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_walkingToList == null)
                return;

            if (_walkingToList.Any())
            {
                Vector2 lastPoint = default;
                foreach (var point in _walkingToList)
                {
                    if (lastPoint != default(Vector2))
                    {
                        _line.Draw(spriteBatch, lastPoint, point);
                    }

                    lastPoint = point;
                }
            }
        }
    }
}