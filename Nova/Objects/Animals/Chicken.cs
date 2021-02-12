using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nova.Common.Sprite;
using Nova.Objects.Character;

namespace Nova.Objects.Animals
{
    public class Chicken : LivingGameObject
    {
        private AnimationSet _walkingAnimation;
        private AnimationSet _eatingAnimation;

        private AnimatedSpriteSheet _currentAnimationSheet;

        public Chicken(GameServiceContainer services) : base(services)
        {
            SupportedActions[nameof(Eat)] = new LivingAction()
            {
                Action = Eat
            };
        }

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);

            _walkingAnimation = new AnimationSetBuilder(contentManager)
                .WithAsset("Animals/AnimalChickenWalk")
                .WithName("Walk")
                .WithFrameCount(4)
                .WithIndex(Orientation.Left, 4)
                .WithIndex(Orientation.Top, 0)
                .WithIndex(Orientation.Bottom, 8)
                .WithIndex(Orientation.Right, 12)
                .Build();

            _eatingAnimation = new AnimationSetBuilder(contentManager)
                .WithAsset("Animals/AnimalChickenEat")
                .WithName("Walk")
                .WithFrameCount(4)
                .WithIndex(Orientation.Left, 4)
                .WithIndex(Orientation.Top, 0)
                .WithIndex(Orientation.Bottom, 8)
                .WithIndex(Orientation.Right, 12)
                .WithAnimatedSpriteSheetOptions(z => z.Repeat = false)
                .Build();
            
            foreach (var tempSheet in _walkingAnimation.Animations.SelectMany(z => z.AnimatedSpriteSheets))
                tempSheet.Start();
        }

        private void Eat(GameTime gameTime, LivingAction action)
        {
            var anim = _eatingAnimation.GetAnimationForOrientation(Orientation);
            var sheet = anim.AnimatedSpriteSheets.FirstOrDefault();
            
            sheet.Update(gameTime);
            
            if (sheet.HasEnded)
            {
                action.IsRunning = false;
                sheet.Reset();
                
                _currentAnimationSheet = anim.AnimatedSpriteSheets.FirstOrDefault();
                _currentAnimationSheet.Reset();
                return;
            }
            
            _currentAnimationSheet = sheet;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            var anim = _walkingAnimation.GetAnimationForOrientation(Orientation);
            var sheet = anim.AnimatedSpriteSheets.FirstOrDefault();
            if (!IsMoving)
                sheet.Reset();
            else
                sheet.Update(gameTime);

            if (ActiveActions.Any())
            {
                foreach (var act in ActiveActions)
                {
                    act.Action(gameTime, act);
                }

                return;
            }

            _currentAnimationSheet = anim.AnimatedSpriteSheets.FirstOrDefault();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            
            float zDepth = 0.7f - Position.Y * 0.00001f + Position.X * 0.00001f;

            _currentAnimationSheet.Draw(spriteBatch, Position, layerDepth: zDepth);
        }
    }
}
